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
                foreach (var dataRecord in fastParser.GetDataRecords())
                {
                    if (dataRecord.GlobalMessageNumber == GlobalMessageNumber.Record)
                    {
                        var record = new DataSeriesRecord();

                        double latitude, longitude, cadence, heartRate, power, distance, speed;
                        System.DateTime timeStamp;

                        if (dataRecord.TryGetField(FieldNumber.TimeStamp, out timeStamp))
                        {
                            record.TimeStamp = timeStamp;
                        }
                        if (dataRecord.TryGetField(FieldNumber.PositionLat, out latitude))
                        {
                            record.Latitude = latitude * SEMICIRCLES_TO_DEGREES;
                        }
                        if (dataRecord.TryGetField(FieldNumber.PositionLong, out longitude))
                        {
                            record.Longitude = longitude * SEMICIRCLES_TO_DEGREES;
                        }
                        if (dataRecord.TryGetField(FieldNumber.HeartRate, out heartRate))
                        {
                            record.HeartRate = heartRate; // beats * min-1
                        }
                        if (dataRecord.TryGetField(FieldNumber.Cadence, out cadence))
                        {
                            record.Cadence = cadence; // s-1
                        }
                        if (dataRecord.TryGetField(FieldNumber.Power, out power))
                        {
                            record.Power = power; // W
                        }
                        if (dataRecord.TryGetField(FieldNumber.Distance, out distance))
                        {
                            record.Distance = distance / 1000; // m
                        }
                        if (dataRecord.TryGetField(FieldNumber.Speed, out speed))
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

                foreach (var dataRecord in fastParser.GetDataRecords())
                {
                    if (dataRecord.GlobalMessageNumber == GlobalMessageNumber.Record)
                    {
                        double heartRate;
                        if (dataRecord.TryGetField(FieldNumber.HeartRate, out heartRate))
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

                foreach (var dataRecord in fastParser.GetDataRecords())
                {
                    if (dataRecord.GlobalMessageNumber == GlobalMessageNumber.Record)
                    {
                        if (dataRecord.TryGetField(FieldNumber.HeartRate, out currentHeartRate))
                        {
                            heartRate += currentHeartRate;
                        }
                        if (dataRecord.TryGetField(FieldNumber.Cadence, out currentCadence))
                        {
                            cadence += currentCadence;
                        }
                        if (dataRecord.TryGetField(FieldNumber.Power, out currentPower))
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

    [TestClass]
    public class RandomBehaviorTests
    {
        [TestMethod]
        public void TestEnumBehavior()
        {
            GlobalMessageNumber num1 = (GlobalMessageNumber)1; // exists
            GlobalMessageNumber num2 = (GlobalMessageNumber)146; // doesn't exist
            var value1 = Enum.GetName(typeof(GlobalMessageNumber), num1);
            var value2 = Enum.GetName(typeof(GlobalMessageNumber), num2);
            Assert.AreEqual(value1, "Capabilities");
            Assert.IsNull(value2);
            Assert.IsFalse(Enum.IsDefined(typeof(GlobalMessageNumber), num2));
        }
    }
}