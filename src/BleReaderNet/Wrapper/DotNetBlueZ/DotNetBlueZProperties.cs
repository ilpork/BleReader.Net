using HashtagChris.DotNetBlueZ;
using System.Collections.Generic;
using System.Linq;

namespace BleReaderNet.Wrapper.DotNetBlueZ
{
    public class DotNetBlueZProperties : IBluetoothDeviceProperties
    {
        private readonly Device1Properties _dnbProperties;
        public string Address => _dnbProperties.Address;
        public string Name => _dnbProperties.Name;

        public DotNetBlueZProperties(Device1Properties properties)
        {
            _dnbProperties = properties;
        }

        public ManufacturerData GetManufacturerData()
        {
            if (_dnbProperties.ManufacturerData != null)
            {
                var data = _dnbProperties.ManufacturerData.FirstOrDefault();
                if (!data.Equals(default(KeyValuePair<ushort, object>)))
                {
                    return new ManufacturerData() { Data = (byte[])data.Value, Id = data.Key };
                }
            }

            return null;
        }
    }
}
