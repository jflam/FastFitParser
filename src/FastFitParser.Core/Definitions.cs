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

    public static class RecordDef
    {
        public static readonly FieldDef PositionLat = new FieldDef(0, "PositionLat");
        public static readonly FieldDef PositionLong = new FieldDef(1, "PositionLong");
        public static readonly FieldDef Altitude = new FieldDef(2, "Altitude");
        public static readonly FieldDef HeartRate = new FieldDef(3, "HeartRate");
        public static readonly FieldDef Cadence = new FieldDef(4, "Cadence");
        public static readonly FieldDef Distance = new FieldDef(5, "Distance");
        public static readonly FieldDef Speed = new FieldDef(6, "Speed");
        public static readonly FieldDef Power = new FieldDef(7, "Power");
        public static readonly FieldDef CompressedSpeedDistance = new FieldDef(8, "CompressedSpeedDistance", false, true);
        public static readonly FieldDef Grade = new FieldDef(9, "Grade");
        public static readonly FieldDef Resistance = new FieldDef(10, "Resistance");
        public static readonly FieldDef TimeFromCourse = new FieldDef(11, "TimeFromCourse");
        public static readonly FieldDef CycleLength = new FieldDef(12, "CycleLength");
        public static readonly FieldDef Temperature = new FieldDef(13, "Temperature");
        public static readonly FieldDef Speed1s = new FieldDef(17, "Speed1s", false, true);
        public static readonly FieldDef Cycles = new FieldDef(18, "Cycles");
        public static readonly FieldDef TotalCycles = new FieldDef(19, "TotalCycles");
        public static readonly FieldDef CompressedAccumulatedPower = new FieldDef(28, "CompressedAccumulatedPower");
        public static readonly FieldDef AccumulatedPower = new FieldDef(29, "AccumulatedPower");
        public static readonly FieldDef LeftRightBalance = new FieldDef(30, "LeftRightBalance");
        public static readonly FieldDef GpsAccuracy = new FieldDef(31, "GpsAccuracy");
        public static readonly FieldDef VerticalSpeed = new FieldDef(32, "VerticalSpeed");
        public static readonly FieldDef Calories = new FieldDef(33, "Calories");
        public static readonly FieldDef VerticalOscillation = new FieldDef(39, "VerticalOscillation");
        public static readonly FieldDef StanceTimePercent = new FieldDef(40, "StanceTimePercent");
        public static readonly FieldDef StanceTime = new FieldDef(41, "StanceTime");
        public static readonly FieldDef ActivityType = new FieldDef(42, "ActivityType", true, false);
        public static readonly FieldDef LeftTorqueEffectiveness = new FieldDef(43, "LeftTorqueEffectiveness");
        public static readonly FieldDef RightTorqueEffectiveness = new FieldDef(44, "RightTorqueEffectiveness");
        public static readonly FieldDef LeftPedalSmoothness = new FieldDef(45, "LeftPedalSmoothness");
        public static readonly FieldDef RightPedalSmoothness = new FieldDef(46, "RightPedalSmoothness");
        public static readonly FieldDef CombinedPedalSmoothness = new FieldDef(47, "CombinedPedalSmoothness");
        public static readonly FieldDef Time128 = new FieldDef(48, "Time128");
        public static readonly FieldDef StrokeType = new FieldDef(49, "StrokeType", true, false);
        public static readonly FieldDef Zone = new FieldDef(50, "Zone");
        public static readonly FieldDef BallSpeed = new FieldDef(51, "BallSpeed");
        public static readonly FieldDef Cadence256 = new FieldDef(52, "Cadence256");
        public static readonly FieldDef TotalHemoglobinConc = new FieldDef(54, "TotalHemoglobinConc");
        public static readonly FieldDef TotalHemoglobinConcMin = new FieldDef(55, "TotalHemoglobinConcMin");
        public static readonly FieldDef TotalHemoglobinConcMax = new FieldDef(56, "TotalHemoglobinConcMax");
        public static readonly FieldDef SaturatedHemoglobinPercent = new FieldDef(57, "SaturatedHemoglobinPercent");
        public static readonly FieldDef SaturatedHemoglobinPercentMin = new FieldDef(58, "SaturatedHemoglobinPercentMin");
        public static readonly FieldDef SaturatedHemoglobinPercentMax = new FieldDef(59, "SaturatedHemoglobinPercentMax");
        public static readonly FieldDef DeviceIndex = new FieldDef(62, "DeviceIndex");
        public static readonly FieldDef TimeStamp = new FieldDef(253, "TimeStamp");
    }

    public static class EventDef
    {
        public static readonly FieldDef Event = new FieldDef(0, "Event", true, false);
        public static readonly FieldDef EventType = new FieldDef(1, "EventType", true, false);
        public static readonly FieldDef Data16 = new FieldDef(2, "Data16");
        public static readonly FieldDef Data32 = new FieldDef(3, "Data32");
    }

    public static class LapDef
    {
        public static readonly FieldDef Event = new FieldDef(0, "Event", true, false);
        public static readonly FieldDef EventType = new FieldDef(1, "EventType", true, false);
        public static readonly FieldDef StartTime = new FieldDef(2, "StartTime");
        public static readonly FieldDef StartPositionLat = new FieldDef(3, "StartPositionLat");
        public static readonly FieldDef StartPositionLong = new FieldDef(4, "StartPositionLong");
        public static readonly FieldDef EndPositionLat = new FieldDef(5, "EndPositionLat");
        public static readonly FieldDef EndPositionLong = new FieldDef(6, "EndPositionLong");
        public static readonly FieldDef TotalElapsedTime = new FieldDef(7, "TotalElapsedTime");
        public static readonly FieldDef TotalTimerTime = new FieldDef(8, "TotalTimerTime");
        public static readonly FieldDef TotalDistance = new FieldDef(9, "TotalDistance");
        public static readonly FieldDef TotalCycles = new FieldDef(10, "TotalCycles");
        public static readonly FieldDef TotalStrides = new FieldDef(10, "TotalStrides"); // alias
        public static readonly FieldDef TotalCalories = new FieldDef(11, "TotalCalories");
        public static readonly FieldDef TotalFatCalories = new FieldDef(12, "TotalFatCalories");
        public static readonly FieldDef AvgSpeed = new FieldDef(13, "AvgSpeed");
        public static readonly FieldDef MaxSpeed = new FieldDef(14, "MaxSpeed");
        public static readonly FieldDef AvgHeartRate = new FieldDef(15, "AvgHeartRate");
        public static readonly FieldDef MaxHeartRate = new FieldDef(16, "MaxHeartRate");
        public static readonly FieldDef AvgCadence = new FieldDef(17, "AvgCadence");
        public static readonly FieldDef AvgRunningCadence = new FieldDef(17, "AvgRunningCadence"); // alias
        public static readonly FieldDef MaxCadence = new FieldDef(18, "MaxCadence");
        public static readonly FieldDef MaxRunningCadence = new FieldDef(18, "MaxRunningCadence"); // alias
        public static readonly FieldDef AvgPower = new FieldDef(19, "AvgPower");
        public static readonly FieldDef MaxPower = new FieldDef(20, "MaxPower");
        public static readonly FieldDef TotalAscent = new FieldDef(21, "TotalAscent");
        public static readonly FieldDef TotalDescent = new FieldDef(22, "TotalDescent");
        public static readonly FieldDef Intensity = new FieldDef(23, "Intensity");
        public static readonly FieldDef LapTrigger = new FieldDef(24, "LapTrigger", true, false);
        public static readonly FieldDef Sport = new FieldDef(25, "Sport", true, false);
        public static readonly FieldDef EventGroup = new FieldDef(26, "EventGroup");
        public static readonly FieldDef NumLengths = new FieldDef(32, "NumLengths");
        public static readonly FieldDef NormalizedPower = new FieldDef(33, "NormalizedPower");
        public static readonly FieldDef LeftRightBalance = new FieldDef(34, "LeftRightBalance");
        public static readonly FieldDef FirstLengthIndex = new FieldDef(35, "FirstLengthIndex");
        public static readonly FieldDef AvgStrokeDistance = new FieldDef(37, "AvgStrokeDistance");
        public static readonly FieldDef SwimStroke = new FieldDef(38, "SwimStroke", true, false);
        public static readonly FieldDef SubSport = new FieldDef(39, "SubSport", true, false);
        public static readonly FieldDef NumActiveLengths = new FieldDef(40, "NumActiveLengths");
        public static readonly FieldDef TotalWork = new FieldDef(41, "TotalWork");
        public static readonly FieldDef AvgAltitude = new FieldDef(42, "AvgAltitude");
        public static readonly FieldDef MaxAltitude = new FieldDef(43, "MaxAltitude");
        public static readonly FieldDef GpsAccuracy = new FieldDef(44, "GpsAccuracy");
        public static readonly FieldDef AvgGrade = new FieldDef(45, "AvgGrade");
        public static readonly FieldDef AvgPosGrade = new FieldDef(46, "AvgPosGrade");
        public static readonly FieldDef AvgNegGrade = new FieldDef(47, "AvgNegGrade");
        public static readonly FieldDef MaxPosGrade = new FieldDef(48, "MaxPosGrade");
        public static readonly FieldDef MaxNegGrade = new FieldDef(49, "MaxNegGrade");
        public static readonly FieldDef AvgTemperature = new FieldDef(50, "AvgTemperature");
        public static readonly FieldDef MaxTemperature = new FieldDef(51, "MaxTemperature");
        public static readonly FieldDef TotalMovingTime = new FieldDef(52, "TotalMovingTime");
        public static readonly FieldDef AvgPosVerticalSpeed = new FieldDef(53, "AvgPosVerticalSpeed");
        public static readonly FieldDef AvgNegVerticalSpeed = new FieldDef(54, "AvgNegVerticalSpeed");
        public static readonly FieldDef MaxPosVerticalSpeed = new FieldDef(55, "MaxPosVerticalSpeed");
        public static readonly FieldDef MaxNegVerticalSpeed = new FieldDef(56, "MaxNegVerticalSpeed");
        public static readonly FieldDef TimeInHrZone = new FieldDef(57, "TimeInHrZone", false, true);
        public static readonly FieldDef TimeInSpeedZone = new FieldDef(58, "TimeInSpeedZone", false, true);
        public static readonly FieldDef TimeInCadenceZone = new FieldDef(59, "TimeInCadenceZone", false, true);
        public static readonly FieldDef TimeInPowerZone = new FieldDef(60, "TimeInPowerZone", false, true);
        public static readonly FieldDef RepetitionNum = new FieldDef(61, "RepetitionNum");
        public static readonly FieldDef MinAltitude = new FieldDef(62, "MinAltitude");
        public static readonly FieldDef MinHeartRate = new FieldDef(63, "MinHeartRate");
        public static readonly FieldDef WktStepIndex = new FieldDef(71, "WktStepIndex");
        public static readonly FieldDef OpponentScore = new FieldDef(74, "OpponentScore");
        public static readonly FieldDef StrokeCount = new FieldDef(75, "StrokeCount", false, true);
        public static readonly FieldDef ZoneCount = new FieldDef(76, "ZoneCount", false, true);
        public static readonly FieldDef AvgVerticalOscillation = new FieldDef(77, "AvgVerticalOscillation");
        public static readonly FieldDef AvgStanceTimePercent = new FieldDef(78, "AvgStanceTimePercent");
        public static readonly FieldDef AvgStanceTime = new FieldDef(79, "AvgStanceTime");
        public static readonly FieldDef AvgFractionalCadence = new FieldDef(80, "AvgFractionalCadence");
        public static readonly FieldDef MaxFractionalCadence = new FieldDef(81, "MaxFractionalCadence");
        public static readonly FieldDef TotalFractionalCycles = new FieldDef(82, "TotalFractionalCycles");
        public static readonly FieldDef PlayerScore = new FieldDef(83, "PlayerScore");
        public static readonly FieldDef AvgTotalHemoglobinConc = new FieldDef(84, "AvgTotalHemoglobinConc", false, true);
        public static readonly FieldDef MinTotalHemoglobinConc = new FieldDef(85, "MinTotalHemoglobinConc", false, true);
        public static readonly FieldDef MaxTotalHemoglobinConc = new FieldDef(86, "MaxTotalHemoglobinConc", false, true);
        public static readonly FieldDef AvgSaturatedHemoglobinPercent = new FieldDef(87, "AvgSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef MinSaturatedHemoglobinPercent = new FieldDef(88, "MinSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef MaxSaturatedHemoglobinPercent = new FieldDef(89, "MaxSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef AvgLeftTorqueEffectiveness = new FieldDef(91, "AvgLeftTorqueEffectiveness");
        public static readonly FieldDef AvgRightTorqueEffectiveness = new FieldDef(92, "AvgRightTorqueEffectiveness");
        public static readonly FieldDef AvgLeftPedalSmoothness = new FieldDef(93, "AvgLeftPedalSmoothness");
        public static readonly FieldDef AvgRightPedalSmoothness = new FieldDef(94, "AvgRightPedalSmoothness");
        public static readonly FieldDef AvgCombinedPedalSmoothness = new FieldDef(95, "AvgCombinedPedalSmoothness");
    }

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