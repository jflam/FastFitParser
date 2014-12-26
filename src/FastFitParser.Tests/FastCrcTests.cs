using FastFitParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace FastFitParser.Tests
{
    public static class CrcTestData
    {
        public readonly static string[] SimpleStrings = {
            "123456789",
            "0123456789",
            "01234567890",
            "012345678901",
            "0123456789012",
            "01234567890123",
            "012345678901234",
            "0123456789012345",
            "01234567890123456"
        };

        public readonly static ushort[] SimpleStringsCrcs = {
            0xbb3d,
            0x443d,
            0xc585,
            0x77c5,
            0x8636,
            0x0346,
            0x2583,
            0xb6a4,
            0xad37
        };
    }

    [TestClass]
    public class SlowCrcTests
    {
        [TestMethod]
        public void TestSimpleStrings()
        {
            Assert.AreEqual(CrcTestData.SimpleStrings.Length, CrcTestData.SimpleStringsCrcs.Length);

            for (int i = 0; i < CrcTestData.SimpleStrings.Length; i++)
            {
                int crc = 0;
                byte[] data = System.Text.Encoding.UTF8.GetBytes(CrcTestData.SimpleStrings[i]);
                foreach (byte b in data)
                {
                    crc = CRC.Get16(crc, b);
                }

                Assert.AreEqual(CrcTestData.SimpleStringsCrcs[i], crc, "CRC16 for {0}", CrcTestData.SimpleStrings[i]);
            }
        }
    }

    [TestClass]
    public class FastCrcTests
    {
        [TestMethod]
        public void TestSimpleStrings()
        {
            Assert.AreEqual(CrcTestData.SimpleStrings.Length, CrcTestData.SimpleStringsCrcs.Length);

            for (int i = 0; i < CrcTestData.SimpleStrings.Length; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(CrcTestData.SimpleStrings[i]);
                using (var stream = new MemoryStream(bytes))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        ushort crc = Crc16.ComputeCrc(reader, bytes.Length);
                        Assert.AreEqual(CrcTestData.SimpleStringsCrcs[i], crc, "CRC16 for {0}", CrcTestData.SimpleStrings[i]);
                    }
                }
            }
        }
    }
}