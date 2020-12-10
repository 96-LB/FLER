using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLER
{
    /// <summary>
    /// Enables the user to select a color from a dialog
    /// </summary>
    class ColorControl : FLERControl
    {
        public Color Color { get; private set; }

        public ColorControl(Color color)
        {
            Color = color;
        }

        public override void Paint(PaintEventArgs e)
        {
            base.Paint(e);
        }

        public override bool Click(EventArgs e)
        {
            ColorDialog dialog = new ColorDialog() { SolidColorOnly = true };
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                Color = dialog.Color;
                return true;
            }
            return base.Click(e);
        }
    }
}
