using System.Threading.Tasks;

namespace BleReaderNet.Wrapper
{
    public interface IBluetoothService
    {
        Task<IBluetoothAdapter> GetAdapter(string name);
    }
}
