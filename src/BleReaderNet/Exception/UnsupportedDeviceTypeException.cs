using System;
using System.Collections.Generic;
using System.Text;

namespace BleReaderNet.Exception
{
    public class UnsupportedDeviceTypeException : System.Exception
    {
        public UnsupportedDeviceTypeException(string message) : base(message) { }
    }
}
