using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.AquacomputerHighFlowNext
{
    static class HighFlowNextSensorsStructs
    {

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct HighFlowNext_Header 
		{
			public byte IsValid; // If it's != 0

			[MarshalAs(UnmanagedType.U2)]
			[Endian(Endianness.BigEndian)]
			public ushort Version;

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint Serial;

			[MarshalAs(UnmanagedType.U2)]
			[Endian(Endianness.BigEndian)]
			public ushort Hardware;

			[MarshalAs(UnmanagedType.U2)]
			[Endian(Endianness.BigEndian)]
			public ushort DeviceType;

			[MarshalAs(UnmanagedType.U2)]
			[Endian(Endianness.BigEndian)]
			public ushort Bootloader;

			[MarshalAs(UnmanagedType.U2)]
			[Endian(Endianness.BigEndian)]
			public ushort Firmware;

			public static string SerialToText(uint sn)
			{
				return ((sn & 0xFFFF0000L) >> 16).ToString("D5") + "-" + (sn & 0xFFFFL).ToString("D5");
			}
		}

		// 18 byte
		// SE FIRMWARE = 1012:
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct HighFlowNext_Firmware1012
		{
			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint SystemState;

			public byte Features; 

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint Time;

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint PowerCycles;

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint RuntimeTotal;

			[MarshalAs(UnmanagedType.U2)]
			[Endian(Endianness.BigEndian)]
			public ushort _Unknown00;

			public byte Language; // ==0 english, !=0 german
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct HighFlowNext_SensorData
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			[Endian(Endianness.BigEndian)]
			public ushort[] _Unknown00;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			[Endian(Endianness.BigEndian)]
			public short[] _Unknown01; // lista sensori?

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] _Unknown02;

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint FlowRaw;

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short FlowCalibration;

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short _Unknown03;

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short SensorDiff;

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short FlowWithoutUserCompensation;

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short Flow; // (divide by 10.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short TemperatureWater; // (divide by 100.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short TemperatureExt; // (divide by 100.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short WaterQuality; // (divide by 100.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short Power;

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short ConductivityUncompensate; // (divide by 10.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short Conductivity; // (divide by 10.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short Vcc5; // (divide by 100.0)

			[MarshalAs(UnmanagedType.I2)]
			[Endian(Endianness.BigEndian)]
			public short Vcc5usb; // (divide by 100.0)

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint Volume; 

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint ImpulseCounter; 

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint CounterTime; // TimeSpan.FromSeconds()

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint _Unknown04; // Maybe Alarams?

			[MarshalAs(UnmanagedType.U4)]
			[Endian(Endianness.BigEndian)]
			public uint _Unknown05; // Maybe Alarams?

			public byte CurrentProfile;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			[Endian(Endianness.BigEndian)]
			public short[] _Unknown06;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			[Endian(Endianness.BigEndian)]
			public short[] _Unknown07;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public byte[] _Unknown08;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			[Endian(Endianness.BigEndian)]
			public short[] _Unknown09;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			[Endian(Endianness.BigEndian)]
			public short[] _Unknown10;
		}
	}

	public class BaseSensor : Plugins.IPluginSensor
	{
		protected System.Reflection.FieldInfo _field;
        private string _description;
        private readonly string _fieldName;
        private readonly Func<object, float?> _lambda;
		private readonly AquacomputerHFNPlugin _parent;
		float? _value = null;

		internal BaseSensor(AquacomputerHFNPlugin parent, string fieldName, string description, Func<object, float?> lambda)
		{
			_fieldName = fieldName;
			_description = description;
			_lambda = lambda;
			_parent = parent;
			_field = typeof(HighFlowNextSensorsStructs.HighFlowNext_SensorData).GetField(fieldName);
		}

		public string Id => _fieldName;
		public string Name => _description;
		public float? Value => _value;

        public void Update()
		{
			try
			{
				_parent.rwl.AcquireReaderLock(100);
				try
				{
					_value = _lambda(_field.GetValue(_parent.data));
				}
				finally
				{
					_parent.rwl.ReleaseReaderLock();
				}
			}
			catch (ApplicationException) { }
		}
    }
}
