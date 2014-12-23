using System;

namespace FastFitParser.Core
{
    public static class FastCRC
    {
        public static ushort[,] CRC16Table;

        // Architecture-dependent values of [0,1] CRC table
        private const ushort LITTLE_ENDIAN_CRC_01 = 0x1021; // CRC16-CCITT
        //private const ushort LITTLE_ENDIAN_CRC_01 = 0x8005; // CRC16
        private const ushort BIG_ENDIAN_CRC_01 = 0x2110;

        public static ushort ComputeCrc16(ushort crc, byte data)
        {
            if (CRC16Table == null)
            {
                InitializeCrc16LookupTable();
            }

            return 42;
        }

        // TODO: precompute and cache the values in code. There is no point in doing the compute.
        public static void InitializeCrc16LookupTable()
        {
            CRC16Table = new ushort[8, 256];

            for (int i = 0; i < 256; i++)
            {
                ushort crc = 0;
                crc = (ushort)(crc ^ (i << 8));
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ LITTLE_ENDIAN_CRC_01);
                    }
                    else
                    {
                        crc = (ushort)(crc << 1);
                    }
                }

                CRC16Table[0, i] = crc; 
            }

            for (int i = 0; i < 256; i++)
            {
                ushort crc = CRC16Table[0, i];
                for (int k = 1; k < 8; k++)
                {
                    crc = (ushort)(CRC16Table[0, (crc >> 8) & 0xff] ^ (crc << 8));
                    CRC16Table[k, i] = crc;
                }
            }
        }
    }
}