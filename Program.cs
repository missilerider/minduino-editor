using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minduino_Editor.Command;
using System.Web;
using System.Threading;

namespace Minduino_Editor
{
    static class Program
    {
		public static SerialComm COM = new SerialComm();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			Dictionary<String, String> Ports = SerialComm.Ports;

			foreach(String S in Ports.Keys)
			{
				Console.WriteLine(S + " = " + Ports[S]);
			}

			//COM.Open("COM8");

			WebServer Srv = new WebServer();
			Srv.StartServer(1124, COM);

			while (true)
			{
				lock (COM)
				{
					String S = COM.ReadLine(TimeSpan.FromSeconds(2));
					if (S != null)
					{
						Console.WriteLine("<!" + S);
					}
				}

				float msg;

				while(COM.GetMessage(out msg))
				{
					//Console.WriteLine("Msg IN: " + msg);
				}
				//Console.WriteLine(COM.ReadLine());
			}
			 
			MicroCode mc = new MicroCode();

#if DEBUG

			if(!mc.Parse(@"
<xml xmlns=""http://www.w3.org/1999/xhtml"">
  <block type=""program_begin_event"" x=""73"" y=""72"">
    <statement name=""EVENT"">
      <block type=""controls_if"">
        <mutation elseif=""1""></mutation>
        <value name=""IF0"">
          <block type=""logic_boolean"">
            <field name=""BOOL"">TRUE</field>
          </block>
        </value>
        <statement name=""DO0"">
          <block type=""motor_reverse"">
            <field name=""POSITION"">A</field>
          </block>
        </statement>
        <value name=""IF1"">
          <block type=""logic_boolean"">
            <field name=""BOOL"">FALSE</field>
          </block>
        </value>
        <statement name=""DO1"">
          <block type=""motor_reverse"">
            <field name=""POSITION"">C</field>
          </block>
        </statement>
        <next>
          <block type=""motor_reverse"">
            <field name=""POSITION"">B</field>
          </block>
        </next>
      </block>
    </statement>
  </block>
</xml>
"))
			{
				Console.WriteLine("Se produjo un error al procesar el contenido del XML");
			}

			byte[] data = mc.Compile();
			Console.WriteLine("Total: " + data.Length + " bytes");

			PrintProgram(data);

			return;
#else

			// netsh http add urlacl http://+:1124/ user=Todos listen=yes
			HttpListener listener = new HttpListener();
			listener.Prefixes.Add("http://*:1124/");
			try
			{
				listener.Start();
			}
			catch (Exception e)
			{
				Console.Out.WriteLine("Excp");
				Console.Out.WriteLine(e.Message);
			}

			Console.Out.WriteLine("Espera");
			//TcpClient cli = listener.AcceptTcpClient();

			//Stream s = cli.GetStream();
			HttpListenerContext cont = listener.GetContext();
			Console.Out.WriteLine("Recibe solicitud...");
			HttpListenerRequest request = cont.Request;
			Dictionary<String, String> Params = new Dictionary<string, string>();
			using (var reader = new StreamReader(request.InputStream,
												 request.ContentEncoding))
			{
				Console.Out.WriteLine("Params");
				String contents = reader.ReadToEnd();
				Console.Out.WriteLine(contents);
				String[] parts = contents.Split('&');
				foreach(String S in parts)
				{
					String[] p2 = S.Split('=');

					String p3 = Uri.UnescapeDataString(p2[0]);
					String p4 = Uri.UnescapeDataString(p2[1].Replace('+',' '));

					p4 = WebUtility.HtmlDecode(p4);


					Params.Add(p3, p4);
					Console.Out.WriteLine("Fin");
				}
				Console.Out.WriteLine("Fin");
			}

			foreach (String S in Params.Keys)
				Console.WriteLine("Param " + S + " = \n" + Params[S]);

	
			//StreamWriter sw = new StreamWriter(cli);
			//sw.WriteLine("OK!");
			Console.Out.WriteLine("Fin");
			listener.Stop();
#endif
		}

		public static void PrintProgram(byte[] data)
		{
			Console.WriteLine("data[] = { ");
			foreach(byte b in data)
			{
				Console.Write(b.ToString() + ", ");
			}
			Console.WriteLine("\n0 };");
		}
    }
}
