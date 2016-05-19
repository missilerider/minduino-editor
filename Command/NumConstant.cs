using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	class NumConstant : NumCommand
	{
		public DataType Value;

		public override void Write(BinaryCode bc)
		{
			bc.Write(0);
			bc.Write(Value.GetBytes());
		}
	}
}
