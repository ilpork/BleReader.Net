using HashtagChris.DotNetBlueZ;
using System.Linq;
using System.Threading.Tasks;

namespace BleReaderNet.Wrapper.DotNetBlueZ
{
    public class DotNetBlueZService : IBluetoothService
    {
        public async Task<IBluetoothAdapter> GetAdapter(string name)
        {
            IBluetoothAdapter adapter = null;
            if (name != null)
            {
                var dnbAdapter = await BlueZManager.GetAdapterAsync(name);
                adapter = new DotNetBlueZAdapter(dnbAdapter);
            }
            else
            {
                var adapters = await BlueZManager.GetAdaptersAsync();
                if (adapters.Count > 0)
                {
                    adapter = new DotNetBlueZAdapter(adapters.First());
                }
            }
            return adapter;
        }
    }
}
