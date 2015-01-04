// Definitions for record structure and field types

namespace FastFitParser.Core
{
    // TODO: consider putting the type of the field here as well, and throwing type mismatch errors from the TryGet function
    public class FieldDef
    {
        public FieldDef(byte fieldNumber, string fieldName)
        {
            FieldNumber = fieldNumber;
            FieldName = fieldName;
        }

        public FieldDef(byte fieldNumber, string fieldName, bool isEnum, bool isArray)
            : this(fieldNumber, fieldName)
        {
            IsEnum = isEnum;
            IsArray = isArray;
        }

        public readonly string FieldName;

        public readonly byte FieldNumber;

        public readonly bool IsEnum;

        public readonly bool IsArray;

        public static implicit operator byte(FieldDef field)
        {
            return field.FieldNumber;
        }
    }

    public class FieldDefinitions
    {
        public readonly string[] FieldNames = new string[256];

        public FieldDef DefineField(byte fieldNumber, string fieldName, bool isEnum, bool isArray)
        {
            var result = new FieldDef(fieldNumber, fieldName, isEnum, isArray);
            if (FieldNames[fieldNumber] != null)
            {
                FieldNames[fieldNumber] += "/" + fieldName;
            }
            else
            {
                FieldNames[fieldNumber] = fieldName;
            }
            return result;
        }
        
        public FieldDef DefineField(byte fieldNumber, string fieldName)
        {
            return DefineField(fieldNumber, fieldName, false, false);
        }
    }

    public class RecordDef : FieldDefinitions
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef PositionLat = Definitions.DefineField(0, "PositionLat");
        public static readonly FieldDef PositionLong = Definitions.DefineField(1, "PositionLong");
        public static readonly FieldDef Altitude = Definitions.DefineField(2, "Altitude");
        public static readonly FieldDef HeartRate = Definitions.DefineField(3, "HeartRate");
        public static readonly FieldDef Cadence = Definitions.DefineField(4, "Cadence");
        public static readonly FieldDef Distance = Definitions.DefineField(5, "Distance");
        public static readonly FieldDef Speed = Definitions.DefineField(6, "Speed");
        public static readonly FieldDef Power = Definitions.DefineField(7, "Power");
        public static readonly FieldDef CompressedSpeedDistance = Definitions.DefineField(8, "CompressedSpeedDistance", false, true);
        public static readonly FieldDef Grade = Definitions.DefineField(9, "Grade");
        public static readonly FieldDef Resistance = Definitions.DefineField(10, "Resistance");
        public static readonly FieldDef TimeFromCourse = Definitions.DefineField(11, "TimeFromCourse");
        public static readonly FieldDef CycleLength = Definitions.DefineField(12, "CycleLength");
        public static readonly FieldDef Temperature = Definitions.DefineField(13, "Temperature");
        public static readonly FieldDef Speed1s = Definitions.DefineField(17, "Speed1s", false, true);
        public static readonly FieldDef Cycles = Definitions.DefineField(18, "Cycles");
        public static readonly FieldDef TotalCycles = Definitions.DefineField(19, "TotalCycles");
        public static readonly FieldDef CompressedAccumulatedPower = Definitions.DefineField(28, "CompressedAccumulatedPower");
        public static readonly FieldDef AccumulatedPower = Definitions.DefineField(29, "AccumulatedPower");
        public static readonly FieldDef LeftRightBalance = Definitions.DefineField(30, "LeftRightBalance");
        public static readonly FieldDef GpsAccuracy = Definitions.DefineField(31, "GpsAccuracy");
        public static readonly FieldDef VerticalSpeed = Definitions.DefineField(32, "VerticalSpeed");
        public static readonly FieldDef Calories = Definitions.DefineField(33, "Calories");
        public static readonly FieldDef VerticalOscillation = Definitions.DefineField(39, "VerticalOscillation");
        public static readonly FieldDef StanceTimePercent = Definitions.DefineField(40, "StanceTimePercent");
        public static readonly FieldDef StanceTime = Definitions.DefineField(41, "StanceTime");
        public static readonly FieldDef ActivityType = Definitions.DefineField(42, "ActivityType", true, false);
        public static readonly FieldDef LeftTorqueEffectiveness = Definitions.DefineField(43, "LeftTorqueEffectiveness");
        public static readonly FieldDef RightTorqueEffectiveness = Definitions.DefineField(44, "RightTorqueEffectiveness");
        public static readonly FieldDef LeftPedalSmoothness = Definitions.DefineField(45, "LeftPedalSmoothness");
        public static readonly FieldDef RightPedalSmoothness = Definitions.DefineField(46, "RightPedalSmoothness");
        public static readonly FieldDef CombinedPedalSmoothness = Definitions.DefineField(47, "CombinedPedalSmoothness");
        public static readonly FieldDef Time128 = Definitions.DefineField(48, "Time128");
        public static readonly FieldDef StrokeType = Definitions.DefineField(49, "StrokeType", true, false);
        public static readonly FieldDef Zone = Definitions.DefineField(50, "Zone");
        public static readonly FieldDef BallSpeed = Definitions.DefineField(51, "BallSpeed");
        public static readonly FieldDef Cadence256 = Definitions.DefineField(52, "Cadence256");
        public static readonly FieldDef TotalHemoglobinConc = Definitions.DefineField(54, "TotalHemoglobinConc");
        public static readonly FieldDef TotalHemoglobinConcMin = Definitions.DefineField(55, "TotalHemoglobinConcMin");
        public static readonly FieldDef TotalHemoglobinConcMax = Definitions.DefineField(56, "TotalHemoglobinConcMax");
        public static readonly FieldDef SaturatedHemoglobinPercent = Definitions.DefineField(57, "SaturatedHemoglobinPercent");
        public static readonly FieldDef SaturatedHemoglobinPercentMin = Definitions.DefineField(58, "SaturatedHemoglobinPercentMin");
        public static readonly FieldDef SaturatedHemoglobinPercentMax = Definitions.DefineField(59, "SaturatedHemoglobinPercentMax");
        public static readonly FieldDef DeviceIndex = Definitions.DefineField(62, "DeviceIndex");
        public static readonly FieldDef TimeStamp = Definitions.DefineField(253, "TimeStamp");
    }

    public class EventDef : FieldDefinitions
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef Event = Definitions.DefineField(0, "Event", true, false);
        public static readonly FieldDef EventType = Definitions.DefineField(1, "EventType", true, false);
        public static readonly FieldDef Data16 = Definitions.DefineField(2, "Data16");
        public static readonly FieldDef Data32 = Definitions.DefineField(3, "Data32");
    }

    public static class LapDef 
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef Event = Definitions.DefineField(0, "Event", true, false);
        public static readonly FieldDef EventType = Definitions.DefineField(1, "EventType", true, false);
        public static readonly FieldDef StartTime = Definitions.DefineField(2, "StartTime");
        public static readonly FieldDef StartPositionLat = Definitions.DefineField(3, "StartPositionLat");
        public static readonly FieldDef StartPositionLong = Definitions.DefineField(4, "StartPositionLong");
        public static readonly FieldDef EndPositionLat = Definitions.DefineField(5, "EndPositionLat");
        public static readonly FieldDef EndPositionLong = Definitions.DefineField(6, "EndPositionLong");
        public static readonly FieldDef TotalElapsedTime = Definitions.DefineField(7, "TotalElapsedTime");
        public static readonly FieldDef TotalTimerTime = Definitions.DefineField(8, "TotalTimerTime");
        public static readonly FieldDef TotalDistance = Definitions.DefineField(9, "TotalDistance");
        public static readonly FieldDef TotalCycles = Definitions.DefineField(10, "TotalCycles");
        public static readonly FieldDef TotalStrides = Definitions.DefineField(10, "TotalStrides"); // alias
        public static readonly FieldDef TotalCalories = Definitions.DefineField(11, "TotalCalories");
        public static readonly FieldDef TotalFatCalories = Definitions.DefineField(12, "TotalFatCalories");
        public static readonly FieldDef AvgSpeed = Definitions.DefineField(13, "AvgSpeed");
        public static readonly FieldDef MaxSpeed = Definitions.DefineField(14, "MaxSpeed");
        public static readonly FieldDef AvgHeartRate = Definitions.DefineField(15, "AvgHeartRate");
        public static readonly FieldDef MaxHeartRate = Definitions.DefineField(16, "MaxHeartRate");
        public static readonly FieldDef AvgCadence = Definitions.DefineField(17, "AvgCadence");
        public static readonly FieldDef AvgRunningCadence = Definitions.DefineField(17, "AvgRunningCadence"); // alias
        public static readonly FieldDef MaxCadence = Definitions.DefineField(18, "MaxCadence");
        public static readonly FieldDef MaxRunningCadence = Definitions.DefineField(18, "MaxRunningCadence"); // alias
        public static readonly FieldDef AvgPower = Definitions.DefineField(19, "AvgPower");
        public static readonly FieldDef MaxPower = Definitions.DefineField(20, "MaxPower");
        public static readonly FieldDef TotalAscent = Definitions.DefineField(21, "TotalAscent");
        public static readonly FieldDef TotalDescent = Definitions.DefineField(22, "TotalDescent");
        public static readonly FieldDef Intensity = Definitions.DefineField(23, "Intensity");
        public static readonly FieldDef LapTrigger = Definitions.DefineField(24, "LapTrigger", true, false);
        public static readonly FieldDef Sport = Definitions.DefineField(25, "Sport", true, false);
        public static readonly FieldDef EventGroup = Definitions.DefineField(26, "EventGroup");
        public static readonly FieldDef NumLengths = Definitions.DefineField(32, "NumLengths");
        public static readonly FieldDef NormalizedPower = Definitions.DefineField(33, "NormalizedPower");
        public static readonly FieldDef LeftRightBalance = Definitions.DefineField(34, "LeftRightBalance");
        public static readonly FieldDef FirstLengthIndex = Definitions.DefineField(35, "FirstLengthIndex");
        public static readonly FieldDef AvgStrokeDistance = Definitions.DefineField(37, "AvgStrokeDistance");
        public static readonly FieldDef SwimStroke = Definitions.DefineField(38, "SwimStroke", true, false);
        public static readonly FieldDef SubSport = Definitions.DefineField(39, "SubSport", true, false);
        public static readonly FieldDef NumActiveLengths = Definitions.DefineField(40, "NumActiveLengths");
        public static readonly FieldDef TotalWork = Definitions.DefineField(41, "TotalWork");
        public static readonly FieldDef AvgAltitude = Definitions.DefineField(42, "AvgAltitude");
        public static readonly FieldDef MaxAltitude = Definitions.DefineField(43, "MaxAltitude");
        public static readonly FieldDef GpsAccuracy = Definitions.DefineField(44, "GpsAccuracy");
        public static readonly FieldDef AvgGrade = Definitions.DefineField(45, "AvgGrade");
        public static readonly FieldDef AvgPosGrade = Definitions.DefineField(46, "AvgPosGrade");
        public static readonly FieldDef AvgNegGrade = Definitions.DefineField(47, "AvgNegGrade");
        public static readonly FieldDef MaxPosGrade = Definitions.DefineField(48, "MaxPosGrade");
        public static readonly FieldDef MaxNegGrade = Definitions.DefineField(49, "MaxNegGrade");
        public static readonly FieldDef AvgTemperature = Definitions.DefineField(50, "AvgTemperature");
        public static readonly FieldDef MaxTemperature = Definitions.DefineField(51, "MaxTemperature");
        public static readonly FieldDef TotalMovingTime = Definitions.DefineField(52, "TotalMovingTime");
        public static readonly FieldDef AvgPosVerticalSpeed = Definitions.DefineField(53, "AvgPosVerticalSpeed");
        public static readonly FieldDef AvgNegVerticalSpeed = Definitions.DefineField(54, "AvgNegVerticalSpeed");
        public static readonly FieldDef MaxPosVerticalSpeed = Definitions.DefineField(55, "MaxPosVerticalSpeed");
        public static readonly FieldDef MaxNegVerticalSpeed = Definitions.DefineField(56, "MaxNegVerticalSpeed");
        public static readonly FieldDef TimeInHrZone = Definitions.DefineField(57, "TimeInHrZone", false, true);
        public static readonly FieldDef TimeInSpeedZone = Definitions.DefineField(58, "TimeInSpeedZone", false, true);
        public static readonly FieldDef TimeInCadenceZone = Definitions.DefineField(59, "TimeInCadenceZone", false, true);
        public static readonly FieldDef TimeInPowerZone = Definitions.DefineField(60, "TimeInPowerZone", false, true);
        public static readonly FieldDef RepetitionNum = Definitions.DefineField(61, "RepetitionNum");
        public static readonly FieldDef MinAltitude = Definitions.DefineField(62, "MinAltitude");
        public static readonly FieldDef MinHeartRate = Definitions.DefineField(63, "MinHeartRate");
        public static readonly FieldDef WktStepIndex = Definitions.DefineField(71, "WktStepIndex");
        public static readonly FieldDef OpponentScore = Definitions.DefineField(74, "OpponentScore");
        public static readonly FieldDef StrokeCount = Definitions.DefineField(75, "StrokeCount", false, true);
        public static readonly FieldDef ZoneCount = Definitions.DefineField(76, "ZoneCount", false, true);
        public static readonly FieldDef AvgVerticalOscillation = Definitions.DefineField(77, "AvgVerticalOscillation");
        public static readonly FieldDef AvgStanceTimePercent = Definitions.DefineField(78, "AvgStanceTimePercent");
        public static readonly FieldDef AvgStanceTime = Definitions.DefineField(79, "AvgStanceTime");
        public static readonly FieldDef AvgFractionalCadence = Definitions.DefineField(80, "AvgFractionalCadence");
        public static readonly FieldDef MaxFractionalCadence = Definitions.DefineField(81, "MaxFractionalCadence");
        public static readonly FieldDef TotalFractionalCycles = Definitions.DefineField(82, "TotalFractionalCycles");
        public static readonly FieldDef PlayerScore = Definitions.DefineField(83, "PlayerScore");
        public static readonly FieldDef AvgTotalHemoglobinConc = Definitions.DefineField(84, "AvgTotalHemoglobinConc", false, true);
        public static readonly FieldDef MinTotalHemoglobinConc = Definitions.DefineField(85, "MinTotalHemoglobinConc", false, true);
        public static readonly FieldDef MaxTotalHemoglobinConc = Definitions.DefineField(86, "MaxTotalHemoglobinConc", false, true);
        public static readonly FieldDef AvgSaturatedHemoglobinPercent = Definitions.DefineField(87, "AvgSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef MinSaturatedHemoglobinPercent = Definitions.DefineField(88, "MinSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef MaxSaturatedHemoglobinPercent = Definitions.DefineField(89, "MaxSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef AvgLeftTorqueEffectiveness = Definitions.DefineField(91, "AvgLeftTorqueEffectiveness");
        public static readonly FieldDef AvgRightTorqueEffectiveness = Definitions.DefineField(92, "AvgRightTorqueEffectiveness");
        public static readonly FieldDef AvgLeftPedalSmoothness = Definitions.DefineField(93, "AvgLeftPedalSmoothness");
        public static readonly FieldDef AvgRightPedalSmoothness = Definitions.DefineField(94, "AvgRightPedalSmoothness");
        public static readonly FieldDef AvgCombinedPedalSmoothness = Definitions.DefineField(95, "AvgCombinedPedalSmoothness");
        public static readonly FieldDef TimeStamp = Definitions.DefineField(253, "TimeStamp");
        public static readonly FieldDef MessageIndex = Definitions.DefineField(254, "MessageIndex");
    }

    public static class DeviceInfoDef
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef DeviceIndex = Definitions.DefineField(0, "DeviceIndex");
        public static readonly FieldDef DeviceType = Definitions.DefineField(1, "DeviceType");
        public static readonly FieldDef AntplusDeviceType = Definitions.DefineField(1, "AntplusDeviceType"); // alias
        public static readonly FieldDef AntDeviceType = Definitions.DefineField(1, "AntDeviceType"); // alias
        public static readonly FieldDef Manufacturer = Definitions.DefineField(2, "Manufacturer"); 
        public static readonly FieldDef SerialNumber = Definitions.DefineField(3, "SerialNumber"); 
        public static readonly FieldDef Product = Definitions.DefineField(4, "Product"); 
        public static readonly FieldDef SoftwareVersion = Definitions.DefineField(5, "SoftwareVersion"); 
        public static readonly FieldDef HardwareVersion = Definitions.DefineField(6, "HardwareVersion"); 
        public static readonly FieldDef CumOperatingTime = Definitions.DefineField(7, "CumOperatingTime"); 
        public static readonly FieldDef BatteryVoltage = Definitions.DefineField(10, "BatteryVoltage"); 
        public static readonly FieldDef BatteryStatus = Definitions.DefineField(11, "BatteryStatus"); 
        public static readonly FieldDef SensorPosition = Definitions.DefineField(18, "SensorPosition", true, false); 
        public static readonly FieldDef Descriptor = Definitions.DefineField(19, "Descriptor", false, true);  // this is a new type - byte array
        public static readonly FieldDef AntTransmissionType = Definitions.DefineField(20, "AntTransmissionType"); 
        public static readonly FieldDef AntDeviceNumber = Definitions.DefineField(21, "AntDeviceNumber"); 
        public static readonly FieldDef AntNetwork = Definitions.DefineField(22, "AntNetwork", true, false); 
        public static readonly FieldDef SourceType = Definitions.DefineField(25, "SourceType", true, false); 
        public static readonly FieldDef TimeStamp = Definitions.DefineField(253, "TimeStamp");
    }

    // TODO: Session, Activity, FileId, FileCreator

    public enum Event : byte
    {
        Timer = 0,
        Workout = 3,
        WorkoutStep = 4,
        PowerDown = 5,
        PowerUp = 6,
        OffCourse = 7,
        Session = 8,
        Lap = 9,
        CoursePoint = 10,
        Battery = 11,
        VirtualPartnerPace = 12,
        HrHighAlert = 13,
        HrLowAlert = 14,
        SpeedHighAlert = 15,
        SpeedLowAlert = 16,
        CadHighAlert = 17,
        CadLowAlert = 18,
        PowerHighAlert = 19,
        PowerLowAlert = 20,
        RecoveryHr = 21,
        BatteryLow = 22,
        TimeDurationAlert = 23,
        DistanceDurationAlert = 24,
        CalorieDurationAlert = 25,
        Activity = 26,
        FitnessEquipment = 27,
        Length = 28,
        UserMarker = 32,
        SportPoint = 33,
        Calibration = 36,
        Invalid = 0xFF
    }

    public enum EventType : byte
    {
        Start = 0,
        Stop = 1,
        ConsecutiveDepreciated = 2,
        Marker = 3,
        StopAll = 4,
        BeginDepreciated = 5,
        EndDepreciated = 6,
        EndAllDepreciated = 7,
        StopDisable = 8,
        StopDisableAll = 9,
        Invalid = 0xFF

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
}