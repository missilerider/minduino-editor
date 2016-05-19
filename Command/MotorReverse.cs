using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class MotorReverse : Command
	{
		public MotorID Motor = MotorID.A;

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			Byte b1 = 0, b2 = 0;
			switch(Motor)
			{
				case MotorID.A: b1 = 0; b2 = 0; mc.UseMotor1 = true; break;
				case MotorID.B: b1 = 0; b2 = 1; mc.UseMotor2 = true; break;
				case MotorID.C: b1 = 1; b2 = 0; mc.UseMotor3 = true; break;
				case MotorID.D: b1 = 1; b2 = 1; mc.UseMotor4 = true; break;
			}
			bc.Write(GetByte(0, 0, 1, 0, b1, b2, 1, 0));
		}
	}
}
