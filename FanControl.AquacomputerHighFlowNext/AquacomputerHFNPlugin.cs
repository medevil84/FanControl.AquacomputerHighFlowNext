using FanControl.Plugins;
using System;
using System.Linq;
using System.Threading;

namespace FanControl.AquacomputerHighFlowNext
{
    public class AquacomputerHFNPlugin : IPlugin
    {
        private Timer timer;
        private HidLibrary.HidDevice HidDevice;
        internal ReaderWriterLock rwl = new ReaderWriterLock();
        internal HighFlowNextSensorsStructs.HighFlowNext_SensorData data = new HighFlowNextSensorsStructs.HighFlowNext_SensorData();

        public string Name => "Acquacomputer High Flow Next Plugin";

        public void Close()
        {
            timer.Dispose();
            HidDevice.CloseDevice();
            HidDevice = null;
        }

        public void Initialize()
        {
            HidLibrary.HidEnumerator hidEnumerator = new HidLibrary.HidEnumerator();
            HidDevice = (HidLibrary.HidDevice)hidEnumerator.Enumerate(0x0C70, 0xF012).FirstOrDefault();

            if (HidDevice == null)
                return;

            timer = new Timer(DataRefreshTimerCallback, null, 0, 1000);
        }

        public void Load(IPluginSensorsContainer _container)
        {
            _container.TempSensors.Add(new BaseSensor(this, "TemperatureWater", "Water Temperature", (x) => ((short)x) / 100.0f));
            _container.TempSensors.Add(new BaseSensor(this, "TemperatureExt", "External Water Temperature", (x) => ((short)x) / 100.0f));
            _container.FanSensors.Add(new BaseSensor(this, "Flow", "Flow", (x) => ((short)x) / 10.0f));
            _container.FanSensors.Add(new BaseSensor(this, "WaterQuality", "Water Quality", (x) => ((short)x) / 10.0f));
            _container.FanSensors.Add(new BaseSensor(this, "Conductivity", "Conductivity", (x) => ((short)x) / 10.0f));
        }

        private void DataRefreshTimerCallback(object state)
        {
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
            } catch (ApplicationException) { 
            }
        }
    }
}
