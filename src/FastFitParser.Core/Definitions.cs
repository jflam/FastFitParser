// Definitions for record structure and field types

using System.Collections.Generic;
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
        public static readonly FieldDef Data = Definitions.DefineField(3, "Data");
        public static readonly FieldDef TimerTrigger = Definitions.DefineField(3, "TimerTrigger", true, false); // alias
        public static readonly FieldDef CoursePointIndex = Definitions.DefineField(3, "CoursePointIndex"); // alias
        public static readonly FieldDef BatteryLevel = Definitions.DefineField(3, "BatteryLevel"); // alias
        public static readonly FieldDef VirtualPartnerSpeed = Definitions.DefineField(3, "VirtualPartnerSpeed"); // alias
        public static readonly FieldDef HrHighAlert = Definitions.DefineField(3, "HrHighAlert"); // alias
        public static readonly FieldDef HrLowAlert = Definitions.DefineField(3, "HrLowAlert"); // alias
        public static readonly FieldDef SpeedHighAlert = Definitions.DefineField(3, "SpeedHighAlert"); // alias
        public static readonly FieldDef SpeedLowAlert = Definitions.DefineField(3, "SpeedLowAlert"); // alias
        public static readonly FieldDef CadHighAlert = Definitions.DefineField(3, "CadHighAlert"); // alias
        public static readonly FieldDef CadLowAlert = Definitions.DefineField(3, "CadLowAlert"); // alias
        public static readonly FieldDef PowerHighAlert = Definitions.DefineField(3, "PowerHighAlert"); // alias
        public static readonly FieldDef PowerLowAlert = Definitions.DefineField(3, "PowerLowAlert"); // alias
        public static readonly FieldDef TimeDurationAlert = Definitions.DefineField(3, "TimeDurationAlert"); // alias
        public static readonly FieldDef DistanceDurationAlert = Definitions.DefineField(3, "DistanceDurationAlert"); // alias
        public static readonly FieldDef CalorieDurationAlert = Definitions.DefineField(3, "CalorieDurationAlert"); // alias
        public static readonly FieldDef FitnessEquipmentState = Definitions.DefineField(3, "FitnessEquipmentState", true, false); // alias
        public static readonly FieldDef SportPoint = Definitions.DefineField(3, "SportPoint"); // alias
        public static readonly FieldDef GearChangeData = Definitions.DefineField(3, "GearChangeData"); // alias
        public static readonly FieldDef EventGroup = Definitions.DefineField(4, "EventGroup", true, false); 
        public static readonly FieldDef Score = Definitions.DefineField(7, "Score"); 
        public static readonly FieldDef OpponentScore = Definitions.DefineField(8, "OpponentScore"); 
        public static readonly FieldDef FrontGearNum = Definitions.DefineField(9, "FrontGearNum"); 
        public static readonly FieldDef FrontGear = Definitions.DefineField(10, "FrontGear"); 
        public static readonly FieldDef RearGearNum = Definitions.DefineField(11, "RearGearNum"); 
        public static readonly FieldDef RearGear = Definitions.DefineField(12, "RearGear"); 
        public static readonly FieldDef TimeStamp = Definitions.DefineField(253, "TimeStamp");
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

    public static class ActivityDef
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef TotalTimerTime = Definitions.DefineField(0, "TotalTimerTime");
        public static readonly FieldDef NumSessions = Definitions.DefineField(1, "NumSessions");
        public static readonly FieldDef Type = Definitions.DefineField(2, "Type", true, false);
        public static readonly FieldDef Event = Definitions.DefineField(3, "Event", true, false);
        public static readonly FieldDef EventType = Definitions.DefineField(4, "EventType", true, false);
        public static readonly FieldDef LocalTimestamp = Definitions.DefineField(5, "LocalTimestamp");
        public static readonly FieldDef EventGroup = Definitions.DefineField(6, "EventGroup");
        public static readonly FieldDef TimeStamp = Definitions.DefineField(253, "TimeStamp");
    }

    public static class FileIdDef
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef Type = Definitions.DefineField(0, "Type", true, false);
        public static readonly FieldDef Manufacturer = Definitions.DefineField(1, "Manufacturer");
        public static readonly FieldDef Product = Definitions.DefineField(2, "Product");
        public static readonly FieldDef GarminProduct = Definitions.DefineField(2, "GarminProduct"); // alias
        public static readonly FieldDef SerialNumber = Definitions.DefineField(3, "SerialNumber");
        public static readonly FieldDef TimeCreated = Definitions.DefineField(4, "TimeCreated");
        public static readonly FieldDef Number = Definitions.DefineField(5, "Number");
    }

    public static class FileCreatorDef
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef SoftwareVersion = Definitions.DefineField(0, "SoftwareVersion");
        public static readonly FieldDef HardwareVersion = Definitions.DefineField(1, "HardwareVersion");
    }

    public static class SessionDef
    {
        public static FieldDefinitions Definitions = new FieldDefinitions();

        public static readonly FieldDef Event = Definitions.DefineField(0, "Event", true, false);
        public static readonly FieldDef EventType = Definitions.DefineField(1, "EventType", true, false);
        public static readonly FieldDef StartTime = Definitions.DefineField(2, "StartTime");
        public static readonly FieldDef StartPositionLat = Definitions.DefineField(3, "StartPositionLat");
        public static readonly FieldDef StartPositionLong = Definitions.DefineField(4, "StartPositionLong");
        public static readonly FieldDef Sport = Definitions.DefineField(5, "Sport", true, false);
        public static readonly FieldDef SubSport = Definitions.DefineField(6, "SubSport", true, false);
        public static readonly FieldDef TotalElapsedTime = Definitions.DefineField(7, "TotalElapsedTime");
        public static readonly FieldDef TotalTimerTime = Definitions.DefineField(8, "TotalTimerTime");
        public static readonly FieldDef TotalDistance = Definitions.DefineField(9, "TotalDistance");
        public static readonly FieldDef TotalCycles = Definitions.DefineField(10, "TotalCycles");
        public static readonly FieldDef TotalStrides = Definitions.DefineField(10, "TotalStrides"); // alias
        public static readonly FieldDef TotalCalories = Definitions.DefineField(11, "TotalCalories");
        public static readonly FieldDef TotalFatCalories = Definitions.DefineField(13, "TotalFatCalories");
        public static readonly FieldDef AvgSpeed = Definitions.DefineField(14, "AvgSpeed");
        public static readonly FieldDef MaxSpeed = Definitions.DefineField(15, "MaxSpeed");
        public static readonly FieldDef AvgHeartRate = Definitions.DefineField(16, "AvgHeartRate");
        public static readonly FieldDef MaxHeartRate = Definitions.DefineField(17, "MaxHeartRate");
        public static readonly FieldDef AvgCadence = Definitions.DefineField(18, "AvgCadence");
        public static readonly FieldDef AvgRunningCadence = Definitions.DefineField(18, "AvgRunningCadence"); // alias
        public static readonly FieldDef MaxCadence = Definitions.DefineField(19, "MaxCadence");
        public static readonly FieldDef MaxRunningCadence = Definitions.DefineField(19, "MaxRunningCadence"); // alias
        public static readonly FieldDef AvgPower = Definitions.DefineField(20, "AvgPower");
        public static readonly FieldDef MaxPower = Definitions.DefineField(21, "MaxPower");
        public static readonly FieldDef TotalAscent = Definitions.DefineField(22, "TotalAscent");
        public static readonly FieldDef TotalDescent = Definitions.DefineField(23, "TotalDescent");
        public static readonly FieldDef TotalTrainingEffect = Definitions.DefineField(24, "TotalTrainingEffect");
        public static readonly FieldDef FirstLapIndex = Definitions.DefineField(25, "FirstLapIndex");
        public static readonly FieldDef NumLaps = Definitions.DefineField(26, "NumLaps");
        public static readonly FieldDef EventGroup = Definitions.DefineField(27, "EventGroup");
        public static readonly FieldDef Trigger = Definitions.DefineField(28, "Trigger", true, false);
        public static readonly FieldDef NecLat = Definitions.DefineField(29, "NecLat");
        public static readonly FieldDef NecLong = Definitions.DefineField(30, "NecLong");
        public static readonly FieldDef SwcLat = Definitions.DefineField(31, "SwcLat");
        public static readonly FieldDef SwcLong = Definitions.DefineField(32, "SwcLong");
        public static readonly FieldDef NormalizedPower = Definitions.DefineField(34, "NormalizedPower");
        public static readonly FieldDef TrainingStressScore = Definitions.DefineField(35, "TrainingStressScore");
        public static readonly FieldDef IntensityFactor = Definitions.DefineField(36, "IntensityFactor");
        public static readonly FieldDef LeftRightBalance = Definitions.DefineField(37, "LeftRightBalance");
        public static readonly FieldDef AvgStrokeCount = Definitions.DefineField(41, "AvgStrokeCount");
        public static readonly FieldDef AvgStrokeDistance = Definitions.DefineField(42, "AvgStrokeDistance");
        public static readonly FieldDef SwimStroke = Definitions.DefineField(43, "SwimStroke", true, false);
        public static readonly FieldDef PoolLength = Definitions.DefineField(44, "PoolLength");
        public static readonly FieldDef PoolLengthUnit = Definitions.DefineField(46, "PoolLengthUnit", true, false);
        public static readonly FieldDef NumActiveLengths = Definitions.DefineField(47, "NumActiveLengths");
        public static readonly FieldDef TotalWork = Definitions.DefineField(48, "TotalWork");
        public static readonly FieldDef AvgAltitude = Definitions.DefineField(49, "AvgAltitude");
        public static readonly FieldDef MaxAltitude = Definitions.DefineField(50, "MaxAltitude");
        public static readonly FieldDef GpsAccuracy = Definitions.DefineField(51, "GpsAccuracy");
        public static readonly FieldDef AvgGrade = Definitions.DefineField(52, "AvgGrade");
        public static readonly FieldDef AvgPosGrade = Definitions.DefineField(53, "AvgPosGrade");
        public static readonly FieldDef AvgNegGrade = Definitions.DefineField(54, "AvgNegGrade");
        public static readonly FieldDef MaxPosGrade = Definitions.DefineField(55, "MaxPosGrade");
        public static readonly FieldDef MaxNegGrade = Definitions.DefineField(56, "MaxNegGrade");
        public static readonly FieldDef AvgTemperature = Definitions.DefineField(57, "AvgTemperature");
        public static readonly FieldDef MaxTemperature = Definitions.DefineField(58, "MaxTemperature");
        public static readonly FieldDef TotalMovingTime = Definitions.DefineField(59, "TotalMovingTime");
        public static readonly FieldDef AvgPosVerticalSpeed = Definitions.DefineField(60, "AvgPosVerticalSpeed");
        public static readonly FieldDef AvgNegVerticalSpeed = Definitions.DefineField(61, "AvgNegVerticalSpeed");
        public static readonly FieldDef MaxPosVerticalSpeed = Definitions.DefineField(62, "MaxPosVerticalSpeed");
        public static readonly FieldDef MaxNegVerticalSpeed = Definitions.DefineField(63, "MaxNegVerticalSpeed");
        public static readonly FieldDef MinHeartRate = Definitions.DefineField(64, "MinHeartRate");
        public static readonly FieldDef TimeInHrZone = Definitions.DefineField(65, "TimeInHrZone", false, true);
        public static readonly FieldDef TimeInSpeedZone = Definitions.DefineField(66, "TimeInSpeedZone", false, true);
        public static readonly FieldDef TimeInCadenceZone = Definitions.DefineField(67, "TimeInCadenceZone", false, true);
        public static readonly FieldDef TimeInPowerZone = Definitions.DefineField(68, "TimeInPowerZone", false, true);
        public static readonly FieldDef AvgLapTime = Definitions.DefineField(69, "AvgLapTime");
        public static readonly FieldDef BestLapIndex = Definitions.DefineField(70, "BestLapIndex");
        public static readonly FieldDef MinAltitude = Definitions.DefineField(71, "MinAltitude");
        public static readonly FieldDef PlayerScore = Definitions.DefineField(82, "PlayerScore");
        public static readonly FieldDef OpponentScore = Definitions.DefineField(83, "OpponentScore");
        public static readonly FieldDef OpponentName = Definitions.DefineField(84, "OpponentName", false, true);  // this is a new type - byte array
        public static readonly FieldDef StrokeCount = Definitions.DefineField(85, "StrokeCount", false, true);
        public static readonly FieldDef ZoneCount = Definitions.DefineField(86, "ZoneCount", false, true);
        public static readonly FieldDef MaxBallSpeed = Definitions.DefineField(87, "MaxBallSpeed");
        public static readonly FieldDef AvgBallSpeed = Definitions.DefineField(88, "AvgBallSpeed");
        public static readonly FieldDef AvgVerticalOscillation = Definitions.DefineField(89, "AvgVerticalOscillation");
        public static readonly FieldDef AvgStanceTimePercent = Definitions.DefineField(90, "AvgStanceTimePercent");
        public static readonly FieldDef AvgStanceTime = Definitions.DefineField(91, "AvgStanceTime");
        public static readonly FieldDef AvgFractionalCadence = Definitions.DefineField(92, "AvgFractionalCadence");
        public static readonly FieldDef MaxFractionalCadence = Definitions.DefineField(93, "MaxFractionalCadence");
        public static readonly FieldDef TotalFractionalCycles = Definitions.DefineField(94, "TotalFractionalCycles");
        public static readonly FieldDef AvgTotalHemoglobinConc = Definitions.DefineField(95, "AvgTotalHemoglobinConc", false, true);
        public static readonly FieldDef MinTotalHemoglobinConc = Definitions.DefineField(96, "MinTotalHemoglobinConc", false, true);
        public static readonly FieldDef MaxTotalHemoglobinConc = Definitions.DefineField(97, "MaxTotalHemoglobinConc", false, true);
        public static readonly FieldDef AvgSaturatedHemoglobinPercent = Definitions.DefineField(98, "AvgSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef MinSaturatedHemoglobinPercent = Definitions.DefineField(99, "MinSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef MaxSaturatedHemoglobinPercent = Definitions.DefineField(100, "MaxSaturatedHemoglobinPercent", false, true);
        public static readonly FieldDef AvgLeftTorqueEffectiveness = Definitions.DefineField(101, "AvgLeftTorqueEffectiveness");
        public static readonly FieldDef AvgRightTorqueEffectiveness = Definitions.DefineField(102, "AvgRightTorqueEffectiveness");
        public static readonly FieldDef AvgLeftPedalSmoothness = Definitions.DefineField(103, "AvgLeftPedalSmoothness");
        public static readonly FieldDef AvgRightPedalSmoothness = Definitions.DefineField(104, "AvgRightPedalSmoothness");
        public static readonly FieldDef AvgCombinedPedalSmoothness = Definitions.DefineField(105, "AvgCombinedPedalSmoothness");
        public static readonly FieldDef TimeStamp = Definitions.DefineField(253, "TimeStamp");
        public static readonly FieldDef MessageIndex = Definitions.DefineField(254, "MessageIndex");
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

    public class MessageDef
    {
        public readonly ushort MessageNumber;
        public readonly string MessageName;
        public readonly FieldDefinitions FieldDefinitions;

        public MessageDef(ushort messageNumber, string messageName, FieldDefinitions fieldDefinitions)
        {
            MessageNumber = messageNumber;
            MessageName = messageName;
            FieldDefinitions = fieldDefinitions;
        }

        public static implicit operator ushort(MessageDef message)
        {
            return message.MessageNumber;
        }
    }

    // Global message numbers identify unique message types in the .FIT file format. AFAIK, all records
    // are dynamically defined within the .FIT file; that is the schema for each message type is encoded
    // within the file itself. These identifiers are only for well-known message types to help the parser
    // categorize the data itself.
    public static class GlobalMessageDefs
    {
        public static readonly Dictionary<int, MessageDef> Messages = new Dictionary<int, MessageDef>();

        private static MessageDef Add(byte messageNumber, string messageName, FieldDefinitions fieldDefinitions)
        {
            var messageDef = new MessageDef(messageNumber, messageName, fieldDefinitions);
            Messages[messageNumber] = messageDef;
            return messageDef;
        }

        public static readonly MessageDef FileId = Add(0, "FileId", FileIdDef.Definitions);
        public static readonly MessageDef Capabilities = Add(1, "Capabilities", null);
        public static readonly MessageDef DeviceSettings = Add(2, "DeviceSettings", null);
        public static readonly MessageDef UserProfile = Add(3, "UserProfile", null);
        public static readonly MessageDef HrmProfile = Add(4, "HrmProfile", null);
        public static readonly MessageDef SdmProfile = Add(5, "SdmProfile", null);
        public static readonly MessageDef BikeProfile = Add(6, "BikeProfile", null);
        public static readonly MessageDef ZonesTarget = Add(7, "ZonesTarget", null);
        public static readonly MessageDef HrZone = Add(8, "HrZone", null);
        public static readonly MessageDef PowerZone = Add(9, "PowerZone", null);
        public static readonly MessageDef MetZone = Add(10, "MetZone", null);
        public static readonly MessageDef Sport = Add(12, "Sport", null);
        public static readonly MessageDef Goal = Add(15, "Goal", null);
        public static readonly MessageDef Session = Add(18, "Session", SessionDef.Definitions);
        public static readonly MessageDef Lap = Add(19, "Lap", LapDef.Definitions);
        public static readonly MessageDef Record = Add(20, "Record", RecordDef.Definitions);
        public static readonly MessageDef Event = Add(21, "Event", EventDef.Definitions);
        public static readonly MessageDef DeviceInfo = Add(23, "DeviceInfo", DeviceInfoDef.Definitions);
        public static readonly MessageDef Workout = Add(26, "Workout", null);
        public static readonly MessageDef WorkoutStep = Add(27, "WorkoutStep", null);
        public static readonly MessageDef Schedule = Add(28, "Schedule", null);
        public static readonly MessageDef WeightScale = Add(30, "WeightScale", null);
        public static readonly MessageDef Course = Add(31, "Course", null);
        public static readonly MessageDef CoursePoint = Add(32, "CoursePoint", null);
        public static readonly MessageDef Totals = Add(33, "Totals", null);
        public static readonly MessageDef Activity = Add(34, "Activity", ActivityDef.Definitions);
        public static readonly MessageDef Software = Add(35, "Software", null);
        public static readonly MessageDef FileCapabilities = Add(37, "FileCapabilities", null);
        public static readonly MessageDef MesgCapabilities = Add(38, "MesgCapabilities", null);
        public static readonly MessageDef FieldCapabilities = Add(39, "FieldCapabilities", null);
        public static readonly MessageDef FileCreator = Add(49, "FileCreator", FileCreatorDef.Definitions);
        public static readonly MessageDef BloodPressure = Add(51, "BloodPressure", null);
        public static readonly MessageDef SpeedZone = Add(53, "SpeedZone", null);
        public static readonly MessageDef Monitoring = Add(55, "Monitoring", null);
        public static readonly MessageDef Hrv = Add(78, "Hrv", null);
        public static readonly MessageDef Length = Add(101, "Length", null);
        public static readonly MessageDef MonitoringInfo = Add(103, "MonitoringInfo", null);
        public static readonly MessageDef Pad = Add(105, "Pad", null);
        public static readonly MessageDef SlaveDevice = Add(106, "SlaveDevice", null);
        public static readonly MessageDef CadenceZone = Add(131, "CadenceZone", null);
        public static readonly MessageDef MemoGlob = Add(145, "MemoGlob", null);
    }
}