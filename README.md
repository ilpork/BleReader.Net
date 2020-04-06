# BleReader.Net 
[![Build Status](https://ilpork.visualstudio.com/GitHub/_apis/build/status/ilpork.BleReader.Net?branchName=master)](https://ilpork.visualstudio.com/GitHub/_build/latest?definitionId=4&branchName=master)

A simple wrapper made to read manufacturer data from Bluetooth LE devices using .NET. 

It currently supports Linux using DBus and BlueZ (using [DotNet-BlueZ](https://github.com/hashtagchris/DotNet-BlueZ)). In addition to providing raw manufacturer data, it provides parsed data for specific devices (supports only  [RuuviTag](https://ruuvi.com/ruuvitag-specs/) currently).

## Features
* Read data of a RuuviTag
* Get list of all available devices and their data (address, name, manufacturer ID and manufacturer data)

## Requirements
* Linux system with BlueZ 5.50 or later installed

## Installation
Available as NuGet package:
```
dotnet add package BleReader.Net
```

## Usage
Get RuuviTag data:
```
var adapterName = "hci0";
var scanDurationSeconds = 5;
var ruuviMacAddress = "12:34:56:78:90:AB";

IBleReader reader = new BleReader(new DotNetBlueZService());
await reader.ScanAsync(adapterName, scanDurationSeconds);
var ruuviTag = await reader.GetManufacturerDataAsync<RuuviTag>(ruuviMacAddress);
if (ruuviTag != null)
{
    var dataAsJson = JsonSerializer.Serialize(ruuviTag, new JsonSerializerOptions() { WriteIndented = true });
    Console.WriteLine($"Data: {dataAsJson}");
}
else
{
    Console.WriteLine("No RuuviTag found");
}
```
Output:
```
Data: {
  "DataFormat": 5,
  "Humidity": 27.495,
  "Temperature": 18.1,
  "AirPressure": 1006.94,
  "AccelerationX": -0.004,
  "AccelerationY": -0.012,
  "AccelerationZ": 1.032,
  "BatteryVoltage": 2.881,
  "TxPower": 4,
  "MovementCounter": 184,
  "MeasurementSequenceNumber": 12774,
  "MacAddress": "12-34-56-78-90-AB"
}
```
Get information of all found devices:
```
var adapterName = "hci0";
var scanDurationSeconds = 5;

IBleReader reader = new BleReader(new DotNetBlueZService());
await reader.ScanAsync(adapterName, scanDurationSeconds);
var deviceInfoList = await reader.GetAllDevicesAsync();

var firstDevice = deviceInfoList.FirstOrDefault();
if (firstDevice != null)
{
    var bytesAsString = BitConverter.ToString(firstDevice.ManufacturerData?.Data);
    Console.WriteLine($"Bluetooth device with address {firstDevice.Address} found");
    Console.WriteLine($"Manufacturer ID = {firstDevice.ManufacturerData?.Id}");
    Console.WriteLine($"Manufacturer data = {bytesAsString}");
}
else
{
    Console.WriteLine("No devices found");
}
```

Output:
```
Bluetooth device with address 12:34:56:78:90:AB found
Manufacturer ID = 1177
Manufacturer data = 05-0E-24-2A-F6-C6-06-FF-FC-FF-F4-04-08-A0-36-B8-31-E6-12-34-56-78-90-AB
```

## Contribute
You can contribute by submitting bugs, feature requests or pull requests (like implementing support for other device types).