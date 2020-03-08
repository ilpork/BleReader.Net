using System;
using System.Collections.Generic;
using System.Text;

namespace BleReaderNet.Exception
{
    public class UnsupportedDataFormatException : System.Exception
    {
        public int DataFormat { get; set; }

        public UnsupportedDataFormatException(string message, int dataFormat) : base(message)
        {
            DataFormat = dataFormat;
        }

    }
}
