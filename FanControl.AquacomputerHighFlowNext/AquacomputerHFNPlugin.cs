using FanControl.Plugins;
using System;
using System.Linq;
using System.Threading;

namespace FanControl.AquacomputerHighFlowNext
{
    public class AquacomputerHFNPlugin : IPlugin2
    {
        private HidLibrary.HidDevice HidDevice = null;
        internal ReaderWriterLock rwl;
        internal HighFlowNextSensorsStructs.HighFlowNext_SensorData data;

        public string Name => "Acquacomputer High Flow Next Plugin";

        private readonly IPluginLogger _logger;
        public AquacomputerHFNPlugin(IPluginLogger logger)
        {
            _logger = logger;
            rwl = new ReaderWriterLock();
            data = new HighFlowNextSensorsStructs.HighFlowNext_SensorData();
        }

        public void Close()
        {
            HidDevice.CloseDevice();
            HidDevice = null;
        }

        public void Initialize()
        {
            HidLibrary.HidEnumerator hidEnumerator = new HidLibrary.HidEnumerator();
            HidDevice = (HidLibrary.HidDevice)hidEnumerator.Enumerate(0x0C70, 0xF012).FirstOrDefault();

            if (HidDevice == null)
            {
                _logger.Log("AquacomputerHFNPlugin: ERROR! Unable to find device!");
                return;
            }

        }

        public void Load(IPluginSensorsContainer _container)
        {
            if (HidDevice != null)
            {
                _container.TempSensors.Add(new BaseSensor(this, "TemperatureWater", "Water Temperature", (x) => ((short)x) / 100.0f));
                _container.TempSensors.Add(new BaseSensor(this, "TemperatureExt", "External Water Temperature", (x) => ((short)x) / 100.0f));
                _container.FanSensors.Add(new BaseSensor(this, "Flow", "Flow", (x) => ((short)x) / 10.0f));
                _container.FanSensors.Add(new BaseSensor(this, "WaterQuality", "Water Quality", (x) => ((short)x) / 100.0f));
                _container.FanSensors.Add(new BaseSensor(this, "Conductivity", "Conductivity", (x) => ((short)x) / 10.0f));
            }
        }

        public void Update()
        {
            if (HidDevice == null)
                return;

            try
            {
                rwl.AcquireWriterLock(100);
                try
                {
                    var deviceData = HidDevice.Read(500);

                    if (deviceData != null && deviceData.Status == HidLibrary.HidDeviceData.ReadStatus.Success)
                    {
                        int offset = 0;
                        var hfnHeader = EndianAttribute.GetStructAtOffset<HighFlowNextSensorsStructs.HighFlowNext_Header>(deviceData.Data, ref offset);
                        var hfnFirmware = EndianAttribute.GetStructAtOffset<HighFlowNextSensorsStructs.HighFlowNext_Firmware1012>(deviceData.Data, ref offset);
                        data = EndianAttribute.GetStructAtOffset<HighFlowNextSensorsStructs.HighFlowNext_SensorData>(deviceData.Data, ref offset);
                    }
                }
                finally
                {
                    rwl.ReleaseWriterLock();
                }
            } catch (ApplicationException ex) {
                _logger.Log("AquacomputerHFNPlugin Exception: " + ex);
            }
        }
    }
}
