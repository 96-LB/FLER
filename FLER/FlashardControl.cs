using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLER
{
    class FlashcardControl : FLERControl
    {
        public List<Image> Sprites { get => flipped ? hidden : visible; }
        public readonly List<Image> visible = new List<Image>();
        public readonly List<Image> hidden = new List<Image>();

        public float Factor { get; set; }
        public int Radius { get; set; }

        public bool flipped, going;
        bool mouse = false;
        int counter = 0;
        bool outline = false;
        Size oldSize = Size.Empty;
        GraphicsPath path = new GraphicsPath();
        public override bool Paint(PaintEventArgs e)
        {
            Region r = e.Graphics.Clip;
            e.Graphics.IntersectClip(Bounds);
            e.Graphics.DrawImage(Sprites[counter], Bounds);
            using Pen p = new Pen(Color.Red, 4);
            if (outline)
            {
                inBounds(Point.Empty);
                e.Graphics.TranslateTransform(Left, Top);
                e.Graphics.DrawPath(p, path);
                e.Graphics.TranslateTransform(-Left, -Top);
            }
            e.Graphics.Clip = r;
            return base.Paint(e);
        }

        public override bool Click(EventArgs e)
        {
            if (outline)
            {
                going = true;
            }
            base.Click(e);
            return outline;
        }

        bool inBounds(Point p)
        {
            if (oldSize != Size)
            {
                path.Reset();
                if (Sprites.Count > 0)
                {
                    float WIDTH = Sprites[0].Width;
                    float HEIGHT = Sprites[0].Height;
                    float WDIAMETER = Math.Min(WIDTH, Radius * 2);
                    float HDIAMETER = Math.Min(HEIGHT, Radius * 2);
                    float LEFT = HEIGHT * Factor * 0.5f;
                    float RIGHT = WIDTH - WDIAMETER - LEFT;
                    float BOTTOM = HEIGHT - HDIAMETER;
                    float WFACTOR = Width / WIDTH;
                    float HFACTOR = Height / HEIGHT;

                    path.AddArc(WFACTOR * LEFT, 0, WFACTOR * WDIAMETER, HFACTOR * HDIAMETER, 180, 90);
                    path.AddArc(WFACTOR * RIGHT, 0, WFACTOR * WDIAMETER, HFACTOR * HDIAMETER, 270, 90);
                    path.AddArc(WFACTOR * RIGHT, HFACTOR * BOTTOM, WFACTOR * WDIAMETER, HFACTOR * HDIAMETER, 0, 90);
                    path.AddArc(WFACTOR * LEFT, HFACTOR * BOTTOM, WFACTOR * WDIAMETER, HFACTOR * HDIAMETER, 90, 90);
                    path.CloseFigure();

                    oldSize = Size;
                }
                else
                {
                    path.AddRectangle(Bounds);
                }
            }

            return path.IsVisible(p);
        }

        public override bool MouseMove(MouseEventArgs e)
        {
            bool oldline = outline;
            mouse = inBounds(e.Location - (Size)Location);
            outline = !going && mouse;
            Cursor = outline ? Cursors.Hand : Cursors.Default;
            base.MouseMove(e);
            return outline != oldline;
        }

        public override bool MouseLeave(EventArgs e)
        {
            mouse = false;
            outline = false;
            base.MouseLeave(e);
            return true;
        }

        int c = 0;
        public bool Flip()
        {
            bool output = going;
            if (going && ++counter >= Sprites.Count)
            {
                counter = 0;
                flipped = !flipped;
                going = false;
                if (mouse)
                {
                    outline = true;
                    Cursor = Cursors.Hand;
                }
            }
            return output;
        }
    }
}
