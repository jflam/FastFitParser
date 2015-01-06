using FastFitParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FastFitParser.Tests
{
    // This isn't really a set of tests. Instead, what it does is dump to the console all of the fields in each record type that
    // is encountered while parsing one of the .fit files in TestData.
    // I may choose later to have it assert the field names / types that it finds.

    [TestClass]
    public class GarminRecordsAndFieldsTests
    {
        private void DumpFieldsOfKnownRecord(MessageDecl messageDef, List<FieldDefinition> fieldDefinitions)
        {
            if (messageDef.FieldDefinitions != null)
            {
                foreach (var item in fieldDefinitions)
                {
                    string fieldName = messageDef.FieldDefinitions.FieldNames[item.FieldDefinitionNumber];
                    if (fieldName == null)
                    {
                        fieldName = item.FieldDefinitionNumber.ToString();
                    }
                    Console.WriteLine("    {0}, Type {1}", fieldName, item.FieldType);
                }
            }
            else
            {
                DumpFieldsOfUnknownRecord(fieldDefinitions);
            }
        }

        private void DumpFieldsOfUnknownRecord(List<FieldDefinition> fieldDefinitions)
        {
            foreach (var item in fieldDefinitions)
            {
                Console.WriteLine("    Id {0}, Type {1}", item.FieldDefinitionNumber, item.FieldType);
            }
        }

        [TestMethod]
        public void TestFitFile()
        {
            using (var stream = System.IO.File.OpenRead(@"TestData\large_file.fit"))
            {
                var fastParser = new FastParser(stream);
                var listOfRecordTypesSeen = new List<ushort>();

                foreach (var dataRecord in fastParser.GetDataRecords())
                {
                    ushort messageNumber = dataRecord.GlobalMessageNumber;
                    if (!listOfRecordTypesSeen.Contains(messageNumber))
                    {
                        MessageDecl messageDef;
                        if (GlobalMessageDecls.Declarations.TryGetValue(messageNumber, out messageDef))
                        {
                            Console.WriteLine("Record type: {0}", GlobalMessageDecls.Declarations[messageNumber].MessageName);
                            DumpFieldsOfKnownRecord(messageDef, dataRecord.RecordDefinition.FieldDefinitions);
                        }
                        else
                        {
                            Console.WriteLine("Record type: {0}", messageNumber);
                            DumpFieldsOfUnknownRecord(dataRecord.RecordDefinition.FieldDefinitions);
                        }
                        foreach (var field in dataRecord.RecordDefinition.FieldDefinitions)
                        {
                            
                        }
                        listOfRecordTypesSeen.Add(messageNumber);
                    }
                }
            }
        }
    }
}
