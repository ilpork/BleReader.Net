using BleReaderNet.Device;
using BleReaderNet.Reader;
using BleReaderNet.Wrapper.DotNetBlueZ;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace BleNetApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IBleReader reader = new BleReader(new DotNetBlueZService());            
            int scanTimeSeconds = 5;

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine($"{assemblyName} {version}\n");
            Console.WriteLine("Scan Bluetooth LE devices and read their data");
            Console.WriteLine($"Usage: {assemblyName} [adapterName]\n");

            string adapterName = null;
            if (args.Length == 1)
            {
                adapterName = args[0];
            }
                            
            Console.WriteLine($"Scanning for {scanTimeSeconds} seconds...\n");
            await reader.Scan(adapterName, scanTimeSeconds);
            var deviceInfoList = await reader.GetAllDevices();

            foreach (var deviceInfo in deviceInfoList)
            {                
                var manufacturerDataString = (deviceInfo.ManufacturerData != null) ? $", Manufacturer ID: " +
                    $"{deviceInfo.ManufacturerData.Id}, Manufacturer data: " +
                    $"{BitConverter.ToString(deviceInfo.ManufacturerData.Data ?? new byte[] { })}" : "";
                
                Console.WriteLine($"Found device with address {deviceInfo.Address}: Name: {deviceInfo.Name ?? "<unknown>"}{manufacturerDataString}");

                if (deviceInfo.ManufacturerData?.Id == 1177)
                {
                    var ruuviData = await reader.GetManufacturerData<RuuviTag>(deviceInfo.Address);
                    Console.WriteLine($"RuuviTag sensor data: {JsonSerializer.Serialize(ruuviData, new JsonSerializerOptions() { WriteIndented = true })}");
                }             
            }
        }
    }
}
