using BleReaderNet.Device;
using BleReaderNet.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BleReaderNet.Reader
{
    /// <summary>
    /// Interface for reader used to read data from Bluetooth LE devices
    /// </summary>
    public interface IBleReader
    {
        /// <summary>
        /// Scan the available devices. Scan has to be done to get updated device data.
        /// </summary>
        /// <param name="adapterName">Bluetooth adapter name to be used for scanning (e.g. hci0). If null, then tries to use first found adapter</param>        
        /// <param name="scanDurationSeconds">How many seconds to scan for devices</param>
        /// <returns>Amount of IEDs found</returns>
        /// <exception cref="AdapterNotFoundException">Bluetooth adapter was not found</exception>
        Task<int> ScanAsync(string adapterName = null, int scanDurationSeconds = 10);

        /// <summary>
        /// Gets information about all devices that were found during latest scan
        /// </summary>
        /// <returns>List of device information</returns>
        /// <exception cref="DevicesNotScannedException">Devices have not been scanned before trying to get device information</exception>
        Task<IReadOnlyList<DeviceInfo>> GetAllDevicesAsync();

        /// <summary>
        /// Gets parsed manufacturer data of a device 
        /// </summary>
        /// <remarks>Supported types: <see cref="RuuviTag"/></remarks>        
        /// <param name="macAddress">MAC address of the device</param>
        /// <returns>Parsed data or null if no device of given type was found at given address</returns>
        /// <exception cref="DevicesNotScannedException">Devices have not been scanned before trying to read device information</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null</exception>
        Task<T> GetManufacturerDataAsync<T>(string macAddress);
    }
}