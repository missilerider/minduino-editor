using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class NumArithmetic : NumCommand
	{
		public enum OperationType { Add = 0, Sub = 1, Multiply = 2, Divide = 3, Power = 4 };

		public NumCommand Oper1, Oper2;
		public OperationType Operation;

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			base.PreCompile(mc, Cmds);
			Oper1.PreCompile(mc, Cmds);
			Oper2.PreCompile(mc, Cmds);
		}

		public override void Write(BinaryCode bc)
		{
			bc.Write(Command.GetByte(0, 0, 0, 0, 0, 0, 0, 1));
			bc.Write((byte)Operation);
			bc.Write(Oper1);
			bc.Write(Oper2);
		}
	}
}
