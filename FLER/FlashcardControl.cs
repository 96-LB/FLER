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
        /// <summary>
        /// [Internal] The list of sprites for the currently selected face
        /// </summary>
        private List<Image> Sprites { get => flipped ? _hidden : _visible; }

        /// <summary>
        /// The list of sprites for the visible face
        /// </summary>
        private readonly List<Image> _visible = new List<Image>();

        /// <summary>
        /// The list of sprites for the hidden face
        /// </summary>
        private readonly List<Image> _hidden = new List<Image>();

        #region Constants

        /// <summary>
        /// The perspective factor used when rotating the flashcard sprites
        /// </summary>
        public const float FACTOR = 1.25f;

        /// <summary>
        /// The border radius of the flashcard
        /// </summary>
        public const int RADIUS = 350;

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
        public const int BASEHEIGHT = 520;

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

        public bool flipped, going;
        bool mouse = false;
        int counter = 0;
        bool outline = false;
        Size oldSize = Size.Empty;
        GraphicsPath boundsPath = new GraphicsPath();

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
        private Bitmap RenderFace(Flashcard.Face face)
        {
            Bitmap output = new Bitmap(BASEWIDTH, BASEHEIGHT); //the rendered bitmap

            using Graphics graphics = Graphics.FromImage(output); //the graphics with which to draw on the bitmap
            using GraphicsPath path = new GraphicsPath(); //the border of the card
            using Brush backColor = new SolidBrush(face.BackColor); //the brush used to color the background of the card
            using Brush foreColor = new SolidBrush(face.ForeColor); //the brush used to color the text of the card
            using Pen forePen = new Pen(face.ForeColor, OUTLINE); //the pen used to color the border of the card

            int WDIAMETER = Math.Min(DIAMETER, BASEWIDTH - OUTLINE); //the constrained width (horizontal) diameter
            int HDIAMETER = Math.Min(DIAMETER, BASEHEIGHT - OUTLINE); //the constrained height (vertical) diameter
            int RIGHT = BASEWIDTH - WDIAMETER - OUT; //the start position of the right corner arcs
            int BOTTOM = BASEHEIGHT - HDIAMETER - OUT; //the start position of the bottom corner arcs

            //creates the border of the card
            path.AddArc(OUT, OUT, WDIAMETER, HDIAMETER, 180, 90);
            path.AddArc(RIGHT, OUT, WDIAMETER, HDIAMETER, 270, 90);
            path.AddArc(RIGHT, BOTTOM, WDIAMETER, HDIAMETER, 0, 90);
            path.AddArc(OUT, BOTTOM, WDIAMETER, HDIAMETER, 90, 90);
            path.CloseFigure();

            //fills the card area
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
                        using Image img = Image.FromFile(face.ImagePath); //the image to render

                        //if the image was successfully loaded, draw it in the image box
                        graphics.DrawImage(img, face.ImageBox);
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

            //draw the card border
            graphics.ResetClip();
            graphics.DrawPath(forePen, path);

            return output; //returns the rendered bitmap
        }

        /// <summary>
        /// Psuedo-rotates the specified image on the Z-axis by the specified amount
        /// </summary>
        /// <param name="img">The image to rotate</param>
        /// <param name="degrees">The number of degrees to rotate it</param>
        /// <returns>A bitmap of the rotated image</returns>
        private Bitmap RotateFace(Image img, int degrees)
        {
            static int round(double num) => (int)Math.Round(num); //a helper function to round to an integral type

            using Bitmap bmp = new Bitmap(img.Width + round(img.Height * FACTOR), img.Height); //an intermediate bitmap
            using Graphics graphics = QualityGraphics(bmp); //the graphics with which to draw on the intermediate bitmap
            using ImageAttributes attributes = new ImageAttributes(); //attributes used to sample images correctly
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            double RAD = degrees * Math.PI / 180.0; //the rotation amount converted to radians
            double SIN = FACTOR * Math.Sin(RAD); //the sine factor in the rotation, used for horizontal stretching
            double COS = Math.Cos(RAD); //the cosine factor in rotation, used for vertical squashing

            //iterates over every row in the image and applies a horizontal stretching factor
            for (int i = 0; i < img.Height; i++)
            {
                int WIDTH = round(2 * SIN * (i - 0.5f * img.Height) + img.Width); //the width of this row

                //draws the stretched row
                graphics.DrawImage(img,                                  //the source image
                    new Rectangle((bmp.Width - WIDTH) / 2, i, WIDTH, 1), //the stretched destination rectangle
                    0, i, img.Width, 1,                                  //the source rectangle
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

        /// <summary>
        /// Loads sprites from the given flashcard, rendering any missing ones
        /// </summary>
        /// <param name="card">The flashcard to render and load</param>
        /// <returns>Whether the control requires a paint event</returns>
        public bool LoadCard(Flashcard card)
        {
            //disposes and clears all of the loaded sprite images
            foreach (Image img in _visible)
            {
                img.Dispose();
            }
            foreach (Image img in _hidden)
            {
                img.Dispose();
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
                flipped = i > 90;
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
                    Image img = RotateFace(vbase ??= RenderFace(card.Visible), flipped ? i - 180 : i); //the rendered image
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    img.Save(path);
                    Sprites.Add(img);
                }

                //repeat above but with the hidden sprite
                flipped = !flipped;
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
                    Image img = RotateFace(hbase ??= RenderFace(card.Hidden), flipped ? i : i - 180); //the rendered image
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    img.Save(path);
                    Sprites.Add(img);
                }
            }

            //disposes any base images that were rendered
            vbase?.Dispose();
            hbase?.Dispose();

            //resets movement variables
            going = false;
            counter = 0;

            return true; //repaint the control
        }

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
                e.Graphics.DrawPath(p, boundsPath);
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
                boundsPath.Reset();
                if (Sprites.Count > 0)
                {
                    float WFACTOR = Width / IMGWIDTH;
                    float HFACTOR = Height / IMGHEIGHT;
                    float WDIAMETER = WFACTOR * Math.Min(DIAMETER, BASEWIDTH);
                    float HDIAMETER = HFACTOR * Math.Min(DIAMETER, BASEHEIGHT);
                    float LEFT = WFACTOR * IMGHEIGHT * FACTOR * 0.5f;
                    float RIGHT = WFACTOR * (IMGWIDTH - WDIAMETER) - LEFT;
                    float BOTTOM = HFACTOR * (IMGHEIGHT - HDIAMETER);

                    boundsPath.AddArc(LEFT, 0, WDIAMETER, HDIAMETER, 180, 90);
                    boundsPath.AddArc(RIGHT, 0, WDIAMETER, HDIAMETER, 270, 90);
                    boundsPath.AddArc(RIGHT, BOTTOM, WDIAMETER, HDIAMETER, 0, 90);
                    boundsPath.AddArc(LEFT, BOTTOM, WDIAMETER, HDIAMETER, 90, 90);
                    boundsPath.CloseFigure();

                    oldSize = Size;
                }
                else
                {
                    boundsPath.AddRectangle(Bounds);
                }
            }

            return boundsPath.IsVisible(p);
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
