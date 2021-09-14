//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021. All Rights reserved.
//*************************************************************************************************

using System;

namespace ColorfulSoft.DeOldify
{

    ///<summary>Half to Float conversion.</summary>
    internal static class HalfHelper
    {

        private static uint[] mantissaTable = GenerateMantissaTable();

        private static uint[] exponentTable = GenerateExponentTable();

        private static ushort[] offsetTable = GenerateOffsetTable();

        ///<summary> Transforms the subnormal representation to a normalized one. </summary>
        private static uint ConvertMantissa(int i)
        {
            uint m = (uint)(i << 13);
            uint e = 0;
            while((m & 0x00800000) == 0)
            {
                e -= 0x00800000;
                m <<= 1;
            }
            m &= unchecked((uint)~0x00800000);
            e += 0x38800000;
            return m | e;
        }

        private static uint[] GenerateMantissaTable()
        {
            uint[] mantissaTable = new uint[2048];
            mantissaTable[0] = 0;
            for(int i = 1; i < 1024; i++)
            {
                mantissaTable[i] = ConvertMantissa(i);
            }
            for(int i = 1024; i < 2048; i++)
            {
                mantissaTable[i] = (uint)(0x38000000 + ((i - 1024) << 13));
            }
            return mantissaTable;
        }

        private static uint[] GenerateExponentTable()
        {
            uint[] exponentTable = new uint[64];
            exponentTable[0] = 0;
            for(int i = 1; i < 31; i++)
            {
                exponentTable[i] = (uint)(i << 23);
            }
            exponentTable[31] = 0x47800000;
            exponentTable[32] = 0x80000000;
            for(int i = 33; i < 63; i++)
            {
                exponentTable[i] = (uint)(0x80000000 + ((i - 32) << 23));
            }
            exponentTable[63] = 0xc7800000;
            return exponentTable;
        }

        private static ushort[] GenerateOffsetTable()
        {
            ushort[] offsetTable = new ushort[64];
            offsetTable[0] = 0;
            for(int i = 1; i < 32; i++)
            {
                offsetTable[i] = 1024;
            }
            offsetTable[32] = 0;
            for(int i = 33; i < 64; i++)
            {
                offsetTable[i] = 1024;
            }
            return offsetTable;
        }

        public static unsafe float HalfToSingle(ushort half)
        {
            uint result = mantissaTable[offsetTable[half >> 10] + (half & 0x3ff)] + exponentTable[half >> 10];
            return *((float*)&result);
        }

    }

}