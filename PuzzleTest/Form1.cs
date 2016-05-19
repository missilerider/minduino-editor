using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			PuzzlePiece p = new PuzzlePiece();
			this.Controls.Add(p);
			//p.Location = new Point(10, 10);
			p.SetBounds(10, 10, 200, 100);
			//p.Size.Height = 50;
			p.Invalidate();
		}
	}
}
