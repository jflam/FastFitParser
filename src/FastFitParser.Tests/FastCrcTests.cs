using FastFitParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FastFitParser.Tests
{
    [TestClass]
    public class SlowCrcTests
    {
        [TestMethod]
        public void TestSimpleString1()
        {
            string source = "123456789";
            ushort expectedCRC = 0xBB3D;

            int crc = 0;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            foreach (byte b in data)
            {
                crc = CRC.Get16(crc, b);
            }

            Assert.AreEqual(expectedCRC, crc);
        }
    }

    [TestClass]
    public class FastCrcTests
    {
        [TestMethod]
        public void TestSimpleString1()
        {
            string source = "123456789";
            ushort expectedCRC = 0xBB3D; // CRC16
            ushort expectedCRCCCITT = 0x31C3; // CRC-CCITT

            FastCRC.InitializeCrc16LookupTable();

            ushort crc = 0;
            ushort[,] table = FastCRC.CRC16Table;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            using (var stream = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ulong chunk = reader.ReadUInt64();
                    crc = (ushort)(table[7, (int)(chunk & 0xff) ^ ((crc >> 8) & 0xff)] ^ 
                                   table[6, (int)((chunk >> 8) & 0xff) ^ (crc & 0xff)] ^
                                   table[5, (chunk >> 16) & 0xff] ^
                                   table[4, (chunk >> 24) & 0xff] ^
                                   table[3, (chunk >> 32) & 0xff] ^
                                   table[2, (chunk >> 40) & 0xff] ^
                                   table[1, (chunk >> 48) & 0xff] ^
                                   table[0, (chunk >> 56)]);

                    // process remaining bytes
                    byte next = reader.ReadByte();
                    crc = (ushort)(table[0, ((crc >> 8) ^ next) & 0xff] ^ (crc << 8));

                    Assert.AreEqual(expectedCRC, crc);
                }
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSimpleString2()
        {
            string source = "123456789";
            ushort expectedCRC = 0xBB3D; // CRC16

            FastCRC.InitializeCrc16();

            ushort crc = 0;
            ushort[,] table = FastCRC.CRC16Table;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            using (var stream = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ulong n = reader.ReadUInt64();
                    crc = (ushort)(table[7, (int)(n & 0xff) ^ ((crc >> 8) & 0xff)] ^
                                   table[6, (int)((n >> 8) & 0xff) ^ (crc & 0xff)] ^
                                   table[5, (n >> 16) & 0xff] ^
                                   table[4, (n >> 24) & 0xff] ^
                                   table[3, (n >> 32) & 0xff] ^
                                   table[2, (n >> 40) & 0xff] ^
                                   table[1, (n >> 48) & 0xff] ^
                                   table[0, (n >> 56)]);

                    // process remaining bytes
                    for (int i = 0; i < 1; i++)
                    {
                        byte next = reader.ReadByte();
                        crc = (ushort)(table[0, (crc ^ next) & 0xff] ^ (crc >> 8));
                    }

                    Assert.AreEqual(expectedCRC, crc);
                }
            }

            Assert.IsTrue(true);
        }
    }
}