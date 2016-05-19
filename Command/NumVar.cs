using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class NumVar : NumCommand
	{
		public String Variable;
		private byte VariableID;

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			base.PreCompile(mc, Cmds);
			VariableID = mc.GetVar(Variable);
		}
		
		public override void Write(BinaryCode bc)
		{
			bc.Write(Command.GetByte(0, 0, 0, 0, 0, 0, 1, 0));
			bc.Write(VariableID);
		}
	}
}
