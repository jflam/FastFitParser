using FastFitParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CyclingAnalytics.Core.Tests
{
    public class FitCsvField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Units { get; set; }
    }

    public class FitCsvRecord
    {
        private List<FitCsvField> _fields = new List<FitCsvField>();

        public string Type { get; set; }
        public int LocalNumber { get; set; }
        public string Message { get; set; }
        public List<FitCsvField> Fields
        {
            get { return _fields; }
        }

        public FitCsvField AddField()
        {
            var result = new FitCsvField();
            _fields.Add(result);
            return result;
        }

        public FitCsvField GetField(string name)
        {
            foreach (var field in Fields)
            {
                if (field.Name == name)
                {
                    return field;
                }
            }
            return null;
        }
    }

    public class FitCsvFile
    {
        private List<FitCsvRecord> _records = new List<FitCsvRecord>();

        public FitCsvRecord AddRecord()
        {
            var result = new FitCsvRecord();
            _records.Add(result);
            return result;
        }

        public List<FitCsvRecord> Records
        {
            get { return _records; }
        }
    }

    public class FitCsvFileParser
    {
        private StreamReader _reader;
        private FitCsvFile _file = new FitCsvFile();

        private void ReadCsvHeader()
        {
            _reader.ReadLine();
        }

        private string ReadLine()
        {
            // Trim the final , from the line if it exists
            var line = _reader.ReadLine();
            if (line[line.Length - 1] == ',')
            {
                return line.Substring(0, line.Length - 1);
            }
            else
            {
                return line;
            }
        }

        private void ReadCsvBody()
        {
            while (!_reader.EndOfStream)
            {
                var line = ReadLine();
                var fields = line.Split(',');
                var record = _file.AddRecord();

                Debug.Assert(fields.Length >= 3);
                record.Type = fields[0];
                record.LocalNumber = Convert.ToInt32(fields[1]);
                record.Message = fields[2];

                Debug.Assert((fields.Length - 3) % 3 == 0);
                for (int i = 3; i < fields.Length; i += 3)
                {
                    var field = record.AddField();
                    field.Name = fields[i];
                    field.Value = fields[i + 1];
                    field.Units = fields[i + 2];
                }
            }
        }

        public FitCsvFileParser(StreamReader reader)
        {
            _reader = reader;
            ReadCsvHeader();
            ReadCsvBody();
        }

        public FitCsvFile File
        {
            get { return _file; }
        }
    }

    public class FitParserHelpers
    {
        public static FitCsvFile ReadFitCsvFile(string path)
        {
            using (var stream = System.IO.File.OpenRead(path))
            {
                using (var reader = new StreamReader(stream))
                {
                    var parser = new FitCsvFileParser(reader);
                    return parser.File;
                }
            }
        }

        public static string ConvertPascalCaseToRubyCase(string input)
        {
            var sb = new StringBuilder();
            int index = 0;
            foreach (var c in input)
            {
                if (index == 0)
                {
                    sb.Append(Char.ToLower(c));
                }
                else
                {
                    if (Char.IsUpper(c))
                    {
                        sb.Append('_');
                        sb.Append(Char.ToLower(c));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                index++;
            }
            return sb.ToString();
        }
    }

    public class DataSeriesRecord
    {
        public System.DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double HeartRate { get; set; }
        public double Cadence { get; set; }
        public double Power { get; set; }
        public double Distance { get; set; }
        public double Speed { get; set; }
    }
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
    public class BenchmarkTests
    {
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
                                if (timeStamp > maxTime)
                                {
                                    maxTime = timeStamp; // Bogus calculation to make sure we don't optimize this away
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
    }

    [TestClass]
    public class FitParserHelperTests
    {
        [TestMethod]
        public void TestPascalCaseToRubyCaseConverter()
        {
            Assert.AreEqual("foo", FitParserHelpers.ConvertPascalCaseToRubyCase("foo"));
            Assert.AreEqual("foo", FitParserHelpers.ConvertPascalCaseToRubyCase("Foo"));
            Assert.AreEqual("foo_bar", FitParserHelpers.ConvertPascalCaseToRubyCase("fooBar"));
            Assert.AreEqual("foo_bar", FitParserHelpers.ConvertPascalCaseToRubyCase("FooBar"));
            Assert.AreEqual("foo_b_b", FitParserHelpers.ConvertPascalCaseToRubyCase("FooBB"));
            Assert.AreEqual("b_b", FitParserHelpers.ConvertPascalCaseToRubyCase("BB"));
            Assert.AreEqual("b", FitParserHelpers.ConvertPascalCaseToRubyCase("B"));
            Assert.AreEqual("b", FitParserHelpers.ConvertPascalCaseToRubyCase("b"));
        }

        [TestMethod]
        public void TestOpenFitActivityCsvFile()
        {
            var file = FitParserHelpers.ReadFitCsvFile(@"TestData\Activity.csv");
            var records = file.Records;
            Assert.AreEqual(32, records.Count);
            var record1 = file.Records[0];
            Assert.AreEqual("Definition", record1.Type);
            Assert.AreEqual(0, record1.LocalNumber);
            Assert.AreEqual("file_id", record1.Message);
            var field1 = record1.Fields[0];
            Assert.AreEqual("serial_number", field1.Name);
            Assert.AreEqual("1", field1.Value);
            Assert.AreEqual(String.Empty, field1.Units);
            var record32 = file.Records[31];
            Assert.AreEqual("Data", record32.Type);
            Assert.AreEqual(5, record32.LocalNumber);
            Assert.AreEqual("activity", record32.Message);
            var field2 = record32.Fields[1];
            Assert.AreEqual("total_timer_time", field2.Name);
            Assert.AreEqual("13.749", field2.Value);
            Assert.AreEqual("s", field2.Units);
        }
    }
}