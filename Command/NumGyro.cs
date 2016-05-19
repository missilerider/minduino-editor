using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class NumGyro : NumCommand
	{
		public ReadAxis Axis;
		public ReadParameter Parameter;

		public enum ReadAxis { X, Y, Z };
		public enum ReadParameter { Accel, Gyro, Temperature };

		public override void Write(BinaryCode bc)
		{
			switch(Parameter)
			{
				case ReadParameter.Accel:
					switch(Axis)
					{
						case ReadAxis.X: bc.Write(25); break;
						case ReadAxis.Y: bc.Write(26); break;
						case ReadAxis.Z: bc.Write(27); break;
					}
					break;
				
				case ReadParameter.Gyro:
					switch(Axis)
					{
						case ReadAxis.X: bc.Write(28); break;
						case ReadAxis.Y: bc.Write(29); break;
						case ReadAxis.Z: bc.Write(30); break;
					}
					break;

				case ReadParameter.Temperature:
					bc.Write(31);
					break;
			}
		}
	}
}
