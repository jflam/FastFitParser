using Dynastream.Fit;
using FastFitParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

// This file contains some comparative benchmark tests that are used 
// to measure the improvement that FastFitParser has over the FIT
// SDK parser.

namespace FastFitParser.Tests
{
    public struct FileResult
    {
        public int Records;
        public System.DateTime LatestDateTimeInFile;
    }

    // This class is moved here from the FastFitParser.Core to reduce the code
    // size of the library. 
    public static class CRC
    {
        public static int Get16(int crc, byte data)
        {
            int[] crcTable = new int[]
            {
                0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401,
                0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 
            };

            int tmp;

            // compute checksum of lower four bits of byte 
            tmp = crcTable[crc & 0xF];
            crc = (crc >> 4) & 0x0FFF;
            crc = (crc ^ tmp ^ crcTable[data & 0xF]);

            // compute checksum of upper four bits of byte 
            tmp = crcTable[crc & 0xF];
            crc = (crc >> 4) & 0x0FFF;
            crc = (crc ^ tmp ^ crcTable[(data >> 4) & 0xF]);

            return crc & 0xFFFF;
        }
    }

    [TestClass]
    public class BenchmarkTests
    {
        // TODO: if you want to run this test against your own data, point this at
        // a directory of FIT files that you want to benchmark parsing
        public const string BENCHMARK_DIRECTORY = @"c:\users\john\onedrive\garmin";

        private void TestFramework(Func<Stream, FileResult> func)
        {
            long cumulativeFileSize = 0;
            int recordsParsed = 0;
            var stopWatch = Stopwatch.StartNew();
            System.DateTime maxTime = System.DateTime.MinValue;

            foreach (var file in Directory.GetFiles(BENCHMARK_DIRECTORY, "*.fit"))
            {
                var fi = new FileInfo(file);
                cumulativeFileSize += fi.Length;

                using (var stream = System.IO.File.OpenRead(file))
                {
                    FileResult result = func(stream);
                    recordsParsed += result.Records;
                    if (result.LatestDateTimeInFile > maxTime)
                    {
                        maxTime = result.LatestDateTimeInFile;
                    }
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Parsing performance: {0:F2} MB/s, Record parse rate: {1:F2} million records/s, Most Recent Time = {2}",
                (cumulativeFileSize / 1048576.0) / (stopWatch.ElapsedMilliseconds / 1000.0),
                (recordsParsed / 1000000.0) / (stopWatch.ElapsedMilliseconds / 1000.0),
                maxTime);
        }

        private FileResult TestReadingAllFilesInDirectoryUsingFastParser(Stream stream, bool validateCrc)
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            int recordsParsed = 0;
            var fastParser = new FastParser(stream);
            if (validateCrc)
            {
                Assert.IsTrue(fastParser.IsFileValid());
            }

            foreach (var dataRecord in fastParser.GetDataRecords())
            {
                System.DateTime timeStamp;
                if (dataRecord.GlobalMessageNumber == GlobalMessageDefs.Record)
                {
                    if (dataRecord.TryGetField(RecordDef.TimeStamp, out timeStamp))
                    {
                        // Bogus calculation to make sure we don't optimize this away
                        if (timeStamp > maxTime)
                        {
                            maxTime = timeStamp;
                        }
                    }
                    recordsParsed++;
                }
            }
            return new FileResult { Records = recordsParsed, LatestDateTimeInFile = maxTime };
        }

        [TestMethod]
        public void TestReadingAllFilesInDirectoryUsingFastParserWithCrcValidation()
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            TestFramework((stream) =>
                {
                    return TestReadingAllFilesInDirectoryUsingFastParser(stream, true);
                }
            );
        }

        [TestMethod]
        public void TestReadingAllFilesInDirectoryUsingFastParser()
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            TestFramework((stream) =>
                {
                    return TestReadingAllFilesInDirectoryUsingFastParser(stream, false);
                }
            );
        }

        private FileResult TestReadingAllFilesInDirectoryUsingSdkParser(Stream stream, bool validateCrc)
        {
            int recordCount = 0;
            System.DateTime maxTime = System.DateTime.MinValue;

            var parser = new Decode();
            if (validateCrc)
            {
                Assert.IsTrue(parser.CheckIntegrity(stream));
            }

            parser.MesgEvent += (sender, args) =>
            {
                if (args.mesg.Name == "Record")
                {
                    var recordMesg = new RecordMesg(args.mesg);
                    System.DateTime timeStamp = recordMesg.GetTimestamp().GetDateTime();
                    if (timeStamp > maxTime)
                    {
                        maxTime = timeStamp;
                    }
                    recordCount++;
                }
            };

            Assert.IsTrue(parser.Read(stream));
            return new FileResult { Records = recordCount, LatestDateTimeInFile = maxTime };
        }

        [TestMethod]
        public void TestReadingAllFilesInDirectoryUsingSdkParserWithCrcValidation()
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            TestFramework((stream) =>
                {
                    return TestReadingAllFilesInDirectoryUsingSdkParser(stream, true);
                }
            );
        }

        [TestMethod]
        public void TestReadingAllFilesInDirectoryUsingSdkParser()
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            TestFramework((stream) =>
                {
                    return TestReadingAllFilesInDirectoryUsingSdkParser(stream, false);
                }
            );
        }

        private long TimeIt(int iterations, string path, Action<Stream> func)
        {
            var timer = new Stopwatch();
            long lowestElapsedTime = long.MaxValue;

            for (int i = 0; i < iterations; i++)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    timer.Start();
                    func.Invoke(stream);
                    timer.Stop();
                }

                if (timer.ElapsedMilliseconds < lowestElapsedTime)
                {
                    lowestElapsedTime = timer.ElapsedMilliseconds;
                }
            }

            return lowestElapsedTime;
        }

        [TestMethod]
        public void TestSingleFileParsePerformance()
        {
            int recordsParsed = 0;
            long fastParserTime = TimeIt(5, @"TestData\large_file.fit", (stream) =>
            {
                System.DateTime maxTime = System.DateTime.MinValue;
                var fastParser = new FastParser(stream);
                foreach (var dataRecord in fastParser.GetDataRecords())
                {
                    System.DateTime timeStamp;
                    if (dataRecord.GlobalMessageNumber == GlobalMessageDefs.Record)
                    {
                        if (dataRecord.TryGetField(RecordDef.TimeStamp, out timeStamp))
                        {
                            // Bogus calculation to make sure we don't optimize this away
                            if (timeStamp > maxTime)
                            {
                                maxTime = timeStamp;
                            }
                        }
                        recordsParsed++;
                    }
                }
            });
            Console.WriteLine("Parse large_file.fit best time {0}ms, {1} records parsed", fastParserTime, recordsParsed);
        }

        [TestMethod]
        public void CompareFastCrcWithSlowCrc()
        {
            long slowParserTime = TimeIt(5, @"TestData\large_file.fit", (stream) =>
            {
                int crc = 0;
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < stream.Length - 2; i++)
                    {
                        crc = CRC.Get16(crc, reader.ReadByte());
                    }
                    ushort fileCrc = reader.ReadUInt16();
                    Assert.AreEqual(fileCrc, crc);
                }
            });
            Console.WriteLine("CRC via Dynastream CRC16 implementation = {0}ms", slowParserTime);

            long fastParserTime = TimeIt(5, @"TestData\large_file.fit", (stream) =>
            {
                using (var reader = new BinaryReader(stream))
                {
                    ushort crc = Crc16.ComputeCrc(reader, stream.Length - 2);
                    ushort fileCrc = reader.ReadUInt16();
                    Assert.AreEqual(fileCrc, crc);
                }
            });
            Console.WriteLine("CRC via 64 bit 8x256 CRC16 implementation = {0}ms", fastParserTime);
            Console.WriteLine("64 bit 8x256 CRC16 implementation is {0:0.0}x faster", (double)((double)slowParserTime / (double)fastParserTime));
        }
    }
}