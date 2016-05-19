using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor
{
	public class BinaryCode
	{
		MemoryStream Data = new MemoryStream(1);
		Dictionary<string, ushort> Labels = new Dictionary<string, ushort>();

		public ushort Position
		{
			get { return (ushort)Data.Position; }
		}
	
		public void AddLabel(String Label)
		{
			Labels.Add(Label, Position);
		}

		public ushort GetLabel(String Label)
		{
			if (Labels.ContainsKey(Label))
				return Labels[Label];
			return 0;
		}

		public void Write(byte b)
		{
			Data.Write(new byte[] { b }, 0, 1);
		}

		public void Write(byte[] b)
		{
			Data.Write(b, 0, b.Length);
		}

		public void RewritePosition(ushort NewPosition)
		{
			if (NewPosition > Data.Length) NewPosition = 0;
			Data.Seek(NewPosition, SeekOrigin.Begin);
		}

		public byte[] GetBytes()
		{
			Data.Seek(0, SeekOrigin.Begin);
			byte[] buffer = new byte[Data.Length];
			Data.Read(buffer, 0, (int)Data.Length);
			return buffer;
		}

		internal void Write(Command.NumCommand Num)
		{
			Num.Write(this);
		}

/*		internal void Compile(List<Command.Command> Cmds, MicroCode mc, BinaryCode bc)
		{
			foreach(Command.Command C in Cmds)
			{
				C.Compile(mc, bc);
			}
		}*/
	}
}
