using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Minduino_Editor
{
	public class SerialComm
	{
		public SerialPort COM = null;

		private List<float> Messages = new List<float>();

		public byte[] Buffer = new byte[2048];
		public int BufferSize = 0;

		public bool GetMessage(out float Msg)
		{
			if (!IsOpen) { Msg = 0; return false; }
			lock(COM)
			{
				if (Messages.Count > 0)
				{
					Msg = Messages[0];
					Messages.RemoveAt(0);
					return true;
				}
				else
					Msg = 0.0f;
					return false;
			}
		}

		public bool IsOpen
		{
			get
			{
				if (COM == null) return false;
				return COM.IsOpen;
			}
		}

		public static Dictionary<String, String> Ports
		{
			get
			{
				return BuildPortNameHash(SerialPort.GetPortNames());
			}
		}

		public SerialComm()
		{

		}

		public bool Open(String Name)
		{
			Close();
			COM = new SerialPort(Name, 9600);

			try
			{
				COM.Open();
			}
			catch
			{
				COM = null;
				return false;
			}

			return true;
		}

		public void Close()
		{
			try
			{
				if (COM != null)
					COM.Close();
			}
			catch { }
			
			COM = null;
		}

		public void Write(byte b)
		{
			if(IsOpen)
				lock(COM)
					COM.Write(new byte[] { b }, 0, 1);
		}

		public void Write(byte[] b)
		{
			if (IsOpen)
				lock(COM)
					COM.Write(b, 0, b.Length);
		}

		public void Write(ushort i)
		{
			Write(Command.Command.GetBytes(i));
		}

		public String ReadLine()
		{
			if (IsOpen)
			{
				FillBuffer();

				String S = ReadLineBuffer();
				while (S != null ? S.StartsWith("MSG:") : false)
				{
					float f;

					try
					{
						f = Convert.ToSingle(S.Substring(4), CultureInfo.InvariantCulture.NumberFormat);

						Messages.Add(f);
					}
					finally
					{
						S = ReadLineBuffer();
					}
				}

				return S;
			}
			else return null;
		}

		public String ReadLine(TimeSpan MaxWait)
		{
			if(!IsOpen) return null;

			String S;
			do
			{
				S = ReadLine();
				if (S == null) { Thread.Sleep(50); MaxWait -= TimeSpan.FromMilliseconds(50); }
			} while (S == null && MaxWait > TimeSpan.FromMilliseconds(0));

			return S;
		}
	
		private void FillBuffer()
		{
			lock (COM)
			{
				try
				{
					int b = COM.BytesToRead;
					if (b > 0)
					{
						COM.Read(Buffer, BufferSize, b);
						BufferSize += b;
					}
				}
				catch { }
			}
		}

		private int BufferContainsLine()
		{
			int i = 0;
			while(i < BufferSize)
			{
				if (Buffer[i] == '\r' || Buffer[i] == '\n')
				{
					while ((Buffer[i] == '\r' || Buffer[i] == '\n') && i < BufferSize) i++;
					return i;
				}
				i++;
			}
			return -1;
		}
		
		private String ReadLineBuffer()
		{
			int i = 0;
			while (i < BufferSize)
			{
				if (Buffer[i] == '\r' || Buffer[i] == '\n')
				{
					while ((Buffer[i] == '\r' || Buffer[i] == '\n') && i < BufferSize) i++;

					String S = Encoding.UTF8.GetString(Buffer, 0, i).TrimEnd('\r', '\n');
					byte[] Buffer2 = new byte[2048];
					Array.Copy(Buffer, i, Buffer2, 0, BufferSize - i);
					Buffer = Buffer2;
					BufferSize -= i;

					Console.WriteLine("<<" + S);

					return S;
				}
				i++;
			}
			return null;
		}

		public byte[] Read(ushort DataLen)
		{
			if (!IsOpen) return null;

			byte[] ret = new byte[DataLen];

			lock (COM)
			{
				while (COM.BytesToRead < DataLen)
					Thread.Sleep(50);

				COM.Read(ret, 0, DataLen);
			}

			Console.WriteLine("<<(bin " + ret.Length + " bytes)");

			return ret;
		}

		internal void Write(String S)
		{
			if (IsOpen)
			{
				Write(Encoding.UTF8.GetBytes(S));
				Console.WriteLine("[>>]" + S);
			}
		}

		#region Port Names

		/// <summary>
		/// Begins recursive registry enumeration
		/// </summary>
		/// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
		/// <returns>a hashtable mapping Friendly names to non-friendly port values</returns>
		private static Dictionary<string, string> BuildPortNameHash(string[] portsToMap)
		{
			Dictionary<string, string> oReturnTable = new Dictionary<string, string>();
			MineRegistryForPortName("SYSTEM\\CurrentControlSet\\Enum", oReturnTable, portsToMap);
			return oReturnTable;
		}
		/// <summary>
		/// Recursively enumerates registry subkeys starting with startKeyPath looking for 
		/// "Device Parameters" subkey. If key is present, friendly port name is extracted.
		/// </summary>
		/// <param name="startKeyPath">the start key from which to begin the enumeration</param>
		/// <param name="targetMap">dictionary that will get populated with 
		/// nonfriendly-to-friendly port names</param>
		/// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
		private static void MineRegistryForPortName(string startKeyPath, Dictionary<string, string> targetMap,
			string[] portsToMap)
		{
			if (targetMap.Count >= portsToMap.Length)
				return;
			using (RegistryKey currentKey = Registry.LocalMachine)
			{
				try
				{
					using (RegistryKey currentSubKey = currentKey.OpenSubKey(startKeyPath))
					{
						string[] currentSubkeys = currentSubKey.GetSubKeyNames();
						if (currentSubkeys.Contains("Device Parameters") &&
							startKeyPath != "SYSTEM\\CurrentControlSet\\Enum")
						{
							object portName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
								startKeyPath + "\\Device Parameters", "PortName", null);
							if (portName == null ||
								portsToMap.Contains(portName.ToString()) == false)
								return;
							object friendlyPortName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
								startKeyPath, "FriendlyName", null);
							string friendlyName = "N/A";
							if (friendlyPortName != null)
								friendlyName = friendlyPortName.ToString();
							if (friendlyName.Contains(portName.ToString()) == false)
								friendlyName = string.Format("{0} ({1})", friendlyName, portName);
							targetMap[portName.ToString()] = friendlyName;
						}
						else
						{
							foreach (string strSubKey in currentSubkeys)
								MineRegistryForPortName(startKeyPath + "\\" + strSubKey, targetMap, portsToMap);
						}
					}
				}
				catch (Exception)
				{
					//Console.WriteLine("Error accessing key '{0}'.. Skipping..", startKeyPath);
				}
			}
		}

		#endregion
	}
}
