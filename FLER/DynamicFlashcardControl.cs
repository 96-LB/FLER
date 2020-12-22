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
    /// <summary>
    /// Renders a flashcard with a dynamic flip animation
    /// </summary>
    class DynamicFlashcardControl : StaticFlashcardControl
    {

        #region Properties

        /// <summary>
        /// [Internal] The list of sprites for the currently selected face
        /// </summary>
        private List<Image> Sprites { get => Flipped ? _hidden : _visible; }

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

        //inherited docstring
        protected override Image ToPaint { get => Sprites[_counter]; }

        #endregion

        #region Fields

        /// <summary>
        /// [Internal] Whether the flashcard is currently flipping
        /// </summary>
        private bool _moving = false;

        /// <summary>
        /// [Internal] The index of the current sprite to display
        /// </summary>
        private int _counter = 0;

        #endregion

        #region Constants
        
        /// <summary>
        /// The perspective factor used when rotating the flashcard sprites
        /// </summary>
        public const float FACTOR = 0.25f;

        /// <summary>
        /// The width of a full flashcard sprite, including room for rotation
        /// </summary>
        public const float IMGWIDTH = WIDTH + FACTOR * HEIGHT;

        /// <summary>
        /// The height of a full flashcard sprite, including room for rotation
        /// </summary>
        public const float IMGHEIGHT = HEIGHT;

        /// <summary>
        /// The number of degrees rotated in between each sprite
        /// </summary>
        public const int INTERVAL = 3;

        #endregion

        #region Methods

        #region Private Static

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

        //inherits docstring
        public override bool InBounds(Point parent)
        {
            PointF local = PointToLocal(parent);
            PointF translated = PointF.Subtract(local, new SizeF(0.5f * FACTOR * HEIGHT / IMGWIDTH * Width, 0));
            PointF scaled = new PointF(translated.X * IMGWIDTH / WIDTH, translated.Y);
            return base.InBounds(PointToParent(Point.Round(scaled)));
        }

        /// <summary>
        /// Loads sprites from the given flashcard, rendering any missing ones
        /// </summary>
        /// <param name="card">The flashcard to render and load</param>
        /// <returns>Whether the control requires a paint event</returns>
        public override bool LoadCard(Flashcard card)
        {
            //calls the base function to load static sprites
            base.LoadCard(card);

            //disposes and clears all of the loaded sprite images
            foreach (Image image in _visible)
            {
                image?.Dispose();
            }
            foreach (Image image in _hidden)
            {
                image?.Dispose();
            }
            _visible.Clear();
            _hidden.Clear();

            //generates sprites across a 180 degree rotation, using the specified interval
            for (int i = 0; i < 180; i += INTERVAL)
            {
                //if past 90 degrees, add each face's sprites to the opposite list
                Flipped = i > 90;
                string vpath = Path.Combine(FLERForm.IMG_DIR, card.Filename, "v", i + ".png"); //the visible sprite's stored image path
                try
                {
                    //load the stored image if it exists
                    Sprites.Add(Image.FromFile(vpath));
                }
                catch
                {
                    bool flip = Flipped; //temp value for the flipped variable
                    
                    //grabs the base image to render
                    Flipped = false;
                    Image face = Sprite;
                    Flipped = flip;

                    //if there is no stored image, generate it and save it
                    Image image = RotateFace(face, Flipped ? i - 180 : i); //the rendered image
                    Directory.CreateDirectory(Path.GetDirectoryName(vpath));
                    image.Save(vpath);
                    Sprites.Add(image);
                }

                //repeat above but with the hidden sprite
                Flipped = !Flipped;
                string hpath = Path.Combine(FLERForm.IMG_DIR, card.Filename, "h", i + ".png"); //the hidden sprite's stored image path
                try
                {
                    //load the stored image if it exists
                    Sprites.Add(Image.FromFile(hpath));
                }
                catch
                {
                    bool flip = Flipped; //temp value for Flipped

                    //grabs the base image to render
                    Flipped = true;
                    Image face = Sprite;
                    Flipped = flip;

                    //if there is no stored image, generate it and save it
                    //otherwise, generate it and save it
                    Image image = RotateFace(face, Flipped ? i : i - 180); //the rendered image
                    Directory.CreateDirectory(Path.GetDirectoryName(hpath));
                    image.Save(hpath);
                    Sprites.Add(image);
                }
            }

            //resets movement variables
            Flipped = true;
            _counter = Sprites.Count;
            Tick();

            return true; //returns true to repaint the control
        }

        /// <summary>
        /// Invokes a flip event for the flashcard if it is currently flipping
        /// </summary>
        /// <returns>Whether the control requires a paint event</returns>
        public bool Tick()
        {
            bool output = _moving; //whether the control needs a repaint

            //if the flashcard is flipping, increment the counter
            if(_moving)
            {
                _counter++;
            }

            //if the counter equals the number of sprites, the flip is finished
            if (_counter >= Sprites.Count)
            {
                //reset movement variables
                _counter = 0;
                Flipped = !Flipped;
                _moving = false;
                Cursor = _mouse ? Cursors.Hand : Cursors.Default;
            }

            return output; //returns true if the flashcard was flipping to repaint the control
        }

        #endregion

        #endregion

        #region Events

        //inherits docstring
        public override bool MouseMove(MouseEventArgs e)
        {
            bool repaint = base.MouseMove(e); //whether the control needs a repaint
            
            //if the control is moving, reset the cursor
            Cursor = _moving ? Cursors.Default : Cursor;
            
            return repaint; //returns the base method to determine whether to repaint
        }

        //inherits docstring
        public override bool Flip()
        {
            //begins flipping
            _moving = true;
            return true; //returns true to repaint the control
        }

        #endregion

    }
}
