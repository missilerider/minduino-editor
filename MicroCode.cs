using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Minduino_Editor.Command;

namespace Minduino_Editor
{
	public class MicroCode
	{
		public Dictionary<EventProgrammer, List<Command.Command>> Events = new Dictionary<EventProgrammer, List<Command.Command>>();

		Dictionary<String, byte> Variables = new Dictionary<string, byte>();

		public bool UseGyro = false, UseCompass = false, UseMotor1 = false, UseMotor2 = false, UseMotor3 = false, UseMotor4 = false;

		public MicroCode() { }
		
		public MicroCode(MicroCode Original)
		{
			foreach (String S in Original.Variables.Keys)
				Variables.Add(S, Original.Variables[S]);
		}

		public byte GetVar(String VarName)
		{
			if (Variables.ContainsKey(VarName)) return Variables[VarName];
			Variables.Add(VarName, (byte)Variables.Count);
			return (byte)(Variables.Count - 1);
		}

		public Label AddLabel()
		{
			Label l = new Label();
			l.Name = Guid.NewGuid().ToString();
			return l;
		}

		public bool Parse(String XmlCode)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(XmlCode);
			XmlNode First = xmlDoc.FirstChild;
//			byte param1 = 0, param2, param3;

			if (First.Name != "xml")
				return false;

			XmlNode Node;
			Label l;
			Goto g;
//			NumCommand nparam1, nparam2;

			List<Command.Command> Cmds = null;

			EventProgrammer ep = null;

			foreach(XmlNode Block in First.ChildNodes)
			{
				switch(Block.Attributes["type"].Value)
				{
					case "program_begin_event":
						Cmds = new List<Command.Command>();
						ep = new EventProgrammer();

						ep.Type = EventProgrammer.EventType.ProgramStart;

						l = new Label();
						l.Name = "PROGRAM_INIT";
						Cmds.Add(l);
						ep.ExecutionLabel = l;

						if (Block.FirstChild.Name != "statement" || Block.FirstChild.Attributes["name"].Value != "EVENT")
							return false;

						Node = Block.FirstChild.FirstChild;
						if (Node == null) break;

						if(!ParseBlock(Node, Cmds))
							return false;

						g = new Goto();
						g.Label = "PROGRAM_INIT";
						Cmds.Add(g);

						Events.Add(ep, Cmds);
						break;

					case "sensor_temperature_event_range":
						Cmds = new List<Command.Command>();
						ep = new EventProgrammer();

						ep.Type = EventProgrammer.EventType.TemperatureRange;

						l = AddLabel();
						Cmds.Add(l);
						ep.ExecutionLabel = l;

						Node = Block.FirstChild;
						if (Node == null) break;

						if (Node.Name != "value" || Node.Attributes["name"].Value != "PARAM_MIN")
							return false;

						ep.NParam1 = ParseNumber(Node.FirstChild);

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "value" || Node.Attributes["name"].Value != "PARAM_MAX")
							return false;

						ep.NParam2 = ParseNumber(Node.FirstChild);

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "statement" || Node.Attributes["name"].Value != "EVENT")
							return false;

						if (!ParseBlock(Node.FirstChild, Cmds))
							return false;

						Cmds.Add(new EndTrigger());

						Events.Add(ep, Cmds);
						UseGyro = true;
						break;

					case "sensor_gyro_event":
						Cmds = new List<Command.Command>();
						ep = new EventProgrammer();

						ep.Type = EventProgrammer.EventType.Gyroscope;

						l = AddLabel();
						Cmds.Add(l);
						ep.ExecutionLabel = l;

						Node = Block.FirstChild;
						if (Node == null) break;

						if (Node.Name != "field" || Node.Attributes["name"].Value != "COMP")
							return false;
						
						switch(Block.FirstChild.InnerText)
						{
							case "GYRO": ep.Param1 = 1; break;
							case "ACCEL": ep.Param1 = 2; break;
						}

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "field" || Node.Attributes["name"].Value != "AXIS")
							return false;

						switch (Node.InnerText)
						{
							case "X": ep.Param2 = 1; break;
							case "Y": ep.Param2 = 2; break;
							case "Z": ep.Param2 = 3; break;
							case "ANY": ep.Param2 = 4; break;
						}

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "value" || Node.Attributes["name"].Value != "PARAM")
							return false;

						ep.NParam1 = ParseNumber(Node.FirstChild);

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "statement" || Node.Attributes["name"].Value != "EVENT")
							return false;

						if (!ParseBlock(Node.FirstChild, Cmds))
							return false;

						Cmds.Add(new EndTrigger());

						Events.Add(ep, Cmds);
						UseGyro = true;
						break;

					case "sensor_temperature_event":
						Cmds = new List<Command.Command>();
						ep = new EventProgrammer();

						ep.Type = EventProgrammer.EventType.Temperature;

						l = AddLabel();
						Cmds.Add(l);
						ep.ExecutionLabel = l;

						Node = Block.FirstChild;
						if (Node == null) break;

						if (Node.Name != "field" || Node.Attributes["name"].Value != "COMP")
							return false;

						switch (Block.FirstChild.InnerText)
						{
							case "GT": ep.Param1 = 1; break;
							case "GE": ep.Param1 = 2; break;
							case "EQ": ep.Param1 = 3; break;
							case "LE": ep.Param1 = 4; break;
							case "LT": ep.Param1 = 5; break;
							case "NE": ep.Param1 = 6; break;
						}

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "value" || Node.Attributes["name"].Value != "PARAM")
							return false;

						ep.NParam1 = ParseNumber(Node.FirstChild);

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "statement" || Node.Attributes["name"].Value != "EVENT")
							return false;

						if (!ParseBlock(Node.FirstChild, Cmds))
							return false;

						Cmds.Add(new EndTrigger());

						Events.Add(ep, Cmds);
						UseGyro = true;
						break;

					case "sensor_touch_event":
						Cmds = new List<Command.Command>();
						ep = new EventProgrammer();

						ep.Type = EventProgrammer.EventType.Touch;

						l = AddLabel();
						Cmds.Add(l);
						ep.ExecutionLabel = l;

						Node = Block.FirstChild;
						if (Node == null) break;

						if (Node.Name != "field" || Node.Attributes["name"].Value != "SENSOR")
							return false;

						switch (Block.FirstChild.InnerText)
						{
							case "A": ep.Param1 = 1; break;
							case "B": ep.Param1 = 2; break;
							case "C": ep.Param1 = 3; break;
							case "D": ep.Param1 = 4; break;
							case "E": ep.Param1 = 5; break;
							case "F": ep.Param1 = 6; break;
							case "G": ep.Param1 = 7; break;
							case "H": ep.Param1 = 8; break;
						}

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "field" || Node.Attributes["name"].Value != "STATE")
							return false;

						switch (Block.FirstChild.InnerText)
						{
							case "TOUCH": ep.Param2 = 1; break;
							case "RELEASE": ep.Param2 = 2; break;
							case "CLICK": ep.Param2 = 3; break;
						}

						Node = Node.NextSibling;
						if (Node == null) break;

						if (Node.Name != "statement" || Node.Attributes["name"].Value != "EVENT")
							return false;

						if (!ParseBlock(Node.FirstChild, Cmds))
							return false;

						Cmds.Add(new EndTrigger());

						Events.Add(ep, Cmds);
						break;

					default:
						Console.WriteLine("No se conoce el evento tipo " + Block.Attributes["type"].Value);
						return false;
				}
			}

			return true;
		}
		
		/// <summary>
		/// Transforma el MicroCode en byte[]
		/// </summary>
		/// <returns></returns>
		public byte[] Compile()
		{
			MicroCode output = new MicroCode(this);

			foreach(EventProgrammer ev in Events.Keys)
				output.Events.Add(ev, new List<Command.Command>());

			foreach (EventProgrammer ev in Events.Keys)
			{
				foreach (Command.Command C in Events[ev])
					C.PreCompile(output, output.Events[ev]);
			}

			BinaryCode bc = new BinaryCode();

			// Event header
			if (UseGyro) bc.Write(129);
			if (UseCompass) bc.Write(130);
			if (UseMotor1) bc.Write(131);
			if (UseMotor2) bc.Write(132);
			if (UseMotor3) bc.Write(133);
			if (UseMotor4) bc.Write(134);

			foreach (EventProgrammer ev in Events.Keys)
				ev.Compile(bc);

			// Variable header
			bc.Write(128);
			bc.Write((byte)output.Variables.Count);


			EventProgrammer.CompileFooter(bc);

			foreach (EventProgrammer ev in output.Events.Keys)
			{
				foreach (Command.Command C in output.Events[ev])
					C.Compile(output, bc);
			}

			foreach (EventProgrammer ev in Events.Keys)
				ev.PostCompile(output, bc);

			foreach (EventProgrammer ev in output.Events.Keys)
				foreach (Command.Command C in output.Events[ev])
					C.PostCompile(output, bc);

			return bc.GetBytes();
		}

		private bool ParseBlock(XmlNode Block, List<Command.Command> Commands)
		{
			XmlNode Child;

			if (Block == null) return false;

			if(Block.Name != "block" && Block.Name != "shadow") return false;
			switch(Block.Attributes["type"].Value)
			{
				case "operation_wait":
					Child = Block.FirstChild;
					if(Child.Name != "value" || Child.Attributes["name"].Value != "WAIT") return false;

					Wait w1 = new Wait();
					w1.Cents = ParseNumber(Child.FirstChild);
					if (w1.Cents == null) return false;
					Commands.Add(w1);

					Child = Child.NextSibling;
					if(Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

				case "send_message":
					Child = Block.FirstChild;
					if(Child.Name != "value" || Child.Attributes["name"].Value != "MESSAGE") return false;

					SendMessage send = new SendMessage();
					send.Message = ParseNumber(Child.FirstChild);
					if (send.Message == null) return false;
					Commands.Add(send);

					Child = Child.NextSibling;
					if(Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

/*				case "controls_repeat_ext":
					LoopFor loopFor = new LoopFor();
					Child = Block.FirstChild;
					if(Child.Name != "value" || Child.Attributes["name"].Value != "TIMES") return false;
					loopFor.Times = CompileNumber(Child.FirstChild);
					if (loopFor.Times == null) return false;

					Child = Child.NextSibling;
					if(Child.Name != "statement" || Child.Attributes["name"].Value != "DO") return false;
					if (!CompileBlock(Child.FirstChild, loopFor.Do)) return false;
					
					Commands.Add(loopFor);

					Child = Child.NextSibling;
					if(Child != null)
						if (Child.Name == "next")
							if (!CompileBlock(Child.FirstChild, Commands)) return false;
					break;*/

				case "controls_for":
					LoopFor loopFor = new LoopFor();
					Child = Block.FirstChild;
					if (Child == null) return false;
					if (Child.Name != "field" || Child.Attributes["name"].Value != "VAR") return false;
					loopFor.Variable = Child.InnerText;

					Child = Child.NextSibling;
					if (Child == null) return false;
					if (Child.Name != "value" || Child.Attributes["name"].Value != "FROM") return false;
					loopFor.From = ParseNumber(Child.FirstChild);

					Child = Child.NextSibling;
					if (Child == null) return false;
					if (Child.Name != "value" || Child.Attributes["name"].Value != "TO") return false;
					loopFor.To = ParseNumber(Child.FirstChild);

					Child = Child.NextSibling;
					if (Child == null) return false;
					if (Child.Name != "value" || Child.Attributes["name"].Value != "BY") return false;
					loopFor.Step = ParseNumber(Child.FirstChild);

					Child = Child.NextSibling;
					if (Child != null) // No existe "DO"
					{
						if (Child.Name != "statement" || Child.Attributes["name"].Value != "DO") return false;
						if (!ParseBlock(Child.FirstChild, loopFor.Do)) return false;
					}
					else loopFor.Do = null;

					Commands.Add(loopFor);

					Child = Child.NextSibling;
					if (Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

				case "controls_if":
					List<Command.Command> cmdList;
					If cmdIf = new If();
					Child = Block.FirstChild;
					if (Child == null) return false;
					if (Child.Name == "mutation") Child = Child.NextSibling;

					while (Child != null ? Child.Name == "value" : false)
					{
						if (Child.Name != "value" || Child.Attributes["name"].Value.Substring(0, 2) != "IF") return false;
						cmdIf.Conditions.Add(ParseNumber(Child.FirstChild));

						Child = Child.NextSibling;
						if (Child == null) return false;
						if (Child.Name != "statement" || Child.Attributes["name"].Value.Substring(0, 2) != "DO") return false;
						cmdList = new List<Command.Command>();
						if (!ParseBlock(Child.FirstChild, cmdList)) return false;
						cmdIf.Actions.Add(cmdList);

						Child = Child.NextSibling;
					}

					if (Child != null)
					{
						if (Child.Name == "statement" && Child.Attributes["name"].Value == "ELSE")
						{
							NumBoolean cmdTrue = new NumBoolean();
							cmdTrue.Value = true;
							cmdIf.Conditions.Add(cmdTrue);
							cmdList = new List<Command.Command>();
							if (!ParseBlock(Child.FirstChild, cmdList)) return false;
							cmdIf.Actions.Add(cmdList);
						}
					}

					Commands.Add(cmdIf);

					Child = Child.NextSibling;
					if (Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

				case "motor_set_speed":
					MotorSet motorSet = new MotorSet();
					Child = Block.FirstChild;
					if (Child.Name != "field" || Child.Attributes["name"].Value != "POSITION") return false;
					switch(Child.InnerText)
					{
						case "A": motorSet.Motor = MotorID.A; break;
						case "B": motorSet.Motor = MotorID.B; break;
						case "C": motorSet.Motor = MotorID.C; break;
						default: return false;
					}
					Child = Child.NextSibling;
					if(Child.Name != "value" || Child.Attributes["name"].Value != "SPEED") return false;
					motorSet.Speed = ParseNumber(Child.FirstChild);
					if (motorSet.Speed == null) return false;
					Commands.Add(motorSet);

					Child = Child.NextSibling;
					if (Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

				case "motor_reverse":
					MotorReverse motorRev = new MotorReverse();
					Child = Block.FirstChild;
					if (Child == null) return false;
					if (Child.Name != "field" || Child.Attributes["name"].Value != "POSITION") return false;
					switch (Child.InnerText)
					{
						case "A": motorRev.Motor = MotorID.A; break;
						case "B": motorRev.Motor = MotorID.B; break;
						case "C": motorRev.Motor = MotorID.C; break;
						default: return false;
					}
					Commands.Add(motorRev);

					Child = Child.NextSibling;
					if (Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

				case "variables_set":
					SetVar var = new SetVar();
					Child = Block.FirstChild;
					if (Child == null) return false;
					if (Child.Name != "field" || Child.Attributes["name"].Value != "VAR") return false;
					var.Variable = Child.InnerText;

					Child = Child.NextSibling;
					if(Child.Name != "value" || Child.Attributes["name"].Value != "VALUE") return false;
					var.Value = ParseNumber(Child.FirstChild);
					if (var.Value == null) return false;

					Commands.Add(var);

					Child = Child.NextSibling;
					if (Child != null)
						if (Child.Name == "next")
							if (!ParseBlock(Child.FirstChild, Commands)) return false;
					break;

				default:
					Console.WriteLine("No se conoce la operacion tipo " + Block.Attributes["type"].Value);
					return false;
			}

			return true;
		}

		private NumCommand ParseNumber(XmlNode Block)
		{
			XmlNode Child;
			DataType i;
			NumCommand ret = null;

			if (Block.Name == "shadow")
			{
				if(Block.NextSibling != null)
				{
					Block = Block.NextSibling;
					if (Block.Name != "block") return null;
				}
			}
			else if (Block.Name != "block") return null;

			switch (Block.Attributes["type"].Value)
			{
				case "math_number": // Constante aritmetica
					Child = Block.FirstChild;
					if(Child.Name != "field" || Child.Attributes["name"].Value != "NUM") return null;
					i = new DataType(Child.InnerText);

					ret = new NumConstant();
					((NumConstant)ret).Value = i;
					break;

				case "math_arithmetic":
					NumArithmetic na = new NumArithmetic();
					ret = na;
					Child = Block.FirstChild;
					if(Child.Name != "field" || Child.Attributes["name"].Value != "OP") return null;
					switch(Child.InnerText)
					{
						case "ADD": na.Operation = NumArithmetic.OperationType.Add; break;
						case "MINUS": na.Operation = NumArithmetic.OperationType.Sub; break;
						case "MULTIPLY": na.Operation = NumArithmetic.OperationType.Multiply; break;
						case "DIVIDE": na.Operation = NumArithmetic.OperationType.Divide; break;
						case "POWER": na.Operation = NumArithmetic.OperationType.Power; break;
						default:
							Console.WriteLine("No se conoce la operacion aritmetica " + Child.InnerText);
							return null;
					}

					Child = Child.NextSibling;
					if (Child == null) return null;
					if (Child.Name != "value" || Child.Attributes["name"].Value != "A") return null;
					na.Oper1 = ParseNumber(Child.FirstChild);
					if (na.Oper1 == null) return null;

					Child = Child.NextSibling;
					if (Child == null) return null;
					if (Child.Name != "value" || Child.Attributes["name"].Value != "B") return null;
					na.Oper2 = ParseNumber(Child.FirstChild);
					if (na.Oper2 == null) return null;
					break;

				case "logic_boolean":
					NumBoolean nb = new NumBoolean();
					ret = nb;
					Child = Block.FirstChild;
					if(Child.Name != "field" || Child.Attributes["name"].Value != "BOOL") return null;
					switch (Child.InnerText)
					{
						case "TRUE":
							nb.Value = true;
							break;

						default: // Mayormente FALSE
							nb.Value = false;
							break;
					}
					break;

				case "variables_get":
					NumVar var = new NumVar();
					ret = var;
					Child = Block.FirstChild;
					if(Child.Name != "field" || Child.Attributes["name"].Value != "VAR") return null;
					var.Variable = Child.InnerText;
					break;

				case "sensor_gyro_value":
					NumGyro ng = new NumGyro();
					Child = Block.FirstChild;
					if(Child.Name != "field" || Child.Attributes["name"].Value != "COMP") return null;
					switch(Child.InnerText)
					{
						case "GYRO": ng.Parameter = NumGyro.ReadParameter.Gyro; break;
						case "ACCEL": ng.Parameter = NumGyro.ReadParameter.Accel; break;
						default: return null;
					}
					Child = Child.NextSibling;
					if(Child.Name != "field" || Child.Attributes["name"].Value != "AXIS") return null;
					switch(Child.InnerText)
					{
						case "X": ng.Axis = NumGyro.ReadAxis.X; break;
						case "Y": ng.Axis = NumGyro.ReadAxis.Y; break;
						case "Z": ng.Axis = NumGyro.ReadAxis.Z; break;
						default: return null;
					}
					ret = ng;
					break;

				case "sensor_temperature":
					NumGyro nt = new NumGyro();
					nt.Parameter = NumGyro.ReadParameter.Temperature;
					ret = nt;
					break;

				default:
					Console.Out.WriteLine("No se entiende la operación aritmética " + Block.Attributes["type"].Value);
					return null;
			}
			return ret;
		}
	}
}
