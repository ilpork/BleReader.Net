using System.Threading.Tasks;

namespace BleReaderNet.Wrapper
{
    public interface IBluetoothService
    {
        Task<IBluetoothAdapter> GetAdapterAsync(string name);
    }
}
