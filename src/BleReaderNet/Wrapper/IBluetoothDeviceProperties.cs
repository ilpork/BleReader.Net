
namespace BleReaderNet.Wrapper
{
    public interface IBluetoothDeviceProperties
    {
        string Name { get; }
        string Address { get; }
        ManufacturerData GetManufacturerData();
    }
}
