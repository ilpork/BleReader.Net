
using HashtagChris.DotNetBlueZ;
using System.Linq;
using System.Threading.Tasks;

namespace BleReaderNet.Wrapper.DotNetBlueZ
{
    public class DotNetBlueZDevice : IBluetoothDevice
    {
        IDevice1 _device;

        public string Address { get; set; }

        internal DotNetBlueZDevice(IDevice1 device)
        {
            _device = device;            
        }
        public async Task<IBluetoothDeviceProperties> GetProperties()
        {
            var dnbProperties = await _device.GetAllAsync();

            return new DotNetBlueZProperties(dnbProperties);
        }

        public async Task<ManufacturerData> GetManufacturerData()
        {
            var keyValuePair = (await _device.GetManufacturerDataAsync())?.FirstOrDefault();

            if (keyValuePair == null)
                return null;

            return new ManufacturerData() { Data = (byte[])keyValuePair.Value.Value, Id = keyValuePair.Value.Key };
        }
    }
}
