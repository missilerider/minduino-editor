using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Minduino_Editor
{
	public class WebServer
	{
		HttpListener listener = null;
		Thread th;
		SerialComm COM;

		public bool StartServer(ushort port, SerialComm COM)
		{
			this.COM = COM;

			// netsh http add urlacl http://+:1124/ user=Todos listen=yes
			listener = new HttpListener();
			listener.Prefixes.Add(String.Format("http://*:{0}/", port));
			try
			{
				listener.Start();
			}
			catch(Exception e)
			{
				Console.Out.WriteLine("Excp");
				Console.Out.WriteLine(e.Message);
				return false;
			}

			th = new Thread(Worker);
			th.Start();

			return true;
		}

		public void Worker()
		{
			while (true)
			{
				HttpListenerContext cont;

				try
				{
					cont = listener.GetContext();
				}
				catch
				{
					break;
				}

				HttpListenerRequest request = cont.Request;
				Dictionary<String, String> Params = new Dictionary<string, string>();
				using (var reader = new StreamReader(request.InputStream,
													 request.ContentEncoding))
				{
					Console.Out.WriteLine("Params");
					String contents = reader.ReadToEnd();
					Console.Out.WriteLine(contents);
					String[] parts = contents.Split('&');
					foreach (String S in parts)
					{
						if (S == "") continue;

						String[] p2 = S.Split('=');

						String p3 = Uri.UnescapeDataString(p2[0]);
						String p4 = Uri.UnescapeDataString(p2[1].Replace('+', ' '));

						p4 = WebUtility.HtmlDecode(p4);


						Params.Add(p3, p4);
						Console.Out.WriteLine("Fin");
					}
					Console.Out.WriteLine("Fin");
				}

				foreach (String S in Params.Keys)
					Console.WriteLine("Param " + S + " = \n" + Params[S]);
				if (request.Url.LocalPath == "/cgi" && request.HttpMethod == "POST")
				{
					ParameterizedThreadStart client = new ParameterizedThreadStart(DoCommand);
					(new Thread(client)).Start(new object[] { Params, cont.Response });
				}
				else
				{
					ParameterizedThreadStart client = new ParameterizedThreadStart(SendFile);
					(new Thread(client)).Start(new object[] { Params, cont.Response, request });
				}
			}
		}

		public void Stop()
		{
			listener.Stop();
		}

		public void SendFile(object p)
		{
			Dictionary<String, String> Params = (Dictionary<String, String>)((object[])p)[0];
			HttpListenerResponse Response = (HttpListenerResponse)((object[])p)[1];
			HttpListenerRequest Request = (HttpListenerRequest)((object[])p)[2];

			String f = "./html";
			if (Request.Url.LocalPath == "/") f += "/index.html";
			else f += Request.Url.LocalPath;

			try
			{
				Stream sr = new FileStream(f, FileMode.Open);
				sr.CopyTo(Response.OutputStream);
				sr.Close();
				Response.Close();
				Console.WriteLine("Se envia fichero " + f);
			}
			catch
			{
				Response.StatusCode = 404;
				Response.Close();
				Console.WriteLine("No se encuentra fichero " + f);
			}
		}

		public void DoCommand(object p)
		{
			Dictionary<String, String> Params = (Dictionary<String, String>)((object[])p)[0];
			HttpListenerResponse Response = (HttpListenerResponse)((object[])p)[1];
			String resp = null;

			if (!Params.ContainsKey("op"))
				resp = "{ code: 'error' }";
			else
				switch(Params["op"])
				{
					case "PROG":
						lock (COM)
						{
							if (!Params.ContainsKey("prog"))
							{
								resp = "{ code: 'error' }";
								break;
							}

							MicroCode mc = new MicroCode();
							mc.Parse(Params["prog"]);
							byte[] data = mc.Compile();
							Program.PrintProgram(data);

							COM.Write("PROG");
							if (COM.ReadLine(TimeSpan.FromSeconds(2)) != "PROG_OK")
							{
								Console.WriteLine("Programacion fallida, no PROG_OK");
								resp = "{ code: 'error' }";
								break;
							}

							COM.Write((ushort)data.Length);
							COM.Write(data);
							if (COM.ReadLine(TimeSpan.FromSeconds(2)) == "OK")
							{
								resp = "{ code: 'ok' }";
							}
							else
							{
								resp = "{ code: 'error' }";
							}
						}
						break;

					case "SAVE":
						if (!Params.ContainsKey("prog"))
						{
							resp = "{ code: 'error' }";
							break;
						}

						if (!Params.ContainsKey("filename"))
						{
							resp = "{ code: 'error' }";
							break;
						}

						try
						{
							if(!new DirectoryInfo("programs").Exists)
								new DirectoryInfo(".").CreateSubdirectory("programs");

							StreamWriter sw = new StreamWriter(@"programs\" + Params["filename"] + ".xml");
							sw.Write(Params["prog"]);
							sw.Close();
							resp = "{ code: 'ok' }";
						}
						catch
						{
							resp = "{ code: 'error' }";
						}
						break;

					case "LOAD":
						if (!Params.ContainsKey("filename"))
						{
							resp = "{ code: 'error' }";
							break;
						}

						try
						{
							StreamReader sr = new StreamReader(@"programs\" + Params["filename"] + ".xml");
							resp = sr.ReadToEnd();
							sr.Close();
						}
						catch
						{
							resp = "error";
						}
						break;

					case "STOP":
						lock (COM)
						{
							COM.Write("STOP");
							if (COM.ReadLine(TimeSpan.FromSeconds(2)) != "OK")
							{
								Console.WriteLine("Detencion fallida");
								resp = "{ code: 'error' }";
							}
							else
								resp = "{ code: 'ok' }";
						}
						break;

					case "PING":
						lock (COM)
						{
							COM.Write("PING");
							String S2 = COM.ReadLine(TimeSpan.FromSeconds(2));
							if (S2 == "PONG")
							{
								Console.WriteLine("Hello ok");
								resp = "{ code: 'ok' }";
							}
							else
							{
								Console.WriteLine("NO RESPONSE / ERROR");
								Console.WriteLine(S2);
								resp = "{ code: 'error' }";
							}
						}
						break;

					case "GET_PORTS":
						resp = "[";
						Dictionary<string, String> Ports = SerialComm.Ports;

						bool head = true;

						foreach(String S in Ports.Keys)
						{
							if(!head) resp += ",";
							else head = false;

							resp += "{port:'" + S + "', name:'" + Ports[S] + "'}";
						}

						resp += "]";
						break;

					case "SET_PORT":
						if (!Params.ContainsKey("port"))
						{
							resp = "{ code: 'error' }";
							break;
						}

						if (COM != null) COM.Close();
						
						if (COM.Open(Params["port"]))
							resp = "{ code: 'ok' }";
						else
							resp = "{ code: 'error' }";
						break;

					default:
						Console.WriteLine("No se entiende el comando " + Params["op"]);
						break;
				}

			if (resp != null)
			{
				Response.ContentType = "text/html";
				Console.WriteLine("Respuesta: " + resp);
				Response.Close(Encoding.UTF8.GetBytes(resp), true);
			}
			else
			{
				Response.StatusCode = 500;
				Response.Close();
			}
		}
	}
}
