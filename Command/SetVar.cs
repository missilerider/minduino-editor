using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class SetVar : Command
	{
		public String Variable;
		private byte VariableID;
		public NumCommand Value;

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			base.PreCompile(mc, Cmds);
			VariableID = mc.GetVar(Variable);
			Value.PreCompile(mc, Cmds);
		}
		
		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 0, 0, 0, 0, 1, 1));
			bc.Write(VariableID);
			bc.Write(Value);
		}
	}
}
