using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minduino_Editor.Command;

namespace Minduino_Editor
{
	public class EventProgrammer
	{
		public enum EventType
		{
			ProgramStart = 1,
			Gyroscope = 2, 
				// p1: Gyro=1, Accel=1; p2: X=1, Y=2, Z=3, ANY=4
			Touch = 3, 
				// p1: Touch A:1, B:2, ...; p2: State=(TOUCH=1|RELEASE=2|CLICK=3)
			Temperature = 4, 
				// p1: COMP GT=1,GE=2,EQ=3,LE=4,LT=5,NE=6; NParam1; 
			TemperatureRange = 5
				// NParam1: MIN; NParam2: MAX
		}

		public EventType Type;

		public byte Param1, Param2, Param3;
		public NumCommand NParam1, NParam2;

		public Label ExecutionLabel = null;

		private ushort Position;

		public void Compile(BinaryCode bc)
		{
			switch(Type)
			{
				case EventType.ProgramStart:
					bc.Write((byte)1);
					Position = bc.Position;
					bc.Write(0);
					bc.Write(0);
					break;

				case EventType.Gyroscope:
					bc.Write((byte)2);
					Position = bc.Position;
					bc.Write(0);
					bc.Write(0);
					bc.Write(Param1);
					bc.Write(Param2);
					bc.Write(NParam1);
					break;

				case EventType.Touch:
					bc.Write((byte)3);
					Position = bc.Position;
					bc.Write(0);
					bc.Write(0);
					bc.Write(Param1);
					bc.Write(Param2);
					break;

				case EventType.Temperature:
					bc.Write((byte)4);
					Position = bc.Position;
					bc.Write(0);
					bc.Write(0);
					bc.Write(Param1);
					bc.Write(NParam1);
					break;

				case EventType.TemperatureRange:
					bc.Write((byte)5);
					Position = bc.Position;
					bc.Write(0);
					bc.Write(0);
					bc.Write(NParam1);
					bc.Write(NParam2);
					break;
			}
		}

		public void PostCompile(MicroCode mc, BinaryCode bc)
		{
			ushort pos = bc.GetLabel(ExecutionLabel.Name);
			bc.RewritePosition(Position);
			bc.Write(Command.Command.GetBytes(pos));
		}

		public static void CompileFooter(BinaryCode bc)
		{
			bc.Write(255);
		}
	}
}
