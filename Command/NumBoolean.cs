using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class NumBoolean : NumCommand
	{
		public Boolean Value = false;
		
		public override void Write(BinaryCode bc)
		{
			if (Value)
				bc.Write(Command.GetByte(0, 0, 0, 0, 0, 0, 1, 1));
			else
				bc.Write(Command.GetByte(0, 0, 0, 0, 0, 1, 0, 0));
		}
	}
}
