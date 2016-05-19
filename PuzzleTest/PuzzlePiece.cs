using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleTest
{
	public class PuzzlePiece : UserControl
	{
		public Color frontColor = Color.Beige;
		public Color FrontColor { get { return frontColor; } set { frontColor = value; Invalidate(); } }

		public PuzzlePiece()
		{
			BackColor = Color.Transparent;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//e.Graphics.Clear(BackColor);
			e.Graphics.DrawLine(new Pen(Color.Black), 0, 0, this.ClientSize.Width, this.ClientSize.Height);
		}
	}
}
