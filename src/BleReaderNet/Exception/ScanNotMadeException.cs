namespace BleReaderNet.Exception
{
    /// <summary>
    /// Exception used when trying to access data with scanning available the devices
    /// </summary>
    public class DevicesNotScannedException : System.Exception
    {
        public DevicesNotScannedException(string message) : base(message) { }
    }
}
