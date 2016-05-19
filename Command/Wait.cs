using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class Wait : Command
	{
		public NumCommand Cents = null;

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			Cents.PreCompile(mc, Cmds);
			base.PreCompile(mc, Cmds);
		}

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 0, 0, 0, 0, 0, 1));
			bc.Write(Cents);
		}
	}
}
