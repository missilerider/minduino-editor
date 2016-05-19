using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class EndTrigger : Command
	{
		public override void Compile(MicroCode mc, BinaryCode bc)
		{
			bc.Write(GetByte(0, 0, 0, 0, 1, 1, 0, 1));
		}
	}
}
