using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduino_Editor.Command
{
	public enum MotorID
	{
		A, B, C, D
	}

	public abstract class Command
	{
		public virtual void PreCompile(MicroCode mc, List<Command> Cmds) { Cmds.Add(this); }
		public abstract void Compile(MicroCode mc, BinaryCode bc);
		public virtual void PostCompile(MicroCode mc, BinaryCode bc) { }

		public static byte GetByte(byte bit8, byte bit7, byte bit6, byte bit5, byte bit4, byte bit3, byte bit2, byte bit1)
		{
			return (byte)(bit1 + bit2 * 2 + bit3 * 4 + bit4 * 8 + bit5 * 16 + bit6 * 32 + bit7 * 64 + bit8 * 128);
		}

		public static byte[] GetBytes(String S)
		{
			byte[] data = new byte[S.Length];

			for (int i = 0; i < S.Length; i++)
				data[i] = Convert.ToByte(S[i]);

			return data;
		}

		public static byte[] GetBytes(ushort i)
		{
			return BitConverter.GetBytes(i);
		}
	}
}
