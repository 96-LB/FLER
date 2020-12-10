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

        #region Properties

        /// <summary>
        /// The color stored by the control
        /// </summary>
        public Color Color { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initiializes the control with an initial color
        /// </summary>
        /// <param name="color">The initial color</param>
        public ColorControl(Color color)
        {
            //sets the initial color and cursor
            Color = color;
            Cursor = Cursors.Hand;
        }

        #endregion

        #region Events

        //inherited docstring
        public override void Paint(PaintEventArgs e)
        {
            //call base method to trigger events
            base.Paint(e);

            using Region clip = e.Graphics.Clip; //the clip region of the graphics
            using Brush brush = new SolidBrush(Color);

            //sets the clip to the bounds, fills the bounds with the selected color, and resets the clip
            e.Graphics.IntersectClip(Bounds);
            e.Graphics.FillRectangle(brush, Bounds);
            e.Graphics.Clip = clip;
        }

        //inherited docstring
        public override bool Click(EventArgs e)
        {
            bool repaint = base.Click(e); //call base method to trigger events

            ColorDialog dialog = new ColorDialog() { SolidColorOnly = true, FullOpen = true }; //a dialog to choose a color
            
            //if the user clicked ok, set the control's color
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                Color = dialog.Color;
                return true; //returns true to repaint the control
            }

            return repaint; //return the base method to determine whether to repaint
        }

        #endregion

    }
}
