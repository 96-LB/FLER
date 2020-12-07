using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLER
{
    class FlashcardControl : FLERControl
    {

        #region Properties

        /// <summary>
        /// [Internal] The list of sprites for the currently selected face
        /// </summary>
        private List<Image> Sprites { get => _flipped ? _hidden : _visible; }

        #region [Sprites] Backing Fields

        /// <summary>
        /// [Internal] The list of sprites for the visible face
        /// </summary>
        private readonly List<Image> _visible = new List<Image>();

        /// <summary>
        /// [Internal] The list of sprites for the hidden face
        /// </summary>
        private readonly List<Image> _hidden = new List<Image>();

        #endregion

        #endregion

        #region Constants

        /// <summary>
        /// The perspective factor used when rotating the flashcard sprites
        /// </summary>
        public const float FACTOR = 0.25f;

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
        public const int BASEWIDTH = 480;

        /// <summary>
        /// The height of the base flashcard sprite
        /// </summary>
        public const int BASEHEIGHT = 320;

        /// <summary>
        /// The width of a full flashcard sprite, including room for rotation
        /// </summary>
        public const float IMGWIDTH = BASEWIDTH + FACTOR * BASEHEIGHT;

        /// <summary>
        /// The height of a full flashcard sprite, including room for rotation
        /// </summary>
        public const float IMGHEIGHT = BASEHEIGHT;

        /// <summary>
        /// The number of degrees rotated in between each sprite
        /// </summary>
        public const int INTERVAL = 3;

        #region Fake Constants

        /// <summary>
        /// The default font to use when none is specified
        /// </summary>
        public static Font FONT_DEF { get; } = new Font("Arial", 18);

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// [Internal] Whether the flashcard is flipped to show the hidden face
        /// </summary>
        private bool _flipped = false;

        /// <summary>
        /// [Internal] Whether the flashcard is currently flipping
        /// </summary>
        private bool _going = false;

        /// <summary>
        /// [Internal] Whether the mouse is over the flashcard bounds
        /// </summary>
        private bool _mouse = false;

        /// <summary>
        /// [Internal] The index of the current sprite to display
        /// </summary>
        private int _counter = 0;

        /// <summary>
        /// [Internal] The size used to calculate the last flashcard border
        /// </summary>
        private Size _size = Size.Empty;

        /// <summary>
        /// [Internal] The flashcard border
        /// </summary>
        private readonly GraphicsPath _bounds = new GraphicsPath();

        #endregion

        #region Methods

        #region Private Static

        /// <summary>
        /// Creates a high-quality graphics object from the specified bitmap
        /// </summary>
        /// <param name="bmp">The bitmap from which to create the graphics object</param>
        /// <returns>A high-quality graphics object initialized from the bitmap</returns>
        private static Graphics QualityGraphics(Bitmap bmp)
        {
            Graphics graphics = Graphics.FromImage(bmp); //graphics initialized from the bitmap

            //sets all the options on the graphics to high-quality
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            return graphics; //returns the initialized graphics
        }

        /// <summary>
        /// Renders a flashcard face onto a bitmap
        /// </summary>
        /// <param name="face">The flashcard face to render</param>
        /// <returns>The base image for a rendered flashcard face</returns>
        private static Bitmap RenderFace(Flashcard.Face face)
        {
            Bitmap output = new Bitmap(BASEWIDTH, BASEHEIGHT); //the rendered bitmap

            using Graphics graphics = Graphics.FromImage(output); //the graphics with which to draw on the bitmap
            using GraphicsPath path = new GraphicsPath(); //the border of the flashcard
            using Brush backColor = new SolidBrush(face.BackColor); //the brush used to color the background of the flashcard
            using Brush foreColor = new SolidBrush(face.ForeColor); //the brush used to color the text of the flashcard
            using Pen forePen = new Pen(face.ForeColor, OUTLINE); //the pen used to color the border of the flashcard

            int WDIAMETER = Math.Min(DIAMETER, BASEWIDTH - OUTLINE); //the constrained width (horizontal) diameter
            int HDIAMETER = Math.Min(DIAMETER, BASEHEIGHT - OUTLINE); //the constrained height (vertical) diameter
            int RIGHT = BASEWIDTH - WDIAMETER - OUT; //the start position of the right corner arcs
            int BOTTOM = BASEHEIGHT - HDIAMETER - OUT; //the start position of the bottom corner arcs

            //creates the border of the flashcard
            path.AddArc(OUT, OUT, WDIAMETER, HDIAMETER, 180, 90);
            path.AddArc(RIGHT, OUT, WDIAMETER, HDIAMETER, 270, 90);
            path.AddArc(RIGHT, BOTTOM, WDIAMETER, HDIAMETER, 0, 90);
            path.AddArc(OUT, BOTTOM, WDIAMETER, HDIAMETER, 90, 90);
            path.CloseFigure();

            //fills the flashcard area
            graphics.FillPath(backColor, path);

            //draws the text in the face's text box
            void _renderText()
            {
                //sets the clipping region to the text box
                graphics.SetClip(path);
                graphics.IntersectClip(face.TextBox);

                //renders the text with the specified options
                graphics.DrawString(face.Text, face.Font ?? FONT_DEF, foreColor, face.TextBox, face.TextFormat);
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
            graphics.DrawPath(forePen, path);

            return output; //returns the rendered bitmap
        }

        /// <summary>
        /// Psuedo-rotates the specified image on the Z-axis by the specified amount
        /// </summary>
        /// <param name="image">The image to rotate</param>
        /// <param name="degrees">The number of degrees to rotate it</param>
        /// <returns>A bitmap of the rotated image</returns>
        private static Bitmap RotateFace(Image image, int degrees)
        {
            static int round(double num) => (int)Math.Round(num); //a helper function to round to an integral type

            using Bitmap bmp = new Bitmap(image.Width + round(image.Height * FACTOR), image.Height); //an intermediate bitmap
            using Graphics graphics = QualityGraphics(bmp); //the graphics with which to draw on the intermediate bitmap
            using ImageAttributes attributes = new ImageAttributes(); //attributes used to sample images correctly
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            double RAD = degrees * Math.PI / 180.0; //the rotation amount converted to radians
            double SIN = FACTOR * Math.Sin(RAD); //the sine factor in the rotation, used for horizontal stretching
            double COS = Math.Cos(RAD); //the cosine factor in rotation, used for vertical squashing

            //iterates over every row in the image and applies a horizontal stretching factor
            for (int i = 0; i < image.Height; i++)
            {
                int WIDTH = round(2 * SIN * (i - 0.5f * image.Height) + image.Width); //the width of this row

                //draws the stretched row
                graphics.DrawImage(image,                                  //the source image
                    new Rectangle((bmp.Width - WIDTH) / 2, i, WIDTH, 1), //the stretched destination rectangle
                    0, i, image.Width, 1,                                  //the source rectangle
                    GraphicsUnit.Pixel, attributes);                     //the settings
            }

            Bitmap output = new Bitmap(bmp.Width, bmp.Height); //the rendered bitmap
            using Graphics outgraphics = QualityGraphics(output); //the graphics with which to draw on the bitmap

            //draw the contents of the intermediate bitmap, applying a vertical squashing factor
            outgraphics.DrawImage(bmp,                                                                              //the source image
                new Rectangle(0, round(0.5 * (1 - COS) * output.Height), output.Width, round(COS * output.Height)), //the stretched destination rectangle
                0, 0, bmp.Width, bmp.Height,                                                                        //the source rectangle
                GraphicsUnit.Pixel, attributes);                                                                    //the settings

            return output; //returns the rendered bitmap
        }

        #endregion

        #region Public Instance

        /// <summary>
        /// Loads sprites from the given flashcard, rendering any missing ones
        /// </summary>
        /// <param name="card">The flashcard to render and load</param>
        /// <returns>Whether the control requires a paint event</returns>
        public bool LoadCard(Flashcard card)
        {
            //disposes and clears all of the loaded sprite images
            foreach (Image image in _visible)
            {
                image.Dispose();
            }
            foreach (Image image in _hidden)
            {
                image.Dispose();
            }
            _visible.Clear();
            _hidden.Clear();

            Image vbase = null; //the base image for the visible face
            Image hbase = null; //the base image for the hidden face
            string path = null; //the filepath to load images from

            //generates sprites across a 180 degree rotation, using the specified interval
            for (int i = 0; i < 180; i += INTERVAL)
            {
                //if past 90 degrees, add each face's sprites to the opposite list
                _flipped = i > 90;
                //loads the visible face's image
                path = Path.Combine(FLERForm.IMG_DIR, card.Filename, "v", i + ".png");
                try
                {
                    //load the stored image if it exists
                    Sprites.Add(Image.FromFile(path));
                }
                catch
                {
                    //otherwise, generate it and save it
                    Image image = RotateFace(vbase ??= RenderFace(card.Visible), _flipped ? i - 180 : i); //the rendered image
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    image.Save(path);
                    Sprites.Add(image);
                }

                //repeat above but with the hidden sprite
                _flipped = !_flipped;
                //loads the hidden face's image
                path = Path.Combine(FLERForm.IMG_DIR, card.Filename, "h", i + ".png");
                try
                {
                    //load the stored image if it exists
                    Sprites.Add(Image.FromFile(path));
                }
                catch
                {
                    //otherwise, generate it and save it
                    Image image = RotateFace(hbase ??= RenderFace(card.Hidden), _flipped ? i : i - 180); //the rendered image
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    image.Save(path);
                    Sprites.Add(image);
                }
            }

            //disposes any base images that were rendered
            vbase?.Dispose();
            hbase?.Dispose();

            //resets movement variables
            _counter = Sprites.Count;
            Flip();

            return true; //returns true to repaint the control
        }

        /// <summary>
        /// Invokes a flip event for the flashcard if it is currently flipping
        /// </summary>
        /// <returns>Whether the control requires a paint event</returns>
        public bool Flip()
        {
            bool output = _going; //whether the control needs a repaint

            //if the flashcard is flipping, increment the counter
            if(_going)
            {
                _counter++;
            }

            //if the counter equals the number of sprites, the flip is finished
            if (_counter >= Sprites.Count)
            {
                //reset movement variables
                _counter = 0;
                _flipped = !_flipped;
                _going = false;
                Cursor = _mouse ? Cursors.Hand : Cursors.Default;
            }

            return output; //returns true if the flashcard was flipping to repaint the control
        }

        /// <summary>
        /// Determines whether a point falls within
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool InBounds(Point parent)
        {
            //if the size changed since the last function call, regenerate the flashcard border path
            if (_size != Size)
            {
                _bounds.Reset();

                float WFACTOR = Width / IMGWIDTH; //the width scaling factor
                float HFACTOR = Height / IMGHEIGHT; //the height scaling factor
                float WDIAMETER = WFACTOR * Math.Min(DIAMETER, BASEWIDTH); //the constrained width (horizontal) diameter
                float HDIAMETER = HFACTOR * Math.Min(DIAMETER, BASEHEIGHT); //the constrained height (vertical) diameter
                float LEFT = WFACTOR * IMGHEIGHT * FACTOR * 0.5f; //the start position of the left corner arcs
                float RIGHT = WFACTOR * (IMGWIDTH - WDIAMETER) - LEFT; //the start position of the right corner arcs
                float BOTTOM = HFACTOR * (IMGHEIGHT - HDIAMETER); //the start position of the bottom corner arcs

                //creates the border of the flashcard
                _bounds.AddArc(LEFT, 0, WDIAMETER, HDIAMETER, 180, 90);
                _bounds.AddArc(RIGHT, 0, WDIAMETER, HDIAMETER, 270, 90);
                _bounds.AddArc(RIGHT, BOTTOM, WDIAMETER, HDIAMETER, 0, 90);
                _bounds.AddArc(LEFT, BOTTOM, WDIAMETER, HDIAMETER, 90, 90);
                _bounds.CloseFigure();

                //sets the size cache
                _size = Size;
            }

            return _bounds.IsVisible(PointToLocal(parent)); //returns whether the point falls within the flashcard border
        }

        #endregion

        #endregion

        #region Events

        public override bool Paint(PaintEventArgs e)
        {
            Region clip = e.Graphics.Clip; //the clip region of the graphics

            //sets the clip to the bounds, draws the currently selected sprite, and resets ths clip
            e.Graphics.IntersectClip(Bounds);
            e.Graphics.DrawImage(Sprites[_counter], Bounds);
            e.Graphics.Clip = clip;

            return base.Paint(e); //returns base method to determine whether to repaint
        }

        public override bool Click(EventArgs e)
        {
            //if the mouse is within the bounds, flip the card
            if (_mouse)
            {
                _going = true;
            }
            return base.Click(e); //returns base method to determine whether to repaint
        }

        public override bool MouseMove(MouseEventArgs e)
        {
            //determines whether the mouse is within the bounds
            _mouse = InBounds(e.Location);
            Cursor = !_going && _mouse ? Cursors.Hand : Cursors.Default;
            return base.MouseMove(e); //returns base method to determine whether to repaint
        }

        public override bool MouseLeave(EventArgs e)
        {
            //the mouse is not within the bounds
            _mouse = false;
            Cursor = Cursors.Default;
            return base.MouseLeave(e); //returns base method to determine whether to repaint
        }

        #endregion

    }
}
