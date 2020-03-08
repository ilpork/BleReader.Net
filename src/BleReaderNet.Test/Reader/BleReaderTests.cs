using BleReaderNet.Device;
using BleReaderNet.Exception;
using BleReaderNet.Reader;
using BleReaderNet.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BleReaderNet.Test.Reader
{
    ///<Summary>
    /// Test data from <see href="https://github.com/ruuvi/ruuvi-sensor-protocols">Ruuvi site</see>
    ///</Summary>
    [TestClass]
    public class BleReaderTests
    {   
        [TestMethod]
        [DataRow(0)]
        [DataRow(2)]
        public async Task Scan_DevicesFound(int mockDeviceCount)
        {
            var adapterName = "adapter";
            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var sourceDeviceList = new List<IBluetoothDevice>();

            for (int i = 1; i <= mockDeviceCount; i++)
            {
                sourceDeviceList.Add(btDeviceMock.Object);
            }

            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);

            var deviceCount = await bleReader.Scan(adapterName, 0);

            Assert.AreEqual(sourceDeviceList.Count, deviceCount, "Scanning did not return correct amount of devices found");
        }

        [TestMethod]       
        public async Task Scan_AdapterNameNull_GiveFirstAdapter_Found()
        {
            string adapterName = null;
            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var sourceDeviceList = new List<IBluetoothDevice>();         

            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);            
            btServiceMock.Setup(s => s.GetAdapter(null)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);

            var deviceCount = await bleReader.Scan(adapterName, 0);

            Assert.AreEqual(sourceDeviceList.Count, deviceCount, "Using null as adapter name should result in using first found adapter");
        }

        [TestMethod]
        [ExpectedException(typeof(AdapterNotFoundException), "Correct exception not thrown when adapter not found")]
        public async Task Scan_AdapterFound_False()
        {
            string adapterName = null;
            var btServiceMock = new Mock<IBluetoothService>();
            IBluetoothAdapter btAdapter = null;            
                        
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapter);
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.Scan();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public async Task Scan_ScanDuration(int scanDurationSeconds)
        {
            var adapterName = "adapter";
            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var sourceDeviceList = new List<IBluetoothDevice>();

            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);

            var sw = new Stopwatch();
            sw.Start();
            var deviceCount = await bleReader.Scan(adapterName, scanDurationSeconds);
            var elapsedSeconds = (int)TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds;
            sw.Stop();

            Assert.AreEqual(scanDurationSeconds, elapsedSeconds, "Scanning did take expected time");
        }

        [TestMethod]
        [ExpectedException(typeof(DevicesNotScannedException), "Correct exception not thrown when accessing data without scanning devices first")]
        public async Task GetAllDevices_ScanMade_False()
        {
            var btServiceMock = new Mock<IBluetoothService>();
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.GetAllDevices();
        }

        [TestMethod]        
        public async Task GetAllDevices_ManufacturerData_Valid()
        {
            var adapterName = "adapter";
            var address = "11:22:33:44:55:66";
            var name = "some device name";
            var manufacturerId = (ushort)12345;
            var manufacturerData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var btPropertiesMock = new Mock<IBluetoothDeviceProperties>();
            var sourceDeviceList = new List<IBluetoothDevice>() { btDeviceMock.Object};
            btPropertiesMock.Setup(p => p.Address).Returns(address);
            btPropertiesMock.Setup(p => p.Name).Returns(name);
            btPropertiesMock.Setup(p => p.GetManufacturerData()).Returns(new ManufacturerData() { Id = manufacturerId, Data = manufacturerData });
            btDeviceMock.Setup(d => d.GetProperties()).ReturnsAsync(btPropertiesMock.Object);
            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);            
            await bleReader.Scan(adapterName, 0);
            var deviceInfoList = await bleReader.GetAllDevices();

            Assert.AreEqual(sourceDeviceList.Count, deviceInfoList.Count, "Scanning did take expected time");
            Assert.AreEqual(name, deviceInfoList[0].Name, "Name is not correct");
            Assert.AreEqual(address, deviceInfoList[0].Address, "Address is not correct");
            Assert.AreEqual(manufacturerId, deviceInfoList[0].ManufacturerData.Id, "Manufacturer ID is not correct");
            Assert.AreEqual(manufacturerData, deviceInfoList[0].ManufacturerData.Data, "Manufacturer data is not correct");
        }

        [TestMethod]
        public async Task GetAllDevices_ManufacturerData_Null()
        {
            var adapterName = "adapter";
            var address = "11:22:33:44:55:66";
            var name = "some device name";            
            ManufacturerData manufacturerData = null;

            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var btPropertiesMock = new Mock<IBluetoothDeviceProperties>();
            var sourceDeviceList = new List<IBluetoothDevice>() { btDeviceMock.Object };
            btPropertiesMock.Setup(p => p.Address).Returns(address);
            btPropertiesMock.Setup(p => p.Name).Returns(name);
            btPropertiesMock.Setup(p => p.GetManufacturerData()).Returns(manufacturerData);
            btDeviceMock.Setup(d => d.GetProperties()).ReturnsAsync(btPropertiesMock.Object);
            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.Scan(adapterName, 0);
            var deviceInfoList = await bleReader.GetAllDevices();

            Assert.AreEqual(sourceDeviceList.Count, deviceInfoList.Count, "Scanning did take expected time");
            Assert.AreEqual(name, deviceInfoList[0].Name, "Name is not correct");
            Assert.AreEqual(address, deviceInfoList[0].Address, "Address is not correct");
            Assert.IsNull(manufacturerData, "Manufacturer data should be null");            
        }

        [TestMethod]
        public async Task GetManufacturerData_RuuviDataFound_True()
        {
            var adapterName = "adapter";
            var address = "11:22:33:44:55:66";
            var name = "some device name";
            var manufacturerId = (ushort)1177;
            string rawData = "05-12-FC-53-94-C3-7C-00-04-FF-FC-04-0C-AC-36-42-00-CD-CB-B8-33-4C-88-4F";            
            var manufacturerData = rawData.Split('-').Select(item => Convert.ToByte(item, 16)).ToArray();

            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var btPropertiesMock = new Mock<IBluetoothDeviceProperties>();
            var sourceDeviceList = new List<IBluetoothDevice>() { btDeviceMock.Object };
            btPropertiesMock.Setup(p => p.Address).Returns(address);
            btPropertiesMock.Setup(p => p.Name).Returns(name);
            btPropertiesMock.Setup(p => p.GetManufacturerData()).Returns(new ManufacturerData() { Id = manufacturerId, Data = manufacturerData });
            btDeviceMock.Setup(d => d.GetProperties()).ReturnsAsync(btPropertiesMock.Object);
            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.Scan(adapterName, 0);
            var ruuviTag = await bleReader.GetManufacturerData<RuuviTag>(address);

            Assert.IsNotNull(ruuviTag, "RuuviTag instance should not be null");           
        }

        [TestMethod]
        public async Task GetManufacturerData_NoRuuviAtAddress_NullReturned()
        {
            var adapterName = "adapter";
            var address = "11:22:33:44:55:66";
            var name = "some device name";
            var manufacturerId = (ushort)1177;
            string rawData = "05-12-FC-53-94-C3-7C-00-04-FF-FC-04-0C-AC-36-42-00-CD-CB-B8-33-4C-88-4F";
            var manufacturerData = rawData.Split('-').Select(item => Convert.ToByte(item, 16)).ToArray();

            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var btPropertiesMock = new Mock<IBluetoothDeviceProperties>();
            var sourceDeviceList = new List<IBluetoothDevice>() { btDeviceMock.Object };
            btPropertiesMock.Setup(p => p.Address).Returns(address);
            btPropertiesMock.Setup(p => p.Name).Returns(name);
            btPropertiesMock.Setup(p => p.GetManufacturerData()).Returns(new ManufacturerData() { Id = manufacturerId, Data = manufacturerData });
            btDeviceMock.Setup(d => d.GetProperties()).ReturnsAsync(btPropertiesMock.Object);
            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            IBleReader bleReader = new BleReader(btServiceMock.Object);
            await bleReader.Scan(adapterName, 0);
            var ruuviTag = await bleReader.GetManufacturerData<RuuviTag>("someaddress");
            
            Assert.IsNull(ruuviTag, "RuuviTag instance should not be null");
        }

        [TestMethod]
        [ExpectedException(typeof(DevicesNotScannedException), "Correct exception not thrown when accessing data without scanning devices first")]
        public async Task GetManufacturerData_ScanMade_False()
        {
            var btServiceMock = new Mock<IBluetoothService>();
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.GetAllDevices();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "ArgumentNullException was not thrown while MAC address was null")]
        public async Task GetManufacturerData_MacAddress_Null()
        {
            var btServiceMock = new Mock<IBluetoothService>();
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.GetManufacturerData<RuuviTag>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(UnsupportedDeviceTypeException), "Correct exception was not thrown while unsupported device type was passed")]
        public async Task GetManufacturerData_UnsupportedType_True()
        {
            var adapterName = "adapter";
            var address = "11:22:33:44:55:66";
            var name = "some device name";
            var manufacturerId = (ushort)1177;
            string rawData = "05-12-FC-53-94-C3-7C-00-04-FF-FC-04-0C-AC-36-42-00-CD-CB-B8-33-4C-88-4F";
            var manufacturerData = rawData.Split('-').Select(item => Convert.ToByte(item, 16)).ToArray();

            var btServiceMock = new Mock<IBluetoothService>();
            var btAdapterMock = new Mock<IBluetoothAdapter>();
            var btDeviceMock = new Mock<IBluetoothDevice>();
            var btPropertiesMock = new Mock<IBluetoothDeviceProperties>();
            var sourceDeviceList = new List<IBluetoothDevice>() { btDeviceMock.Object };
            btPropertiesMock.Setup(p => p.Address).Returns(address);
            btPropertiesMock.Setup(p => p.Name).Returns(name);
            btPropertiesMock.Setup(p => p.GetManufacturerData()).Returns(new ManufacturerData() { Id = manufacturerId, Data = manufacturerData });
            btDeviceMock.Setup(d => d.GetProperties()).ReturnsAsync(btPropertiesMock.Object);
            btAdapterMock.Setup(a => a.GetDevicesAsync()).ReturnsAsync(sourceDeviceList);
            btServiceMock.Setup(s => s.GetAdapter(adapterName)).ReturnsAsync(btAdapterMock.Object);
            var bleReader = new BleReader(btServiceMock.Object);
            await bleReader.Scan(adapterName, 0);
            var data = await bleReader.GetManufacturerData<BleReader>(address);            
        }
    }
}
