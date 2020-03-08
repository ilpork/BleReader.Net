
using HashtagChris.DotNetBlueZ;
using HashtagChris.DotNetBlueZ.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BleReaderNet.Wrapper.DotNetBlueZ
{
    public class DotNetBlueZAdapter : IBluetoothAdapter
    {
        private IAdapter1 _adapter;

        public DotNetBlueZAdapter(IAdapter1 adapter)
        {
            _adapter = adapter;
        }

        public async Task<IReadOnlyList<IBluetoothDevice>> GetDevicesAsync()
        {
            var devices = new List<IBluetoothDevice>();
            var dnbDeviceList = await _adapter.GetDevicesAsync();

            foreach (var dnbDevice in dnbDeviceList)
            {
                devices.Add(new DotNetBlueZDevice(dnbDevice));
            }
            return devices;
        }

        public async Task StartDiscoveryAsync()
        {
            await _adapter.StartDiscoveryAsync();
        }

        public async Task StopDiscoveryAsync()
        {
            await _adapter.StopDiscoveryAsync();
        }
    }
}
