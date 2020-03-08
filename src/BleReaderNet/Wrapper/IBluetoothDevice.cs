using System.Threading.Tasks;

namespace BleReaderNet.Wrapper
{
    public interface IBluetoothDevice
    {
        Task<IBluetoothDeviceProperties> GetPropertiesAsync();
        Task<ManufacturerData> GetManufacturerDataAsync();
    }
}
