using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class LoopFor : Command
	{
		public String Variable;
		private byte VariableID;

		public NumCommand From = null, To = null, Step = null;

		String ExitLabel;

		ushort Position;

		public List<Command> Do = new List<Command>();

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			// Set [Var] = [From]
			// Label "ForBegin"
			// If [Var] > [To]
			//   Goto ForEnd
			// Do
			//   { Comandos }
			// [Var] += [Step]
			// Goto "ForBegin"
			// Label "ForEnd"

			VariableID = mc.GetVar(Variable);

			Label l1 = mc.AddLabel();
			Label l2 = mc.AddLabel();
			Goto g = new Goto();
			g.Label = l1.Name;
			g.LabelEnd = l2.Name;
			ExitLabel = g.LabelEnd;

			l1.PreCompile(mc, Cmds);
			base.PreCompile(mc, Cmds);

			if (Do != null)
			{
				foreach(Command C in Do)
					C.PreCompile(mc, Cmds);
			}

			g.PreCompile(mc, Cmds);
			l2.PreCompile(mc, Cmds);

			From.PreCompile(mc, Cmds);
			To.PreCompile(mc, Cmds);
			Step.PreCompile(mc, Cmds);
		}

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 0, 0, 1, 0, 0, 0));
			bc.Write(VariableID);
			Position = bc.Position;
			bc.Write(0);
			bc.Write(0);
			bc.Write(From);
			bc.Write(To);
			bc.Write(Step);
		}

		public override void PostCompile(MicroCode mc, BinaryCode bc)
		{
			ushort pos = bc.GetLabel(ExitLabel);
			bc.RewritePosition(Position);
			bc.Write(GetBytes(pos));
		}
	}
}
