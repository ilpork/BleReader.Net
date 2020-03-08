using BleReaderNet.Device;
using BleReaderNet.Exception;
using BleReaderNet.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BleReaderNet.Reader
{
    /// <summary>
    /// Provides functionality to scan and read manufacturer data of Bluetooth LE devices
    /// </summary>
    public class BleReader : IBleReader
    {
        public const int DefaultScanDurationSeconds = 10;        
        private readonly IBluetoothService _bluetoothService;
        private IReadOnlyList<IBluetoothDevice> _deviceList = null;

        public BleReader(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<DeviceInfo>> GetAllDevicesAsync()
        {
            if (_deviceList == null)
            {
                throw new DevicesNotScannedException("Scan the available devices before trying to access the data");
            }

            var deviceInfoList = new List<DeviceInfo>();

            foreach (var device in _deviceList)
            {
                var properties = await device.GetPropertiesAsync();                               

                deviceInfoList.Add(new DeviceInfo()
                {
                    Name = properties.Name,
                    Address = properties.Address,
                    ManufacturerData = properties.GetManufacturerData()
                });
            }
            return deviceInfoList;
        }

        /// <inheritdoc/>
        public async Task<int> ScanAsync(string adapterName = null, int scanDurationSeconds = DefaultScanDurationSeconds)
        {
            var adapter = await _bluetoothService.GetAdapterAsync(adapterName);

            if (adapter == null)
            {
                throw new AdapterNotFoundException($"Adapter '{adapterName}' was not found");
            }

            await adapter.StartDiscoveryAsync();
            await Task.Delay(TimeSpan.FromSeconds(scanDurationSeconds));
            await adapter.StopDiscoveryAsync();
            _deviceList = await adapter.GetDevicesAsync();            

            return _deviceList.Count;
        }

        /// <inheritdoc cref="IBleReader"/>
        public async Task<T> GetManufacturerDataAsync<T>(string macAddress)
        {
            var manufacturerData = await ReadDeviceDataAsync(macAddress);

            if (manufacturerData != null)
            {
                if (typeof(T) == typeof(RuuviTag))
                {
                    return (T)Convert.ChangeType(RuuviTag.Parse(manufacturerData.Data), typeof(T));
                }
                // Add support for parsing data of other device types
                else
                {
                    throw new UnsupportedDeviceTypeException($"Support for getting manufacturer data of type '{typeof(T).ToString()}' has not been implemented");
                }                
            }
            
            return default(T);
        }

        private async Task<ManufacturerData> ReadDeviceDataAsync(string macAddress)
        {
            if (macAddress == null)
            {
                throw new ArgumentNullException(nameof(macAddress));
            }

            if (_deviceList == null)
            {
                throw new DevicesNotScannedException("Scan the available devices before trying to access the data");
            }

            foreach (var device in _deviceList)
            {
                var properties = await device.GetPropertiesAsync();

                if (properties.Address.Equals(macAddress))
                {
                    return properties.GetManufacturerData();                    
                }
            }
            return null;
        }
    }
}
