using System;
using System.Drawing;
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
        /// Whether the control should be drawn
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// The cursor to be displayed when the mouse pointer is over the control
        /// </summary>
        public Cursor Cursor { get; protected set; } = Cursors.Default;

        #endregion

        #region Methods

        /// <summary>
        /// Computes the location of the specified parent point into local coordinates
        /// </summary>
        /// <returns>The specified parent point converted into local coordinates</returns>
        public Point PointToLocal(Point parent)
        {
            return parent - (Size)Location; //translates the parent point by the control's location
        }

        /// <summary>
        /// Computes the location of the specified local point into parent coordinates
        /// </summary>
        /// <returns>The specified local point converted into parent coordinates</returns>
        public Point PointToParent(Point local)
        {
            return local + (Size)Location; //translates the local point by the control's location
        }

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
        public virtual void Paint(PaintEventArgs e)
        {
            OnPaint?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when the control's mouse enter event is triggered
        /// </summary>
        public event EventHandler OnMouseEnter;

        /// <summary>
        /// Triggers the control's mouse enter event
        /// </summary>
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires a paint event</returns>
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
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires a paint event</returns>
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
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires a paint event</returns>
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
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires a paint event</returns>
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
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires a paint event</returns>
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
        /// <param name="e">The event data</param>
        /// <returns>Whether the control requires a paint event</returns>
        public virtual bool Click(EventArgs e)
        {
            OnClick?.Invoke(this, e);
            return false;
        }

        #endregion

    }
}
