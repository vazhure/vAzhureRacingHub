using System;
using System.Text;

namespace ProjectMotorSports
{
    public class ConversionHelper
    {
        public static void ConvertVec(ref byte[] data, ref int startIdx, out UDPVec3 vec)
        {
            vec = new UDPVec3
            {
                x = BitConverter.ToSingle(data, startIdx)
            };
            startIdx += sizeof(float);

            vec.y = BitConverter.ToSingle(data, startIdx);
            startIdx += sizeof(float);

            vec.z = BitConverter.ToSingle(data, startIdx);
            startIdx += sizeof(float);
        }

        public static void ConvertQuat(ref byte[] data, ref int startIdx, out UDPQuat quat)
        {
            quat = new UDPQuat
            {
                x = BitConverter.ToSingle(data, startIdx)
            };

            startIdx += sizeof(float);

            quat.y = BitConverter.ToSingle(data, startIdx);
            startIdx += sizeof(float);

            quat.z = BitConverter.ToSingle(data, startIdx);
            startIdx += sizeof(float);

            quat.w = BitConverter.ToSingle(data, startIdx);
            startIdx += sizeof(float);
        }

        public static void ConvertFloat(ref byte[] data, ref int startIdx, out float f)
        {
            f = BitConverter.ToSingle(data, startIdx);
            startIdx += sizeof(float);
        }

        public static void ConvertBool(ref byte[] data, ref int startIdx, out bool b)
        {
            byte val = data[startIdx];
            startIdx += sizeof(byte);

            b = val != 0;
        }
        public static void ConvertByte(ref byte[] data, ref int startIdx, out byte b)
        {
            b = data[startIdx];
            startIdx += sizeof(byte);
        }

        public static void ConvertInt32(ref byte[] data, ref int startIdx, out Int32 i)
        {
            i = BitConverter.ToInt32(data, startIdx);
            startIdx += sizeof(Int32);
        }

        public static void ConvertUInt16(ref byte[] data, ref int startIdx, out UInt16 i)
        {
            i = BitConverter.ToUInt16(data, startIdx);
            startIdx += sizeof(UInt16);
        }

        public static void ConvertUInt32(ref byte[] data, ref int startIdx, out UInt32 i)
        {
            i = BitConverter.ToUInt32(data, startIdx);
            startIdx += sizeof(UInt32);
        }

        public static void ConvertString(ref byte[] data, ref int startIdx, out string str)
        {
            ConvertByte(ref data, ref startIdx, out byte len);

            byte[] characters = new byte[len];
            for (int i = 0; i < len; i++)
            {
                ConvertByte(ref data, ref startIdx, out characters[i]);
            }

            str = Encoding.UTF8.GetString(characters);
        }

    }
}