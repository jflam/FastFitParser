using FastFitParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FastFitParser.Tests
{
    [TestClass]
    public class FastParserTests
    {
        public readonly double SEMICIRCLES_TO_DEGREES = (180 / Math.Pow(2, 31));

        [TestMethod]
        public void TestReadLargeFileIntoInternalDataStructures()
        {
            using (var stream = System.IO.File.OpenRead(@"TestData\large_file.fit"))
            {
                var fastParser = new FastParser(stream);

                var records = new List<DataSeriesRecord>();
                foreach (var dataRecord in fastParser.GetMessages())
                {
                    if (dataRecord.GlobalMessageNumber == GlobalMessageDecls.Record)
                    {
                        var record = new DataSeriesRecord();

                        double latitude, longitude, cadence, heartRate, power, distance, speed;
                        System.DateTime timeStamp;

                        if (dataRecord.TryGetField(RecordDef.TimeStamp, out timeStamp))
                        {
                            record.TimeStamp = timeStamp;
                        }
                        if (dataRecord.TryGetField(RecordDef.PositionLat, out latitude))
                        {
                            record.Latitude = latitude * SEMICIRCLES_TO_DEGREES;
                        }
                        if (dataRecord.TryGetField(RecordDef.PositionLong, out longitude))
                        {
                            record.Longitude = longitude * SEMICIRCLES_TO_DEGREES;
                        }
                        if (dataRecord.TryGetField(RecordDef.HeartRate, out heartRate))
                        {
                            record.HeartRate = heartRate; // beats * min-1
                        }
                        if (dataRecord.TryGetField(RecordDef.Cadence, out cadence))
                        {
                            record.Cadence = cadence; // s-1
                        }
                        if (dataRecord.TryGetField(RecordDef.Power, out power))
                        {
                            record.Power = power; // W
                        }
                        if (dataRecord.TryGetField(RecordDef.Distance, out distance))
                        {
                            record.Distance = distance / 1000; // m
                        }
                        if (dataRecord.TryGetField(RecordDef.Speed, out speed))
                        {
                            record.Speed = speed / 1000; // m/s
                        }

                        records.Add(record);
                    }
                }
                Console.WriteLine("Read {0} timestamp values", records.Count);
            }
        }

        [TestMethod]
        public void ReadFitFileWithoutHrData()
        {
            using (var stream = System.IO.File.OpenRead(@"TestData\no_hr_data.fit"))
            {
                var fastParser = new FastParser(stream);
                Assert.IsTrue(fastParser.IsFileValid());

                foreach (var dataRecord in fastParser.GetMessages())
                {
                    if (dataRecord.GlobalMessageNumber == GlobalMessageDecls.Record)
                    {
                        double heartRate;
                        if (dataRecord.TryGetField(RecordDef.HeartRate, out heartRate))
                        {
                            Assert.IsTrue(false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestReadLargeFile()
        {
            using (var stream = System.IO.File.OpenRead(@"TestData\large_file.fit"))
            {
                var fastParser = new FastParser(stream);

                var recordCount = 0;
                double heartRate = 0, currentHeartRate;
                double cadence = 0, currentCadence;
                double power = 0, currentPower;

                foreach (var dataRecord in fastParser.GetMessages())
                {
                    if (dataRecord.GlobalMessageNumber == GlobalMessageDecls.Record)
                    {
                        if (dataRecord.TryGetField(RecordDef.HeartRate, out currentHeartRate))
                        {
                            heartRate += currentHeartRate;
                        }
                        if (dataRecord.TryGetField(RecordDef.Cadence, out currentCadence))
                        {
                            cadence += currentCadence;
                        }
                        if (dataRecord.TryGetField(RecordDef.Power, out currentPower))
                        {
                            power += currentPower;
                        }
                        recordCount++;
                    }
                }
                Console.WriteLine("Read {0} records, average HR = {1}, average Cadence = {2}, average Power = {3}", recordCount,
                    heartRate / (double)recordCount,
                    cadence / (double)recordCount,
                    power / (double)recordCount);
            }
        }
    }
}