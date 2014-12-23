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
    [TestClass]
    public class BenchmarkTests
    {
        // TODO: if you want to run this test against your own data, point this at
        // a directory of FIT files that you want to benchmark parsing
        public const string BENCHMARK_DIRECTORY = @"c:\users\john\onedrive\garmin";

        private void TestFramework(Func<Stream, int> func)
        {
            long cumulativeFileSize = 0;
            int recordsParsed = 0;
            var stopWatch = Stopwatch.StartNew();

            foreach (var file in Directory.GetFiles(BENCHMARK_DIRECTORY, "*.fit"))
            {
                var fi = new FileInfo(file);
                cumulativeFileSize += fi.Length;

                using (var stream = System.IO.File.OpenRead(file))
                {
                    recordsParsed += func(stream);
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Parsing performance: {0:F2} MB/s, Record parse rate: {1:F2} million records/s",
                (cumulativeFileSize / 1048576.0) / (stopWatch.ElapsedMilliseconds / 1000.0),
                (recordsParsed / 1000000.0) / (stopWatch.ElapsedMilliseconds / 1000.0));
        }

        [TestMethod]
        public void TestReadingAllFilesInDirectoryUsingFastParser()
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            TestFramework((stream) =>
                {
                    int recordsParsed = 0;
                    var fastParser = new FastParser(stream);
                    foreach (var dataRecord in fastParser.GetDataRecords())
                    {
                        System.DateTime timeStamp;
                        if (dataRecord.GlobalMessageNumber == GlobalMessageNumber.Record)
                        {
                            if (dataRecord.TryGetField(FieldNumber.TimeStamp, out timeStamp))
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
                    return recordsParsed;
                }
            );
            Console.WriteLine("Most recent time = {0}", maxTime);
        }

        [TestMethod]
        public void TestReadingAllFilesInDirectoryUsingSdkParser()
        {
            System.DateTime maxTime = System.DateTime.MinValue;
            TestFramework((stream) =>
                {
                    int recordsParsed = 0;
                    var parser = new Decode();

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
                            recordsParsed++;
                        }
                    };

                    Assert.IsTrue(parser.Read(stream));
                    return recordsParsed;
                }
            );
            Console.WriteLine("Most recent time = {0}", maxTime);
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
                    if (dataRecord.GlobalMessageNumber == GlobalMessageNumber.Record)
                    {
                        if (dataRecord.TryGetField(FieldNumber.TimeStamp, out timeStamp))
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
        public void TestCrcPerformance()
        {
            long fastParserTime = TimeIt(5, @"TestData\large_file.fit", (stream) =>
            {
                var fastParser = new FastParser(stream);
                Assert.IsTrue(fastParser.IsFileValid());
            });
            Console.WriteLine("CRC via BinaryReader best time {0}", fastParserTime);
        }
    }
}