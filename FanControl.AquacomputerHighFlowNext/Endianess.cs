using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.AquacomputerHighFlowNext
{
    public enum Endianness
    {
        BigEndian,
        LittleEndian
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EndianAttribute : Attribute
    {
        public Endianness Endianness { get; private set; }

        public EndianAttribute(Endianness endianness)
        {
            this.Endianness = endianness;
        }

        private static void RespectEndianness(Type type, byte[] data, int startOffset = 0)
        {
            var fields = type.GetFields().Where(f => f.IsDefined(typeof(EndianAttribute), false))
                .Select(f => new
                {
                    Field = f,
                    Attribute = (EndianAttribute)f.GetCustomAttributes(typeof(EndianAttribute), false)[0],
                    Offset = Marshal.OffsetOf(type, f.Name).ToInt32()
                }).ToList();

            foreach (var field in fields)
            {
                if ((field.Attribute.Endianness == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
                    (field.Attribute.Endianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian))
                {
                    var fieldType = field.Field.FieldType;
                    if (field.Field.IsStatic)
                        continue;

                    if (fieldType == typeof(string))
                        continue;

                    // handle enums
                    if (fieldType.IsEnum)
                        fieldType = Enum.GetUnderlyingType(fieldType);

                    // check for sub-fields to recurse if necessary
                    var subFields = fieldType.GetFields().Where(subField => subField.IsStatic == false).ToArray();
                    var effectiveOffset = startOffset + field.Offset;

                    if (subFields.Length == 0)
                    {
                        if (fieldType.IsArray)
                        {
                            var arrayFieldType = fieldType.GetElementType();
                            MarshalAsAttribute attr = (MarshalAsAttribute)field.Field.GetCustomAttributes(typeof(MarshalAsAttribute), false)[0];
                            int arraySize = Marshal.SizeOf(arrayFieldType);
                            for (int i = 0; i < attr.SizeConst; i++)
                                Array.Reverse(data, effectiveOffset + i*arraySize, arraySize);
                        }
                        else
                            Array.Reverse(data, effectiveOffset, Marshal.SizeOf(fieldType));
                    }
                    else
                    {
                        // recurse
                        RespectEndianness(fieldType, data, effectiveOffset);
                    }
                }
            }
        }

        public static T BytesToStruct<T>(byte[] rawData) where T : struct
        {
            T result = default(T);

            RespectEndianness(typeof(T), rawData);

            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static byte[] StructToBytes<T>(T data) where T : struct
        {
            byte[] rawData = new byte[Marshal.SizeOf(data)];
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(data, rawDataPtr, false);
            }
            finally
            {
                handle.Free();
            }

            RespectEndianness(typeof(T), rawData);

            return rawData;
        }

        public static T GetStructAtOffset<T>(byte[] rawData, ref int offset) where T : struct
        {
            T result = default;

            int size = Marshal.SizeOf(result);
            byte[] rawDataStruct = new byte[size];
            Array.Copy(rawData, offset, rawDataStruct, 0, size);
            offset += size;

            RespectEndianness(typeof(T), rawDataStruct);

            GCHandle handle = GCHandle.Alloc(rawDataStruct, GCHandleType.Pinned);

            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return result;

        }
    }
}
