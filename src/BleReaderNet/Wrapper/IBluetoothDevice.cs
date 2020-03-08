using System.Threading.Tasks;

namespace BleReaderNet.Wrapper
{
    public interface IBluetoothDevice
    {
        Task<IBluetoothDeviceProperties> GetProperties();
        Task<ManufacturerData> GetManufacturerData();
    }
}
