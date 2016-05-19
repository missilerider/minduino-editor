using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public abstract class NumCommand
	{
		public abstract void Write(BinaryCode bc);
		public virtual void PreCompile(MicroCode mc, List<Command> Cmds) { }
	}
}
