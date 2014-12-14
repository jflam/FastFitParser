using Dynastream.Fit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

// This file contains tests for the FIT SDK parser, which I'm treating as the authoritative parser
// as I build out the functionality in FastFitParser. These are a set of unit tests to validate that
// the FIT SDK parser is functioning correctly.

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

    [TestClass]
    public class FitParserTests
    {
        private string FormatFieldString(int recordNumber, FitCsvRecord csvRecord, List<Field> fitRecord)
        {
            var sb = new StringBuilder();
            sb.Append("\n\nRecord Offset in FIT file: " + recordNumber);
            sb.Append("\nCSV Record:\n");
            foreach (var csvField in csvRecord.Fields)
            {
                sb.Append(String.Format("{0}={1} {2}\n", csvField.Name, csvField.Value, csvField.Units));
            }
            sb.Append("\nFIT Record:\n");
            foreach (var fitField in fitRecord)
            {
                sb.Append(String.Format("{0}={1} {2}\n", FitParserHelpers.ConvertPascalCaseToRubyCase(fitField.Name), ConvertField(fitField.GetValue()), fitField.Units));
            }
            return sb.ToString();
        }

        private object ConvertField(object field)
        {
            byte[] bytes = field as byte[];
            if (bytes != null)
            {
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(Convert.ToChar(b));
                }
                return sb;
            }
            else
            {
                return field;
            }
        }

        const string TESTDATA_PATH = @"TestData\";

        private void TestActivityFitFile(string filename, int expectedTotalMessages, int expectedTotalDefinitions)
        {
            // Read the answer CSV file
            var file = FitParserHelpers.ReadFitCsvFile(TESTDATA_PATH + filename + ".csv");

            // Decode a Fit file
            var fileStream = System.IO.File.OpenRead(TESTDATA_PATH + filename + ".fit");
            Assert.IsNotNull(fileStream);

            var decode = new Decode();
            Assert.IsNotNull(decode);

            var mesgBroadcaster = new MesgBroadcaster();
            Assert.IsNotNull(mesgBroadcaster);

            Assert.IsTrue(decode.IsFIT(fileStream));

            // Note that it is possible to have something that fails an integrity check, but we can still attempt to parse it
            Assert.IsTrue(decode.CheckIntegrity(fileStream));

            // Connect the Broadcaster to our event (message) source (in this case the Decoder)
            decode.MesgEvent += mesgBroadcaster.OnMesg;
            decode.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;

            int totalMessages = 0, totalDefinitions = 0;
            int currentRecord = 0;

            mesgBroadcaster.MesgEvent += (sender, args) =>
            {
                foreach (var field in args.mesg.fields)
                {
                    string fieldName = FitParserHelpers.ConvertPascalCaseToRubyCase(field.Name);
                    var record = file.Records[currentRecord];
                    string fieldValue = ConvertField(field.GetValue()).ToString();
                    var csvField = record.GetField(fieldName);
                    string diagnosticFieldString = FormatFieldString(currentRecord, record, args.mesg.fields);
                    Assert.IsNotNull(csvField, diagnosticFieldString);
                    Assert.AreEqual(csvField.Value.ToString(), fieldValue, diagnosticFieldString);
                }
                totalMessages++;
                currentRecord++;
            };

            mesgBroadcaster.MesgDefinitionEvent += (sender, args) =>
            {
                foreach (var field in args.mesgDef.GetFields())
                {
                    var record = file.Records[currentRecord];
                }
                totalDefinitions++;
                currentRecord++;
            };

            Assert.IsTrue(decode.Read(fileStream));
            Assert.AreEqual(expectedTotalMessages, totalMessages);
            Assert.AreEqual(expectedTotalDefinitions, totalDefinitions);

            fileStream.Close();
        }

        [TestMethod]
        public void TestActivityFile()
        {
            TestActivityFitFile("Activity", 22, 10);
        }

        [TestMethod]
        public void TestSettingsFile()
        {
            TestActivityFitFile("Settings", 3, 3);
        }

        [TestMethod]
        public void TestWeightScaleMultiUserFile()
        {
            TestActivityFitFile("WeightScaleMultiUser", 7, 4);
        }

        [TestMethod]
        public void TestWeightScaleSingleUserFile()
        {
            TestActivityFitFile("WeightScaleSingleUser", 6, 8);
        }

        [TestMethod]
        public void TestWorkoutCustomTargetValuesFile()
        {
            TestActivityFitFile("WorkoutCustomTargetValues", 6, 3);
        }

        [TestMethod]
        public void TestWorkoutIndividualStepsFile()
        {
            TestActivityFitFile("WorkoutIndividualSteps", 6, 3);
        }

        [TestMethod]
        public void TestWorkoutRepeatGreaterThanStep()
        {
            TestActivityFitFile("WorkoutRepeatGreaterThanStep", 7, 3);
        }

        [TestMethod]
        public void TestWorkoutRepeatSteps()
        {
            TestActivityFitFile("WorkoutRepeatSteps", 7, 3);
        }
    }
}