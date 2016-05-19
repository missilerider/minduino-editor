using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class Goto : Command
	{
		public String Label, LabelEnd;

		private ushort Position;

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 0, 0, 0, 0, 0, 0));
			Position = bc.Position;
			bc.Write(0); // Ubicacion dummy byte 1
			bc.Write(0); // Ubicacion dummy byte 2
		}

		public override void PostCompile(MicroCode mc, BinaryCode bc)
		{
			ushort pos = bc.GetLabel(Label);
			bc.RewritePosition(Position);
			bc.Write(GetBytes(pos));
			base.PostCompile(mc, bc);
		}
	}
}
