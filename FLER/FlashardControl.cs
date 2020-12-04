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
        /// The default font to use when none is specified
        /// </summary>
        public static readonly Font FONT_DEF = new Font("Arial", 18);

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

        public const float FACTOR = 0.25f;
        public const int RADIUS = 24;
        public const int DIAMETER = RADIUS * 2;
        public const int OUTLINE = 2;
        public const int OUT = OUTLINE / 2;

        public const int BASEWIDTH = 480;
        public const int BASEHEIGHT = 320;

        public const float IMGWIDTH = BASEWIDTH + FACTOR * BASEHEIGHT;
        public const float IMGHEIGHT = BASEHEIGHT;

        public const int RIGHT = BASEWIDTH - DIAMETER - OUT;
        public const int BOTTOM = BASEHEIGHT - DIAMETER - OUT;

        public const int INTERVAL = 3;

        public bool flipped, going;
        bool mouse = false;
        int counter = 0;
        bool outline = false;
        Size oldSize = Size.Empty;
        GraphicsPath path = new GraphicsPath();

        Bitmap RenderFace(Flashcard.Face face)
        {
            ///note: not final implementation]
            


            Bitmap bmp = new Bitmap(BASEWIDTH, BASEHEIGHT);
            using Graphics graphics = Graphics.FromImage(bmp);
            using GraphicsPath path = new GraphicsPath();
            using Brush backColor = new SolidBrush(face.BackColor);
            using Brush foreColor = new SolidBrush(face.ForeColor);
            using Pen forePen = new Pen(face.ForeColor, OUTLINE);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            path.AddArc(OUT, OUT, DIAMETER, DIAMETER, 180, 90);
            path.AddArc(RIGHT, OUT, DIAMETER, DIAMETER, 270, 90);
            path.AddArc(RIGHT, BOTTOM, DIAMETER, DIAMETER, 0, 90);
            path.AddArc(OUT, BOTTOM, DIAMETER, DIAMETER, 90, 90);
            path.CloseFigure();


            graphics.FillPath(backColor, path);

            void renderText()
            {
                graphics.SetClip(path);
                graphics.IntersectClip(face.TextBox);
                graphics.DrawString(face.Text, face.Font ?? FONT_DEF, foreColor, face.TextBox, face.TextFormat);
            }

            void renderImage()
            {
                if (face.ImagePath != null)
                {
                    graphics.SetClip(path);
                    graphics.IntersectClip(face.ImageBox);
                    try
                    {
                        using Image img = Image.FromFile(face.ImagePath);
                        graphics.DrawImage(img, face.ImageBox);
                    }
                    catch
                    {
                        graphics.DrawImage(Properties.Resources.Missing, face.ImageBox);
                    }
                }
            }

            if (face.ImageTop)
            {
                renderText();
                renderImage();
            }
            else
            {
                renderImage();
                renderText();
            }

            graphics.ResetClip();
            graphics.DrawPath(forePen, path);

            return bmp;
        }


        Bitmap RotateFace(Image img, int degrees)
        {
            ///note: not final implementation
            static Graphics qualityGraphics(Bitmap bmp)
            {
                Graphics graphics = Graphics.FromImage(bmp);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                return graphics;
            }

            static int round(double num) => (int)Math.Round(num);

            using Bitmap bmp = new Bitmap(img.Width + round(img.Height * FACTOR), img.Height);
            using Graphics graphics = qualityGraphics(bmp);
            using ImageAttributes attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            double rad = degrees * Math.PI / 180.0;
            double sin = FACTOR * Math.Sin(rad);
            double cos = Math.Cos(rad);

            for (int i = 0; i < img.Height; i++)
            {
                int width = round(2 * sin * ((double)i / img.Height - 0.5) * img.Height + img.Width);
                graphics.DrawImage(img,
                    new Rectangle((bmp.Width - width) / 2, i, width, 1),
                    0, i, img.Width, 1,
                    GraphicsUnit.Pixel, attributes);
            }
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);
            using Graphics outgraphics = qualityGraphics(output);

            outgraphics.DrawImage(bmp,
                new Rectangle(0, round(0.5 * (1 - cos) * output.Height), output.Width, round(cos * output.Height)),
                0, 0, bmp.Width, bmp.Height,
                GraphicsUnit.Pixel, attributes);

            return output;
        }

        public bool LoadCard(Flashcard card)
        {
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

            _visible.Add(null);
            _hidden.Add(null);

            string path;
            for (int i = 0; i < 180; i += INTERVAL)
            {
                flipped = i > 90;
                path = Path.Combine(FLERForm.IMG_DIR, card.Filename, "v", i + ".png");
                try
                {
                    Sprites.Add(Image.FromFile(path));
                }
                catch
                {
                    Image img = RotateFace(_visible[0] ??= RenderFace(card.Visible), flipped ? i - 180 : i);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    img.Save(path);
                    Sprites.Add(img);
                }

                flipped = !flipped;
                path = Path.Combine(FLERForm.IMG_DIR, card.Filename, "h", i + ".png");
                try
                {
                    Sprites.Add(Image.FromFile(path));
                }
                catch
                {
                    Image img = RotateFace(_hidden[0] ??= RenderFace(card.Hidden), flipped ? i : i - 180);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    img.Save(path);
                    Sprites.Add(img);
                }
            }

            _visible.RemoveAt(0);
            _hidden.RemoveAt(0);

            return true;
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
                    float WFACTOR = Width / IMGWIDTH;
                    float HFACTOR = Height / IMGHEIGHT;
                    float WDIAMETER = WFACTOR * Math.Min(IMGWIDTH, DIAMETER);
                    float HDIAMETER = HFACTOR * Math.Min(IMGHEIGHT, DIAMETER);
                    float PLEFT = WFACTOR * IMGHEIGHT * FACTOR * 0.5f;
                    float PRIGHT = WFACTOR * (IMGWIDTH - WDIAMETER) - PLEFT;
                    float PBOTTOM = HFACTOR * (IMGHEIGHT - HDIAMETER);

                    path.AddArc(PLEFT, 0, WDIAMETER, HDIAMETER, 180, 90);
                    path.AddArc(PRIGHT, 0, WDIAMETER, HDIAMETER, 270, 90);
                    path.AddArc(PRIGHT, PBOTTOM, WDIAMETER, HDIAMETER, 0, 90);
                    path.AddArc(PLEFT, PBOTTOM, WDIAMETER, HDIAMETER, 90, 90);
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
