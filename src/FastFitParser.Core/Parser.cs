// High performance FIT parser

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FastFitParser.Core
{
    public sealed class MessageDefinition
    {
        private readonly byte _header;
        private readonly byte _architecture;

        public byte LocalMessageNumber
        {
            get { return (byte)(_header & 0xF); }
        }

        public List<FieldDefinition> FieldDefinitions { get; private set; }

        public ushort GlobalMessageNumber { get; private set; }

        public int Size { get; private set; }

        public int MessageDefinitionSize
        {
            get { return FieldDefinitions.Count * 3 + 5; }
        }

        public bool IsLittleEndian
        {
            get { return _architecture == 0; }
        }

        public MessageDefinition(byte header, BinaryReader reader)
        {
            _header = header;

            byte reserved = reader.ReadByte();

            // 0 == Definition and Data messages are little-endian
            // 1 == Definition and Data messages are big-endian

            // TODO: do something here to deal with endian-ness on subsequent parsing
            _architecture = reader.ReadByte();

            // Range of message numbers 0:65535
            // TODO: understand what this field really means
            GlobalMessageNumber = reader.ReadUInt16();

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

    public sealed class Message
    {
        private readonly byte _header;
        private readonly MessageDefinition _messageDefinition;
        private readonly byte[] _messageData;

        private bool _isInitialized;
        private BinaryReader _binaryReader;

        public Message(byte header, MessageDefinition messageDefinition, BinaryReader reader)
        {
            _header = header;
            _messageDefinition = messageDefinition;
            _messageData = reader.ReadBytes(_messageDefinition.Size);
        }

        // Linear search through a Message's FieldDefinitions 
        // If found, will also guarantee that the internal BinaryReader
        // over the Message is initialized, and pointing at the start
        // of the field.
        // Returns null if not found.
        private FieldDefinition GetFieldDefinition(byte fieldNumber)
        {
            foreach (var fieldDefinition in _messageDefinition.FieldDefinitions)
            {
                if (fieldDefinition.FieldDefinitionNumber == (byte)fieldNumber)
                {
                    if (!_isInitialized)
                    {
                        var memoryStream = new MemoryStream(_messageData);
                        _binaryReader = new BinaryReader(memoryStream);
                        _isInitialized = true;
                    }
                    _binaryReader.BaseStream.Seek(fieldDefinition.FieldOffset, SeekOrigin.Begin);
                    return fieldDefinition;
                }
            }
            return null;
        }

        public bool TryGetField(FieldDecl fieldDecl, out double value)
        {
            value = 0;
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldDecl);
            if (fieldDefinition == null)
            {
                return false;
            }
            else
            {
                // We will return false if we encounter an invalid value in the raw data.
                // The caller needs to interpret invalid values the same as missing values.
                if (fieldDefinition.FieldType == 0x01)
                {
                    sbyte raw = _binaryReader.ReadSByte();
                    if (raw == 0x7f)
                    {
                        return false;
                    }
                    value = Convert.ToDouble(raw);
                }
                else if (fieldDefinition.FieldType == 0x02 || fieldDefinition.FieldType == 0x0A)
                {
                    byte raw = _binaryReader.ReadByte();
                    if (raw == 0xff)
                    {
                        return false; 
                    }
                    value = Convert.ToDouble(raw);
                }
                else if (fieldDefinition.FieldType == 0x83)
                {
                    Int16 raw = _binaryReader.ReadInt16();
                    if (raw == 0x7fff)
                    {
                        return false;
                    }
                    value = Convert.ToDouble(raw);
                }
                else if (fieldDefinition.FieldType == 0x84 || fieldDefinition.FieldType == 0x8B)
                {
                    UInt16 raw = _binaryReader.ReadUInt16();
                    if (raw == 0xffff)
                    {
                        return false;
                    }
                    value = Convert.ToDouble(raw);
                }
                else if (fieldDefinition.FieldType == 0x85)
                {
                    Int32 raw = _binaryReader.ReadInt32();
                    if (raw == 0x7fffffff)
                    {
                        return false;
                    }
                    value = Convert.ToDouble(raw);
                }
                else if (fieldDefinition.FieldType == 0x86 || fieldDefinition.FieldType == 0x8C)
                {
                    UInt32 raw = _binaryReader.ReadUInt32();
                    if (raw == 0xffffffff)
                    {
                        return false;
                    }
                    value = Convert.ToDouble(raw);
                }
                else if (fieldDefinition.FieldType == 0x88)
                {
                    // TODO: don't know how to handle floating point invalid values.
                    // I think I need to peek the raw bits rather than try to interpret 
                    value = Convert.ToDouble(_binaryReader.ReadSingle());
                }
                else if (fieldDefinition.FieldType == 0x89)
                {
                    // TODO: don't know how to handle floating point invalid values
                    // I think I need to peek the raw bits rather than try to interpret 
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

        public bool TryGetField(FieldDecl fieldDecl, out System.DateTime value)
        {
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldDecl);
            if (fieldDefinition != null && fieldDefinition.FieldType == 0x86)
            {
                UInt32 timeStamp = _binaryReader.ReadUInt32();
                if (timeStamp < 0x10000000)
                {
                    throw new InvalidOperationException("timeStampValue > 0x10000000 I don't know how to compute this.");
                }
                value = new System.DateTime(timeStamp * 10000000L + _dateTimeOffset.Ticks, DateTimeKind.Utc);
                return true;
            }
            else
            {
                value = DateTime.MaxValue;
                return false;
            }
        }

        public bool TryGetField(FieldDecl fieldDecl, out string value)
        {
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldDecl);
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

        // Read an enum type
        public bool TryGetField(FieldDecl fieldDecl, out byte value)
        {
            FieldDefinition fieldDefinition = GetFieldDefinition(fieldDecl);
            if (fieldDefinition != null && fieldDefinition.FieldType == 0x00)
            {
                value = _binaryReader.ReadByte();
                if (value == 0xff)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                value = 0xff;
                return false;
            }
        }

        // TODO: Read an array type

        public ushort GlobalMessageNumber
        {
            get { return _messageDefinition.GlobalMessageNumber; }
        }

        public MessageDefinition MessageDefinition
        {
            get { return _messageDefinition; }
        }
    }

#if DEBUG
    public static class FastParserDebugHelpers
    {
        public static void DumpMessageDefinition(MessageDefinition messageDefinition)
        {
            Debug.WriteLine("Message definition seen: {0}, local message number: {1}",
                messageDefinition.GlobalMessageNumber,
                messageDefinition.LocalMessageNumber);

            foreach (var fieldDefinition in messageDefinition.FieldDefinitions)
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

        private MessageDefinition[] _localMessageDefinitions = new MessageDefinition[16];

        public FastParser(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _fileHeader = new FileHeader(_reader);
        }

        public bool IsFileValid()
        {
            // Compute the CRC and then reset position to start of file
            long startOfMessages = _reader.BaseStream.Position;

            _reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // Use new high-speed CRC calculator
            ushort crc = Crc16.ComputeCrc(_reader, (int)_fileHeader.DataSize + _fileHeader.Size);

            int fileCrc = _reader.ReadUInt16();
            bool result = (fileCrc == crc);

            // Reset position to the start of the messages
            _reader.BaseStream.Seek(startOfMessages, SeekOrigin.Begin);
            return result;
        }

        public IEnumerable<Message> GetMessages()
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

                // Message definitions are parsed internally by the parser and not exposed to
                // the caller.
                if ((header & 0x80) == 0 && (header & 0x40) == 0x40)
                {
                    // Parse the message definition and store the definition in our array
                    var messageDefinition = new MessageDefinition(header, _reader);
                    _localMessageDefinitions[localMessageNumber] = messageDefinition;
                    bytesRead += (uint)(messageDefinition.MessageDefinitionSize + 1);
                }
                else if ((header & 0x80) == 0 && (header & 0x40) == 0)
                {
                    var currentMessageDefinition = _localMessageDefinitions[localMessageNumber];
                    Debug.Assert(currentMessageDefinition != null);

                    // This design reads the current message into an in-memory byte array.
                    // An alternate design would involve passing in the current binary reader
                    // and allowing the caller of Message to read fields using the binary
                    // reader directly instead of creating a MemoryStream over the byte array
                    // and using a different BinaryReader in the Message. I have done 
                    // exactly this and measured the performance, and it is actually SLOWER
                    // than this approach. I haven't root caused why, but would assume that
                    // Seek-ing arbitrarily using the BinaryReader over the FileStream is 
                    // slow vs. Seek-ing over a BinaryReader over a MemoryStream.

                    var message = new Message(header, currentMessageDefinition, _reader);
                    yield return message;

                    bytesRead += (uint)(currentMessageDefinition.Size + 1);
                }
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}