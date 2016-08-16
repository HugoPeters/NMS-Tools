using System;

namespace System.IO
{
    public static class Extensions
    {

        public static float ReadHalfLittle(this BinaryReader binaryReader)
        {
            UInt16 u = binaryReader.ReadUInt16();
            int sign = (u >> 15) & 0x00000001;
            int exp = (u >> 10) & 0x0000001F;
            int mant = u & 0x000003FF;

            exp = exp + (127 - 15);

            int i = (sign << 31) | (exp << 23) | (mant << 13);
            byte[] buff = BitConverter.GetBytes(i);

            return BitConverter.ToSingle(buff, 0);
        }

        public static float ToFloat(UInt16 u)
        {
            // UInt16 u = binaryReader.ReadUInt16();
            int sign = (u >> 15) & 0x00000001;
            int exp = (u >> 10) & 0x0000001F;
            int mant = u & 0x000003FF;

            exp = exp + (127 - 15);

            int i = (sign << 31) | (exp << 23) | (mant << 13);
            byte[] buff = BitConverter.GetBytes(i);

            return BitConverter.ToSingle(buff, 0);
        }

        public static UInt16 ToHalf(this float f)
        {
            byte[] bytes = BitConverter.GetBytes((double)f);
            ulong bits = BitConverter.ToUInt64(bytes, 0);
            ulong exponent = bits & 0x7ff0000000000000L;
            ulong mantissa = bits & 0x000fffffffffffffL;
            ulong sign = bits & 0x8000000000000000L;
            int placement = (int)((exponent >> 52) - 1023);
            if (placement > 15 || placement < -14)
                return ToHalf(-1.0f);

            UInt16 exponentBits = (UInt16)((15 + placement) << 10);
            UInt16 mantissaBits = (UInt16)(mantissa >> 42);
            UInt16 signBits = (UInt16)(sign >> 48);
            return (UInt16)(exponentBits | mantissaBits | signBits);

        }
    }
}