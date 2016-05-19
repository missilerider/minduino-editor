using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public class Label : Command
	{
		public String Name;

		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.AddLabel(Name);
		}
	}
}
