using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Minduino_Editor.Command
{
	public class DataType
	{
		public float Value;

		public byte[] GetBytes()
		{
			return BitConverter.GetBytes(Value);
		}

		public DataType() { }

		public DataType(String S)
		{
			try
			{
				Value = float.Parse(S, new CultureInfo("en-US").NumberFormat);
			}
			catch
			{
				Value = 0;
			}
		}
	}
}
