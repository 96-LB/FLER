using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace FLER
{
    /// <summary>
    /// Renders a static flashcard with no flip animation
    /// </summary>
    class StaticFlashcardControl : FLERControl
    {

        #region Properties

        /// <summary>
        /// Whether the flashcard is flipped to show the hidden face
        /// </summary>
        public bool Flipped { get; protected set; }

        /// <summary>
        /// [Internal] The list of sprites for the currently selected face
        /// </summary>
        protected Image Sprite { get => Flipped ? _hidden : _visible; }

        #region [Sprite] Backing Fields

        /// <summary>
        /// [Internal] The list of sprites for the visible face
        /// </summary>
        private Image _visible;

        /// <summary>
        /// [Internal] The list of sprites for the hidden face
        /// </summary>
        private Image _hidden;

        #endregion

        /// <summary>
        /// [Internal] The sprite to draw on next paint
        /// </summary>
        protected virtual Image ToPaint { get => Sprite; }

        #endregion

        #region Fields

        /// <summary>
        /// [Internal] Whether the mouse is over the flashcard bounds
        /// </summary>
        protected bool _mouse = false;

        /// <summary>
        /// [Internal] The size used to calculate the last flashcard border
        /// </summary>
        private Size _size = Size.Empty;

        /// <summary>
        /// [Internal] The flashcard border
        /// </summary>
        private readonly GraphicsPath _bounds = new GraphicsPath();

        #endregion

        #region Constants

        /// <summary>
        /// The border radius of the flashcard
        /// </summary>
        public const int RADIUS = 24;

        /// <summary>
        /// The border diameter of the flashcard
        /// </summary>
        public const int DIAMETER = RADIUS * 2;

        /// <summary>
        /// The outline thickness of the flashcard
        /// </summary>
        public const int OUTLINE = 2;

        /// <summary>
        /// Half of the outline thickness of the flashcard
        /// </summary>
        public const int OUT = OUTLINE / 2;

        /// <summary>
        /// The width of the base flashcard sprite
        /// </summary>
        public const int WIDTH = 480;

        /// <summary>
        /// The height of the base flashcard sprite
        /// </summary>
        public const int HEIGHT = 320;

        #region Fake Constants

        /// <summary>
        /// The default font to use when none is specified
        /// </summary>
        public static Font FONT_DEF { get; } = new Font("Arial", 24);

        #endregion

        #endregion

        #region Methods

        #region Private Static

        /// <summary>
        /// Creates a high-quality graphics object from the specified bitmap
        /// </summary>
        /// <param name="bmp">The bitmap from which to create the graphics object</param>
        /// <returns>A high-quality graphics object initialized from the bitmap</returns>
        protected static Graphics QualityGraphics(Bitmap bmp)
        {
            Graphics graphics = Graphics.FromImage(bmp); //graphics initialized from the bitmap

            //sets all the options on the graphics to high-quality
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.MeasureString(" ", FONT_DEF); //fixes a strange graphics bug that turns off antialiasing for future controls when making a high-quality graphics
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            return graphics; //returns the initialized graphics
        }

        /// <summary>
        /// Renders a flashcard face onto a bitmap
        /// </summary>
        /// <param name="face">The flashcard face to render</param>
        /// <returns>The base image for a rendered flashcard face</returns>
        private static Bitmap RenderFace(Flashcard.Face face)
        {
            Bitmap output = new Bitmap(WIDTH, HEIGHT); //the rendered bitmap

            using Graphics graphics = QualityGraphics(output); //the graphics with which to draw on the bitmap
            using GraphicsPath path = new GraphicsPath(); //the border of the flashcard
            using Brush backBrush = new SolidBrush(face.BackColor); //the brush used to color the background of the flashcard
            using Brush textBrush = new SolidBrush(face.TextColor); //the brush used to color the text of the flashcard
            using Pen borderPen = new Pen(face.LineColor, OUTLINE); //the pen used to color the border of the flashcard

            int WDIAMETER = Math.Min(DIAMETER, WIDTH - OUTLINE); //the constrained width (horizontal) diameter
            int HDIAMETER = Math.Min(DIAMETER, HEIGHT - OUTLINE); //the constrained height (vertical) diameter
            int RIGHT = WIDTH - WDIAMETER - OUT; //the start position of the right corner arcs
            int BOTTOM = HEIGHT - HDIAMETER - OUT; //the start position of the bottom corner arcs

            //creates the border of the flashcard
            path.AddArc(OUT, OUT, WDIAMETER, HDIAMETER, 180, 90);
            path.AddArc(RIGHT, OUT, WDIAMETER, HDIAMETER, 270, 90);
            path.AddArc(RIGHT, BOTTOM, WDIAMETER, HDIAMETER, 0, 90);
            path.AddArc(OUT, BOTTOM, WDIAMETER, HDIAMETER, 90, 90);
            path.CloseFigure();

            //fills the flashcard area
            graphics.FillPath(backBrush, path);

            //draws the text in the face's text box
            void _renderText()
            {
                //sets the clipping region to the text box
                graphics.SetClip(path);
                graphics.IntersectClip(face.TextBox);

                //renders the text with the specified options
                StringAlignment horizontal = (StringAlignment)Math.Log((int)face.TextAlign % 5, 2); //extracts the horizontal alignment using the underlying enum values
                StringAlignment vertical = (StringAlignment)Math.Log((int)face.TextAlign, 16); //extracts the vertical alignment using the underlying enum values
                graphics.DrawString(face.Text, face.Font ?? FONT_DEF, textBrush, face.TextBox, new StringFormat() { Alignment = horizontal, LineAlignment = vertical });
            }

            //draws the image in the face's image box
            void _renderImage()
            {
                //if an image has been specified, then draw it
                if (face.ImagePath != null)
                {
                    //sets the clipping region to the image box
                    graphics.SetClip(path);
                    graphics.IntersectClip(face.ImageBox);

                    try
                    {
                        using Image image = Image.FromFile(face.ImagePath); //the image to render

                        //if the image was successfully loaded, draw it in the image box
                        graphics.DrawImage(image, face.ImageBox);
                    }
                    catch
                    {
                        //if any error occurred, draw the 'missing' image instead
                        graphics.DrawImage(Properties.Resources.Missing, face.ImageBox);
                    }
                }
            }

            //draw the text and image in the specified order
            if (face.ImageTop)
            {
                _renderText();
                _renderImage();
            }
            else
            {
                _renderImage();
                _renderText();
            }

            //draw the flashcard border
            graphics.ResetClip();
            graphics.DrawPath(borderPen, path);

            return output; //returns the rendered bitmap
        }

        #endregion

        #region Public Instance

        /// <summary>
        /// Determines whether a point falls within the flashcard border
        /// </summary>
        /// <param name="parent">A point represented in parent coordinatess</param>
        /// <returns>Whether the point falls within the flashcard border</returns>
        public virtual bool InBounds(Point parent)
        {
            //if the size changed since the last function call, regenerate the flashcard border path
            if (_size != Size)
            {
                _bounds.Reset();

                float WFACTOR = (float)Width / WIDTH; //the width scaling factor
                float HFACTOR = (float)Height / HEIGHT; //the height scaling factor
                float WDIAMETER = WFACTOR * Math.Min(DIAMETER, WIDTH); //the constrained width (horizontal) diameter
                float HDIAMETER = HFACTOR * Math.Min(DIAMETER, HEIGHT); //the constrained height (vertical) diameter
                float RIGHT = WFACTOR * WIDTH - WDIAMETER; //the start position of the right corner arcs
                float BOTTOM = HFACTOR * HEIGHT - HDIAMETER; //the start position of the bottom corner arcs

                //creates the border of the flashcard
                _bounds.AddArc(0, 0, WDIAMETER, HDIAMETER, 180, 90);
                _bounds.AddArc(RIGHT, 0, WDIAMETER, HDIAMETER, 270, 90);
                _bounds.AddArc(RIGHT, BOTTOM, WDIAMETER, HDIAMETER, 0, 90);
                _bounds.AddArc(0, BOTTOM, WDIAMETER, HDIAMETER, 90, 90);
                _bounds.CloseFigure();

                //sets the size cache
                _size = Size;
            }

            return _bounds.IsVisible(PointToLocal(parent)); //returns whether the point falls within the flashcard border
        }

        /// <summary>
        /// Loads sprites from the given flashcard, rendering any missing ones
        /// </summary>
        /// <param name="card">The flashcard to render and load</param>
        /// <returns>Whether the control requires a paint event</returns>
        public virtual bool LoadCard(Flashcard card)
        {
            //disposes the loaded sprite images
            _visible?.Dispose();
            _hidden?.Dispose();

            //if the temporary image directory exists, delete it
            if (Directory.Exists(Path.Combine(FLERForm.IMG_DIR, "TEMP")))
            {
                Directory.Delete(Path.Combine(FLERForm.IMG_DIR, "TEMP"), true);
            }

            string vpath = Path.Combine(FLERForm.IMG_DIR, !string.IsNullOrWhiteSpace(card.Filename) ? card.Filename : "TEMP", "v.png"); //the visible face's stored image path
            try
            {
                //load the stored image if it exists
                _visible = Image.FromFile(vpath);
            }
            catch
            {
                //if there is no stored image, generate it and save it
                Image image = RenderFace(card.Visible); //the rendered image
                Directory.CreateDirectory(Path.GetDirectoryName(vpath));
                image.Save(vpath);
                _visible = image;
            }

            string hpath = Path.Combine(FLERForm.IMG_DIR, !string.IsNullOrWhiteSpace(card.Filename) ? card.Filename : "TEMP", "h.png"); //the hidden face's stored image path
            try
            {
                //load the stored image if it exists
                _hidden = Image.FromFile(hpath);
            }
            catch
            {
                //if there is no stored image, generate it and save it
                Image image = RenderFace(card.Hidden); //the rendered image
                Directory.CreateDirectory(Path.GetDirectoryName(hpath));
                image.Save(hpath);
                _hidden = image;
            }

            return true; //returns true to repaint the control
        }

        /// <summary>
        /// Flips the flashcard
        /// </summary>
        /// <returns>Whether the control requires a repaint</returns>
        public virtual bool Flip()
        {
            //flips the flip variable
            Flipped = !Flipped;
            return true; //returns true to repaint the control
        }

        #endregion

        #endregion

        #region Events

        //inherited docstring
        public override void Paint(PaintEventArgs e)
        {
            //call base method to trigger events
            base.Paint(e);

            using Region clip = e.Graphics.Clip; //the clip region of the graphics

            //sets the clip to the bounds, draws the currently selected sprite, and resets the clip
            e.Graphics.IntersectClip(Bounds);
            e.Graphics.DrawImage(ToPaint, Bounds);
            e.Graphics.Clip = clip;
        }

        //inherited docstring
        public override bool MouseMove(MouseEventArgs e)
        {
            bool repaint = base.MouseMove(e); //call base method to trigger events

            //determines whether the mouse is within the bounds
            _mouse = InBounds(e.Location);
            Cursor = _mouse ? Cursors.Hand : Cursors.Default;
            return repaint; //returns base method to determine whether to repaint
        }

        //inherited docstring
        public override bool Click(EventArgs e)
        {
            //call base method to trigger events
            base.Click(e);

            //if the mouse is within the bounds, flip the flashcard
            if (_mouse)
            {
                Flip();
            }
            return true; //returns true to repaint the control
        }

        //inherited docstring
        public override bool MouseLeave(EventArgs e)
        {
            bool repaint = base.MouseLeave(e); //call base method to trigger events

            //the mouse is not within the bounds
            _mouse = false;
            Cursor = Cursors.Default;
            return repaint; //returns base method to determine whether to repaint
        }

        #endregion

    }
}