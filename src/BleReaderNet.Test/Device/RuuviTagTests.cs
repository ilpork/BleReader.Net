using BleReaderNet.Device;
using BleReaderNet.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BleNet.Test.Device
{
    ///<Summary>
    /// Tests that parser return correct data
    /// 
    /// Test data from <see href="https://github.com/ruuvi/ruuvi-sensor-protocols">Ruuvi site</see>
    ///</Summary>
    [TestClass]
    public class RuuviTagTests
    {
        [TestMethod]
        public void Parse_Datav3_Valid()
        {
            string rawData = "03-29-1A-1E-CE-1E-FC-18-F9-42-02-CA-0B-53";
            var bytes = ConvertStringBytesToByteArray(rawData);
            CheckV3Data(bytes, 26.3, 20.5, 1027.66, -1, -1.726, 0.714, 2.899);
        }
        [TestMethod]
        public void Parse_MaxV3_OK()
        {
            string rawData = "03-FF-7F-63-FF-FF-7F-FF-7F-FF-7F-FF-FF-FF";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV3Data(bytes, 127.99, 127.5, 1155.35, 32.767, 32.767, 32.767, 65.535);
        }

        [TestMethod]
        public void Parse_MinV3_OK()
        {
            string rawData = "03-00-FF-63-00-00-80-01-80-01-80-01-00-00";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV3Data(bytes, -127.99, 0, 500, -32.767, -32.767, -32.767, 0);
        }

        [TestMethod]
        public void Parse_Datav4_Valid()
        {
            string rawData = "04-29-1A-1E-CE-1E-FC";
            var bytes = ConvertStringBytesToByteArray(rawData);
            CheckV4Data(bytes, 26.3, 20.5, 1027.66);
        }
        [TestMethod]
        public void Parse_MaxV4_OK()
        {
            string rawData = "04-FF-7F-63-FF-FF-7F";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV4Data(bytes, 127.99, 127.5, 1155.35);
        }

        [TestMethod]
        public void Parse_MinV4_OK()
        {
            string rawData = "04-00-FF-63-00-00-80";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV4Data(bytes, -127.99, 0, 500);
        }

        [TestMethod]
        public void Parse_ValidV5_OK()
        {
            string rawData = "05-12-FC-53-94-C3-7C-00-04-FF-FC-04-0C-AC-36-42-00-CD-CB-B8-33-4C-88-4F";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV5Data(bytes, 24.3, 53.49, 1000.44, 0.004, -0.004, 1.036, 2.977, 4, 66, 205, "CB-B8-33-4C-88-4F");
        }

        [TestMethod]
        public void Parse_MaxV5_OK()
        {
            string rawData = "05-7F-FF-FF-FE-FF-FE-7F-FF-7F-FF-7F-FF-FF-DE-FE-FF-FE-FF-FF-FF-FF-FF-FF";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV5Data(bytes, 163.835, 163.8350, 1155.34, 32.767, 32.767, 32.767, 3.646, 20, 254, 65534, "FF-FF-FF-FF-FF-FF");
        }

        [TestMethod]
        public void Parse_MinV5_OK()
        {
            string rawData = "05-80-01-00-00-00-00-80-01-80-01-80-01-00-00-00-00-00-00-00-00-00-00-00";
            var bytes = ConvertStringBytesToByteArray(rawData);

            CheckV5Data(bytes, -163.835, 0, 500, -32.767, -32.767, -32.767, 1.6, -40, 0, 0, "00-00-00-00-00-00");
        }
        [TestMethod]
        [ExpectedException(typeof(UnsupportedDataFormatException), "Using unsupported data format was allowed")]
        public void Parse_InvalidDataFormat_ExceptionThrown()
        {
            string rawData = "01-80-01-00-00-00-00-80-01-80-01-80-01-00-00-00-00-00-CB-B8-33-4C-88-4F";
            var bytes = ConvertStringBytesToByteArray(rawData);

            RuuviTag.Parse(bytes);
        }

        private byte[] ConvertStringBytesToByteArray(string stringBytes)
        {
            return stringBytes.Split('-').Select(item => Convert.ToByte(item, 16)).ToArray();
        }

        private void CheckV3Data(byte[] bytes, double temperature, double humidity, double airPressure, double accelerationx,
            double accelerationy, double accelerationz, double batteryVoltage)
        {
            var data = RuuviTag.Parse(bytes);

            Assert.AreEqual(temperature, data.Temperature, "Temperature");
            Assert.AreEqual(humidity, data.Humidity, "Humidity");
            Assert.AreEqual(airPressure, data.AirPressure, "AirPressure");
            Assert.AreEqual(accelerationx, data.AccelerationX, "Acceleration X");
            Assert.AreEqual(accelerationy, data.AccelerationY, "Acceleration Y");
            Assert.AreEqual(accelerationz, data.AccelerationZ, "Acceleration Z");
            Assert.AreEqual(batteryVoltage, data.BatteryVoltage, "Battery voltage");
            Assert.IsNull(data.TxPower, "TX power");
            Assert.IsNull(data.MovementCounter, "Movement counter");
            Assert.IsNull(data.MeasurementSequenceNumber, "Measurement sequence");
            Assert.IsNull(data.MacAddress, "MAC address");
        }

        private void CheckV4Data(byte[] bytes, double temperature, double humidity, double airPressure)
        {
            var data = RuuviTag.Parse(bytes);

            Assert.AreEqual(temperature, data.Temperature, "Temperature");
            Assert.AreEqual(humidity, data.Humidity, "Humidity");
            Assert.AreEqual(airPressure, data.AirPressure, "AirPressure");
            Assert.IsNull(data.AccelerationX, "Acceleration X");
            Assert.IsNull(data.AccelerationY, "Acceleration Y");
            Assert.IsNull(data.AccelerationZ, "Acceleration Z");
            Assert.IsNull(data.BatteryVoltage, "Battery voltage");
            Assert.IsNull(data.TxPower, "TX power");
            Assert.IsNull(data.MovementCounter, "Movement counter");
            Assert.IsNull(data.MeasurementSequenceNumber, "Measurement sequence");
            Assert.IsNull(data.MacAddress, "MAC address");
        }

        private void CheckV5Data(byte[] bytes, double temperature, double humidity, double airPressure, double accelerationx,
            double accelerationy, double accelerationz, double batteryVoltage, double txPower, int movementCounter,
            int measurementSequenceNumber, string macAddress)
        {
            var data = RuuviTag.Parse(bytes);

            Assert.AreEqual(temperature, data.Temperature, "Temperature");
            Assert.AreEqual(humidity, data.Humidity, "Humidity");
            Assert.AreEqual(airPressure, data.AirPressure, "AirPressure");
            Assert.AreEqual(accelerationx, data.AccelerationX, "Acceleration X");
            Assert.AreEqual(accelerationy, data.AccelerationY, "Acceleration Y");
            Assert.AreEqual(accelerationz, data.AccelerationZ, "Acceleration Z");
            Assert.AreEqual(batteryVoltage, data.BatteryVoltage, "Battery voltage");
            Assert.AreEqual(txPower, data.TxPower, "TX power");
            Assert.AreEqual(movementCounter, data.MovementCounter, "Movement counter");
            Assert.AreEqual(measurementSequenceNumber, data.MeasurementSequenceNumber, "Measurement sequence");
            Assert.AreEqual(macAddress, data.MacAddress, "MAC address");
        }
    }
}
