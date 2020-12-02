using System;
using System.Collections.Generic;
using System.Drawing;
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
        public virtual bool MouseMove(EventArgs e)
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
        public bool flipped, going;
        public List<Image> Sprites { get => flipped ? hidden : visible; }
        public readonly List<Image> visible = new List<Image>();
        public readonly List<Image> hidden = new List<Image>();
        public int counter = 0;
        bool outline = false;
        public FlashcardControl()
        {
            Cursor = Cursors.Hand;
        }

        public override bool Paint(PaintEventArgs e)
        {
            Region r = e.Graphics.Clip;
            e.Graphics.IntersectClip(Bounds);
            e.Graphics.DrawImage(Sprites[counter], Bounds);
            using Pen p = new Pen(Color.Red, 4);
            if (outline) e.Graphics.DrawRectangle(p, Bounds);
            e.Graphics.Clip = r;
            return base.Paint(e);
        }

        public override bool Click(EventArgs e)
        {
            going = true;
            base.Click(e);
            return true;
        }

        public override bool MouseEnter(EventArgs e)
        {
            outline = true;
            base.MouseEnter(e);
            return true;
        }

        public override bool MouseLeave(EventArgs e)
        {
            outline = false;
            base.MouseLeave(e);
            return true;
        }

        int c = 0;
        public bool Flip()
        {
            Console.WriteLine(c++);
            bool output = going;
            if (going && ++counter >= Sprites.Count)
            {
                counter = 0;
                flipped = !flipped;
                going = false;
            }
            return output;
        }
    }
}
