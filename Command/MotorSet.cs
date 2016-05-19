using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class MotorSet : Command
	{
		public MotorID Motor = MotorID.A;
		public NumCommand Speed = null;
		public bool Reverse = false;

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			base.PreCompile(mc, Cmds);
			Speed.PreCompile(mc, Cmds);
		}

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			byte b5 = 0, b6 = 0;
			switch(Motor)
			{
				case MotorID.A:
					b5 = 0; b6 = 0;
					mc.UseMotor1 = true;
					break;

				case MotorID.B:
					b5 = 0; b6 = 1;
					mc.UseMotor2 = true;
					break;

				case MotorID.C:
					b5 = 1; b6 = 0;
					mc.UseMotor3 = true;
					break;

				case MotorID.D:
					b5 = 1; b6 = 1;
					mc.UseMotor4 = true;
					break;
			}

			if(!Reverse)
				bc.Write(GetByte(0, 0, 1, 0, b5, b6, 0, 0));
			else 
				bc.Write(GetByte(0, 0, 1, 0, b5, b6, 0, 1));

			bc.Write(Speed);
		}
	}
}
