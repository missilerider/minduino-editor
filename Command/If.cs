using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class If : Command
	{
		public List<NumCommand> Conditions = new List<NumCommand>();
		public List<List<Command>> Actions = new List<List<Command>>();

		private String DestinationLabel;
		private ushort Position;
		private bool Compiled = false;

		public If() { }

		public If(bool Compiled) { this.Compiled = Compiled; }

		public override void PreCompile(MicroCode mc, List<Command> Cmds)
		{
			for (int n = 0; n < Conditions.Count; n++)
			{
				Label l = mc.AddLabel();
				If i = new If(true);
				i.Actions.Add(Actions[n]);
				i.Conditions.Add(Conditions[n]);
				i.DestinationLabel = l.Name;

				Cmds.Add(i);
				l.PreCompile(mc, Cmds);
			}
		}
		
		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 0, 0, 1, 0, 1, 1));

			Position = bc.Position;
			bc.Write(0); // Ubicacion dummy byte 1
			bc.Write(0); // Ubicacion dummy byte 2

			bc.Write(Conditions[0]);
			foreach (Command C in Actions[0])
				C.Compile(mc, bc);
		}

		public override void PostCompile(MicroCode mc, BinaryCode bc)
		{
			ushort pos = bc.GetLabel(DestinationLabel);
			bc.RewritePosition(Position);
			bc.Write(GetBytes(pos));
			base.PostCompile(mc, bc);
		}
	}
}
