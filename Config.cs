using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Minduino_Editor
{
    public static class Config
    {
		public static String[] Ports = new String[] {};

        public static void RefreshCOMPorts()
        {
			Ports = SerialPort.GetPortNames();
        }
    }
}
