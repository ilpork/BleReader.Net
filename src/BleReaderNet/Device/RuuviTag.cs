using BleReaderNet.Exception;
using System;

namespace BleReaderNet.Device
{
    ///<Summary>
    /// Provides methods for parsing RuuviTag data. <see href="https://github.com/ruuvi/ruuvi-sensor-protocols"/>
    ///</Summary>
    public class RuuviTag
    {
        public const ushort ManufacturerId = 1177;

        public int DataFormat { get; set; }
        public double? Humidity { get; set; }
        public double? Temperature { get; set; }
        public double? AirPressure { get; set; }
        public double? AccelerationX { get; set; }
        public double? AccelerationY { get; set; }
        public double? AccelerationZ { get; set; }
        public double? BatteryVoltage { get; set; }
        public double? TxPower { get; set; }
        public double? MovementCounter { get; set; }
        public double? MeasurementSequenceNumber { get; set; }
        public string MacAddress { get; set; }

        /// <summary>
        /// Parses the raw data of RuuviTag sensor
        /// </summary>
        /// <param name="bytes">Raw data as byte array</param>
        /// <returns>Instance of <see cref="RuuviTag"/></returns>
        public static RuuviTag Parse(byte[] bytes)
        {
            var ruuviTag = new RuuviTag();

            ruuviTag.DataFormat = bytes[0];

            if (ruuviTag.DataFormat != 3 && ruuviTag.DataFormat != 4 && ruuviTag.DataFormat != 5)
            {
                throw new UnsupportedDataFormatException($"Data format '{ruuviTag.DataFormat}' is not supported", ruuviTag.DataFormat);
            }

            ruuviTag.Temperature = GetTemperature(ruuviTag.DataFormat, bytes);
            ruuviTag.Humidity = GetHumidity(ruuviTag.DataFormat, bytes);
            ruuviTag.AirPressure = GetAirPressure(ruuviTag.DataFormat, bytes);
            ruuviTag.AccelerationX = GetAccelerationX(ruuviTag.DataFormat, bytes);
            ruuviTag.AccelerationY = GetAccelerationY(ruuviTag.DataFormat, bytes);
            ruuviTag.AccelerationZ = GetAccelerationZ(ruuviTag.DataFormat, bytes);
            ruuviTag.BatteryVoltage = GetBatteryVoltage(ruuviTag.DataFormat, bytes);
            ruuviTag.TxPower = GetTxPower(ruuviTag.DataFormat, bytes);
            ruuviTag.MeasurementSequenceNumber = GetMeasurementSequenceNumber(ruuviTag.DataFormat, bytes);
            ruuviTag.MovementCounter = GetMovementCounter(ruuviTag.DataFormat, bytes);
            ruuviTag.MacAddress = GetMacAddress(ruuviTag.DataFormat, bytes);

            return ruuviTag;
        }

        private static double? GetTemperature(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                case 4:
                    var tempFraction = (double)bytes[3] / 100;
                    var tempValue = bytes[2] & 0x7F;
                    var sign = bytes[2] & 0x80; 
                    return sign > 0 ? 0 - (tempValue + tempFraction) : tempValue + tempFraction;
                case 5:
                    return CalculateTwosComplement(bytes[1], bytes[2], 16) * 0.005;
                default:
                    return null;
            }
        }

        private static double? GetHumidity(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                case 4:
                    return (double)bytes[1] / 2;
                case 5:
                    return ((bytes[3] << 8) + bytes[4]) * 0.0025;
                default:
                    return null;
            }
        }

        private static double? GetAirPressure(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                case 4:
                    return (double)((bytes[4] << 8) + bytes[5] + 50000) / 100;
                case 5:
                    return (double)((bytes[5] << 8) + bytes[6] + 50000) / 100;
                default:
                    return null;
            }
        }

        private static double? GetAccelerationX(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                    return (double)CalculateTwosComplement(bytes[6], bytes[7], 16) / 1000;
                case 5:
                    return (double)CalculateTwosComplement(bytes[7], bytes[8], 16) / 1000;
                default:
                    return null;
            }
        }

        private static double? GetAccelerationY(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                    return (double)CalculateTwosComplement(bytes[8], bytes[9], 16) / 1000;
                case 5:
                    return (double)CalculateTwosComplement(bytes[9], bytes[10], 16) / 1000;
                default:
                    return null;
            }
        }

        private static double? GetAccelerationZ(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                    return (double)CalculateTwosComplement(bytes[10], bytes[11], 16) / 1000;
                case 5:
                    return (double)CalculateTwosComplement(bytes[11], bytes[12], 16) / 1000;
                default:
                    return null;
            }
        }

        private static double? GetBatteryVoltage(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 3:
                    return ((double)(bytes[12] << 8) + bytes[13]) / 1000;
                case 5:
                    var voltageRangeStart = 1600;
                    var voltageValue = (bytes[13] << 8) + bytes[14]; 
                    var batteryVoltage = (voltageValue & 0xFFE0) >> 5; 
                    return (double)(voltageRangeStart + batteryVoltage) / 1000;
                default:
                    return null;
            }
        }

        private static double? GetMovementCounter(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 5:
                    return bytes[15];
                default:
                    return null;
            }
        }

        private static double? GetMeasurementSequenceNumber(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 5:
                    return (bytes[16] << 8) + bytes[17];
                default:
                    return null;
            }
        }

        private static double? GetTxPower(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 5:
                    var txPowerRangeStart = -40;
                    var value = (bytes[13] << 8) + bytes[14]; 
                    var txPower = value & 0x1F; 
                    return txPowerRangeStart + txPower * 2;
                default:
                    return null;
            }
        }

        private static string GetMacAddress(int dataFormat, byte[] bytes)
        {
            switch (dataFormat)
            {
                case 5:
                    var macBytes = bytes[18..24];
                    return BitConverter.ToString(macBytes);
                default:
                    return null;
            }
        }

        private static int CalculateTwosComplement(byte firstByte, byte secondByte, int numberOfBits)
        {
            var value = (firstByte << 8) + secondByte;
            if ((value & 1 << numberOfBits - 1) != 0)
                value = value - (1 << numberOfBits);
            return value;
        }
    }
}
