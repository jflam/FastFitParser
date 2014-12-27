// High performance FIT parser

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FastFitParser.Core
{
    #region Definitions

    // Well-known field numbers are used to retrieve things like heart rate and power. Note
    // that field numbers are _record relative_, that is they are only unique for a given
    // record type; they are not globally unique.
    public enum FieldNumber : byte
    {
        PositionLat = 0,
        PositionLong = 1,
        Altitude = 2,
        HeartRate = 3,
        Cadence = 4,
        Distance = 5,
        Speed = 6,
        Power = 7,
        TimeStamp = 253,
    }

    // Global message numbers identify unique message types in the .FIT file format. AFAIK, all records
    // are dynamically defined within the .FIT file; that is the schema for each message type is encoded
    // within the file itself. These identifiers are only for well-known message types to help the parser
    // categorize the data itself.
    public enum GlobalMessageNumber : ushort
    {
        FileId = 0,
        Capabilities = 1,
        DeviceSettings = 2,
        UserProfile = 3,
        HrmProfile = 4,
        SdmProfile = 5,
        BikeProfile = 6,
        ZonesTarget = 7,
        HrZone = 8,
        PowerZone = 9,
        MetZone = 10,
        Sport = 12,
        Goal = 15,
        Session = 18,
        Lap = 19,
        Record = 20,
        Event = 21,
        DeviceInfo = 23,
        Workout = 26,
        WorkoutStep = 27,
        Schedule = 28,
        WeightScale = 30,
        Course = 31,
        CoursePoint = 32,
        Totals = 33,
        Activity = 34,
        Software = 35,
        FileCapabilities = 37,
        MesgCapabilities = 38,
        FieldCapabilities = 39,
        FileCreator = 49,
        BloodPressure = 51,
        SpeedZone = 53,
        Monitoring = 55,
        Hrv = 78,
        Length = 101,
        MonitoringInfo = 103,
        Pad = 105,
        SlaveDevice = 106,
        CadenceZone = 131,
        MemoGlob = 145,
        MfgRangeMin = 0xFF00, // 0xFF00 - 0xFFFE reserved for manufacturer specific messages
        MfgRangeMax = 0xFFFE, // 0xFF00 - 0xFFFE reserved for manufacturer specific messages
        Invalid = 0xFFFF,
    }
    #endregion

    public sealed class DefinitionRecord
    {
        private readonly byte _header;
        private readonly byte _architecture;

        public byte LocalRecordNumber
        {
            get { return (byte)(_header & 0xF); }
        }

        public List<FieldDefinition> FieldDefinitions { get; private set; }

        public GlobalMessageNumber GlobalMessageNumber { get; private set; }

        public int Size { get; private set; }

        public int RecordDefinitionSize
        {
            get { return FieldDefinitions.Count * 3 + 5; }
        }

        public bool IsLittleEndian
        {
            get { return _architecture == 0; }
        }

        public DefinitionRecord(byte header, BinaryReader reader)
        {
            _header = header;

            byte reserved = reader.ReadByte();

            // 0 == Definition and Data messages are little-endian
            // 1 == Definition and Data messages are big-endian

            // TODO: do something here to deal with endian-ness on subsequent parsing
            _architecture = reader.ReadByte();

            // Range of message numbers 0:65535
            // TODO: understand what this field really means
            GlobalMessageNumber = (GlobalMessageNumber)reader.ReadUInt16();

            // Parse the all of the fields in the definition message
            byte fieldCount = reader.ReadByte();
            int currentOffset = 0;
            FieldDefinitions = new List<FieldDefinition>();
            for (int i = 0; i < fieldCount; i++)
            {
                var fieldDefinition = new FieldDefinition(reader, ref currentOffset);
                FieldDefinitions.Add(fieldDefinition);
            }

            Size = currentOffset;
        }
    }

    public sealed class FieldDefinition
    {
        public int FieldDefinitionNumber { get; private set; }

        public int FieldOffset { get; private set; }

        public int FieldType { get; private set; }

        public FieldDefinition(BinaryReader reader, ref int currentOffset)
        {
            FieldDefinitionNumber = reader.ReadByte();
            int fieldSize = reader.ReadByte();
            FieldOffset = currentOffset;
            FieldType = reader.ReadByte();
            currentOffset += fieldSize;
        }
    }

    public sealed class FileHeader
    {
        public byte Size { get; private set; }

        public byte ProtocolVersion { get; private set; }

        public ushort ProfileVersion { get; private set; }

        public uint DataSize { get; private set; }

        public ushort CRC { get; private set; }

        public FileHeader(BinaryReader reader)
        {
            Size = reader.ReadByte();
            ProtocolVersion = reader.ReadByte();
            ProfileVersion = reader.ReadUInt16();
            DataSize = reader.ReadUInt32();

            // Assert .fit signature
            byte[] sig = reader.ReadBytes(4);
            Debug.Assert(sig[0] == '.');
            Debug.Assert(sig[1] == 'F');
            Debug.Assert(sig[2] == 'I');
            Debug.Assert(sig[3] == 'T');

            // CRC is optional, and keyed off the size of the header
            if (Size > 12)
            {
                CRC = reader.ReadUInt16();
            }
        }
    }

    public sealed class DataRecord
    {
        private readonly byte _header;
        private readonly DefinitionRecord _definitionRecord;
        private readonly byte[] _recordData;

        private bool _isInitialized;
        private BinaryReader _binaryReader;


        public DataRecord(byte header, DefinitionRecord definitionRecord, BinaryReader reader)
        {
            _header = header;
            _definitionRecord = definitionRecord;
            _recordData = reader.ReadBytes(_definitionRecord.Size);
        }

        // Linear search through a DefinitionRecord's FieldDefinitions 
        // If found, will also guarantee that the internal BinaryReader
        // over the DataRecord is initialized, and pointing at the start
        // of the field.
        // Returns null if not found.
        private FieldDefinition GetFieldDefinition(FieldNumber fieldNumber)
        {
            foreach (var fieldDefinition in _definitionRecord.FieldDefinitions)
            {
                if (fieldDefinition.FieldDefinitionNumber == (byte)fieldNumber)
                {
                    if (!_isInitialized)
                    {
                        var memoryStream = new MemoryStream(_recordData);
                        _binaryReader = new BinaryReader(memoryStream);
                        _isInitialized = true;
                    }
                    _binaryReader.BaseStream.Seek(fieldDefinition.FieldOffset, SeekOrigin.Begin);
                    return fieldDefinition;
                }
            }
            return null;
        }

        public bool TryGetField(FieldNumber fieldNumber, out double value)
        {
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldNumber);
            if (fieldDefinition == null)
            {
                value = 0;
                return false;
            }
            else
            {
                if (fieldDefinition.FieldType == 0x01)
                {
                    value = Convert.ToDouble(_binaryReader.ReadSByte());
                }
                else if (fieldDefinition.FieldType == 0x02 || fieldDefinition.FieldType == 0x0A)
                {
                    value = Convert.ToDouble(_binaryReader.ReadByte());
                }
                else if (fieldDefinition.FieldType == 0x83)
                {
                    value = Convert.ToDouble(_binaryReader.ReadInt16());
                }
                else if (fieldDefinition.FieldType == 0x84 || fieldDefinition.FieldType == 0x8B)
                {
                    value = Convert.ToDouble(_binaryReader.ReadUInt16());
                }
                else if (fieldDefinition.FieldType == 0x85)
                { 
                    value = Convert.ToDouble(_binaryReader.ReadInt32());
                }
                else if (fieldDefinition.FieldType == 0x86 || fieldDefinition.FieldType == 0x8C)
                {
                    value = Convert.ToDouble(_binaryReader.ReadUInt32());
                }
                else if (fieldDefinition.FieldType == 0x88)
                {
                    value = Convert.ToDouble(_binaryReader.ReadSingle());
                }
                else if (fieldDefinition.FieldType == 0x89)
                {
                    value = Convert.ToDouble(_binaryReader.ReadDouble());
                }
                else
                {
                    value = 0;
                    return false;
                }
                return true;
            }
        }

        private readonly System.DateTime _dateTimeOffset = new System.DateTime(1989, 12, 31, 0, 0, 0, System.DateTimeKind.Utc);

        public bool TryGetField(FieldNumber fieldNumber, out System.DateTime value)
        {
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldNumber);
            if (fieldDefinition != null && fieldDefinition.FieldType == 0x86)
            {
                UInt32 timeStamp = _binaryReader.ReadUInt32();
                if (timeStamp < 0x10000000)
                {
                    throw new InvalidOperationException("timeStampValue > 0x10000000 I don't know how to compute this.");
                }
                value = new System.DateTime(timeStamp * 10000000L + _dateTimeOffset.Ticks);
                return true;
            }
            else
            {
                value = DateTime.MaxValue;
                return false;
            }
        }

        public bool TryGetField(FieldNumber fieldNumber, out string value)
        {
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldNumber);
            if (fieldDefinition != null && fieldDefinition.FieldType == 0x07)
            {
                value = _binaryReader.ReadString();
                return true;
            }
            else
            {
                value = String.Empty;
                return false;
            }
        }

        public GlobalMessageNumber GlobalMessageNumber
        {
            get { return _definitionRecord.GlobalMessageNumber; }
        }
    }

#if DEBUG
    public static class FastParserDebugHelpers
    {
        public static void DumpDefinitionRecord(DefinitionRecord recordDefinition)
        {
            Debug.WriteLine("Record definition seen: {0}, local message number: {1}",
                recordDefinition.GlobalMessageNumber,
                recordDefinition.LocalRecordNumber);

            foreach (var fieldDefinition in recordDefinition.FieldDefinitions)
            {
                Debug.WriteLine("::::Field definition number: {0}, Size: {1}, Type: {2}", fieldDefinition.FieldDefinitionNumber,
                    fieldDefinition.FieldOffset,
                    fieldDefinition.FieldType);
            }
        }
    }
#endif

    public sealed class FastParser : IDisposable
    {
        private BinaryReader _reader;
        private FileHeader _fileHeader;

        private DefinitionRecord[] _localRecordDefinitions = new DefinitionRecord[16];

        public FastParser(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _fileHeader = new FileHeader(_reader);
        }

        public bool IsFileValid()
        {
            // Compute the CRC and then reset position to start of file
            long startOfDataRecords = _reader.BaseStream.Position;

            _reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // Use new high-speed CRC calculator
            ushort crc = Crc16.ComputeCrc(_reader, (int)_fileHeader.DataSize + _fileHeader.Size);

            int fileCrc = _reader.ReadUInt16();
            bool result = (fileCrc == crc);

            // Reset position to the start of the data records
            _reader.BaseStream.Seek(startOfDataRecords, SeekOrigin.Begin);
            return result;
        }

        public IEnumerable<DataRecord> GetDataRecords()
        {
            uint bytesToRead = _fileHeader.DataSize;
            uint bytesRead = 0;

            while (bytesRead < bytesToRead)
            {
                byte header = _reader.ReadByte();

                // Normal header (vs. timestamp offset header is indicated by bit 7)
                // Message type is indicated by bit 6 
                //   1 == definition
                //   0 == record
                byte localMessageNumber = (byte)(header & 0xf);

                // Definition records are parsed internally by the parser and not exposed to
                // the caller.
                if ((header & 0x80) == 0 && (header & 0x40) == 0x40)
                {
                    // Parse the record definition and store the definition in our array
                    var recordDefinition = new DefinitionRecord(header, _reader);
                    _localRecordDefinitions[localMessageNumber] = recordDefinition;
                    bytesRead += (uint)(recordDefinition.RecordDefinitionSize + 1);
                }
                else if ((header & 0x80) == 0 && (header & 0x40) == 0)
                {
                    var currentDefinitionRecord = _localRecordDefinitions[localMessageNumber];
                    Debug.Assert(currentDefinitionRecord != null);

                    // This design reads the current record into an in-memory byte array.
                    // An alternate design would involve passing in the current binary reader
                    // and allowing the caller of DataRecord to read fields using the binary
                    // reader directly instead of creating a MemoryStream over the byte array
                    // and using a different BinaryReader in the DataRecord. I have done 
                    // exactly this and measured the performance, and it is actually SLOWER
                    // than this approach. I haven't root caused why, but would assume that
                    // Seek-ing arbitrarily using the BinaryReader over the FileStream is 
                    // slow vs. Seek-ing over a BinaryReader over a MemoryStream.

                    var dataRecord = new DataRecord(header, currentDefinitionRecord, _reader);
                    yield return dataRecord;

                    bytesRead += (uint)(currentDefinitionRecord.Size + 1);
                }
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}