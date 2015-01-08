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
        private void DumpFieldsOfKnownRecord(MessageDecl messageDecl, List<FieldDefinition> fieldDefinitions)
        {
            if (messageDecl.FieldDeclarations != null)
            {
                foreach (var item in fieldDefinitions)
                {
                    bool isArray = false;
                    bool isEnum = false;
                    string fieldName = String.Empty;

                    var fieldDecls = messageDecl.FieldDeclarations.Declarations[item.FieldDefinitionNumber];

                    if (fieldDecls == null)
                    {
                        fieldName = item.FieldDefinitionNumber.ToString();
                    }
                    else
                    {
                        if (fieldDecls.Count == 1)
                        {
                            if (fieldDecls.First.Value.IsArray)
                            {
                                isArray = true;
                            }
                            if (fieldDecls.First.Value.IsEnum)
                            {
                                isEnum = true;
                            }
                            fieldName = fieldDecls.First.Value.FieldName;
                        }
                        else
                        {
                            foreach (var fieldDecl in fieldDecls)
                            {
                                fieldName += fieldDecl.FieldName + "|";
                            }
                        }
                    }
                    Console.WriteLine("    {0}, Type {1}, IsEnum? {2} IsArray? {3}", fieldName, item.FieldType, isEnum, isArray);
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

                foreach (var dataRecord in fastParser.GetMessages())
                {
                    ushort messageNumber = dataRecord.GlobalMessageNumber;
                    if (!listOfRecordTypesSeen.Contains(messageNumber))
                    {
                        MessageDecl messageDef;
                        if (GlobalMessageDecls.Declarations.TryGetValue(messageNumber, out messageDef))
                        {
                            Console.WriteLine("Record type: {0}", GlobalMessageDecls.Declarations[messageNumber].MessageName);
                            DumpFieldsOfKnownRecord(messageDef, dataRecord.MessageDefinition.FieldDefinitions);
                        }
                        else
                        {
                            Console.WriteLine("Record type: {0}", messageNumber);
                            DumpFieldsOfUnknownRecord(dataRecord.MessageDefinition.FieldDefinitions);
                        }
                        foreach (var field in dataRecord.MessageDefinition.FieldDefinitions)
                        {
                            
                        }
                        listOfRecordTypesSeen.Add(messageNumber);
                    }
                }
            }
        }
    }
}
