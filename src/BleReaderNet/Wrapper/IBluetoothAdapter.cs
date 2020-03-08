using System.Collections.Generic;
using System.Threading.Tasks;

namespace BleReaderNet.Wrapper
{
    public interface IBluetoothAdapter
    {
        Task StartDiscoveryAsync();
        Task StopDiscoveryAsync();
        Task<IReadOnlyList<IBluetoothDevice>> GetDevicesAsync();
    }
}
