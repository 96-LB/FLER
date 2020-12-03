using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLER
{
    /// <summary>
    /// Represents a drawable and interactable UI element
    /// </summary>
    abstract class FLERControl
    {

        #region Properties

        /// <summary>
        /// [Internal] The bounding rectangle for the control
        /// </summary>
        private Rectangle _bounds;

        /// <summary>
        /// The bounding rectangle for the control
        /// </summary>
        public Rectangle Bounds { get => _bounds; set => _bounds = value; }

        /// <summary>
        /// The location of the top-left corner of the control
        /// </summary>
        public Point Location { get => _bounds.Location; set => _bounds.Location = value; }

        /// <summary>
        /// The vertical distance from the top of the control to the origin
        /// </summary>
        public int Top { get => _bounds.Y; set => _bounds.Y = value; }

        /// <summary>
        /// The horizontal distance from the left of the control to the origin
        /// </summary>
        public int Left { get => _bounds.X; set => _bounds.X = value; }

        /// <summary>
        /// The size of the control's bounding box
        /// </summary>
        public Size Size { get => _bounds.Size; set => _bounds.Size = value; }

        /// <summary>
        /// The height of the control's bounding box
        /// </summary>
        public int Height { get => _bounds.Height; set => _bounds.Height = value; }

        /// <summary>
        /// The width of the control's bounding box
        /// </summary>
        public int Width { get => _bounds.Width; set => _bounds.Width = value; }

        /// <summary>
        /// The cursor to be displayed when the mouse pointer is over the control
        /// </summary>
        public Cursor Cursor { get; protected set; } = Cursors.Default;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the control's paint event is triggered
        /// </summary>
        public event PaintEventHandler OnPaint;

        /// <summary>
        /// Triggers the control's paint event
        /// </summary>
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires another paint event</returns>
        public virtual bool Paint(PaintEventArgs e)
        {
            OnPaint?.Invoke(this, e);
            return false;
        }

        /// <summary>
        /// Occurs when the control's mouse enter event is triggered
        /// </summary>
        public event EventHandler OnMouseEnter;

        /// <summary>
        /// Triggers the control's mouse enter event
        /// </summary>
        /// <param name="e">Whether the control requires a paint event</param>
        public virtual bool MouseEnter(EventArgs e)
        {
            OnMouseEnter?.Invoke(this, e);
            return false;
        }

        /// <summary>
        /// Occurs when the control's mouse leave event is triggered
        /// </summary>
        public event EventHandler OnMouseLeave;

        /// <summary>
        /// Triggers the control's mouse leave event
        /// </summary>
        /// <param name="e">Whether the control requires a paint event</param>
        public virtual bool MouseLeave(EventArgs e)
        {
            OnMouseLeave?.Invoke(this, e);
            return false;
        }

        /// <summary>
        /// Occurs when the control's mouse move event is triggered
        /// </summary>
        public event EventHandler OnMouseMove;

        /// <summary>
        /// Triggers the control's mouse move event
        /// </summary>
        /// <param name="e">Whether the control requires a paint event</param>
        public virtual bool MouseMove(MouseEventArgs e)
        {
            OnMouseMove?.Invoke(this, e);
            return false;
        }

        /// <summary>
        /// Occurs when the control's mouse down event is triggered
        /// </summary>
        public event MouseEventHandler OnMouseDown;

        /// <summary>
        /// Triggers the control's mouse down event
        /// </summary>
        /// <param name="e">Whether the control requires a paint event</param>
        public virtual bool MouseDown(MouseEventArgs e)
        {
            OnMouseDown?.Invoke(this, e);
            return false;
        }

        /// <summary>
        /// Occurs when the control's mouse up event is triggered
        /// </summary>
        public event MouseEventHandler OnMouseUp;

        /// <summary>
        /// Triggers the control's mouse up event
        /// </summary>
        /// <param name="e">Whether the control requires a paint event</param>
        public virtual bool MouseUp(MouseEventArgs e)
        {
            OnMouseUp?.Invoke(this, e);
            return false;
        }

        /// <summary>
        /// Occurs when the control's click event is triggered
        /// </summary>
        public event EventHandler OnClick;

        /// <summary>
        /// Triggers the control's click event
        /// </summary>
        /// <param name="e">Whether the control requires a paint event</param>
        public virtual bool Click(EventArgs e)
        {
            OnClick?.Invoke(this, e);
            return false;
        }

        #endregion

    }
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
