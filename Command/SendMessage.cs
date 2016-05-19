using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class SendMessage : Command
	{
		public NumCommand Message = null;

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			base.PreCompile(mc, Cmds);
			Message.PreCompile(mc, Cmds);
		}

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 1, 1, 0, 0, 0, 0));
			bc.Write(Message);
		}
	}
}
