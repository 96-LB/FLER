using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FLER
{
    public partial class FLERForm : Form
    {

        #region Fields

        /// <summary>
        /// The flashcard directory location
        /// </summary>
        public static readonly string CARD_DIR = Path.Combine(Application.UserAppDataPath, "CARD");

        /// <summary>
        /// The image directory location
        /// </summary>
        public static readonly string IMG_DIR = Path.Combine(CARD_DIR, "IMG");

        /// <summary>
        /// The default font to use when none is specified
        /// </summary>
        public static readonly Font FONT_DEF = new Font("Arial", 18);

        /// <summary>
        /// A pseudorandom number generator
        /// </summary>
        private readonly Random RAND = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// The list of loaded flashcards
        /// </summary>
        Dictionary<string, Flashcard> Cards { get; set; } = new Dictionary<string, Flashcard>();

        /// <summary>
        /// The filename of the flashcard currently being reviewed
        /// </summary>
        string CurrentDir { get; set; }

        /// <summary>
        /// The flashcard currently being reviewed
        /// </summary>
        Flashcard CurrentCard { get; set; }

        #endregion


        #region Constructor

        public FLERForm()
        {
            LoadDependencies();
            InitializeComponent();
            LoadCards();
            /// TEST CODE
            TESTCODE();
            /// TEST CODE
        }

        /// TEST CODE
        void TESTCODE()
        {

            var f = new Flashcard()
            {
                tags = new string[] { "first" },
                hidden = new Flashcard.Face() { text = "first card", backColor = Color.SkyBlue, foreColor = Color.DeepSkyBlue, font = new Font("OCR A Extended", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), textBox = new Rectangle(0, 0, 500, 500), imagePath = @"C:\Users\Admin\Downloads\96LB_BRR.png", imageBox = new Rectangle(-250, -250, 750, 750) },
                visible = new Flashcard.Face() { text = "first card", backColor = Color.SkyBlue, foreColor = Color.DeepSkyBlue, font = new Font("OCR A Extended", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), textBox = new Rectangle(0, 0, 500, 500), imagePath = @"C:\Users\Admin\Downloads\96LB_BR.png", imageBox = new Rectangle(-250, -250, 750, 750) }
            };
            //f.Save("first.fler");
            //new Flashcard() { hidden = new Flashcard.Face(), visible = new Flashcard.Face() }.Save("empty.fler");
            f = new Flashcard()
            {
                tags = new string[] { "first" },
                hidden = new Flashcard.Face() { text = "first card", backColor = Color.DarkMagenta, foreColor = Color.Magenta, font = new Font("LaBuff_IMP3_Typeface", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), textBox = new Rectangle(0, 0, 480, 320), imageBox = new Rectangle(-250, -250, 750, 750), textFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center } },
                visible = new Flashcard.Face() { text = "you did it!", backColor = Color.DarkMagenta, foreColor = Color.Magenta, font = new Font("LaBuff_IMP3_Typeface", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), textBox = new Rectangle(0, 0, 480, 320), imagePath = @"C:\Users\Admin\Downloads\96LB_BR.png", textFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center } }
            };
            //f.Save("ff.fler");
            Flashcard.TryLoad("ff.fler", out f);
            LoadCard(f, "ff.fler");
            controls.Add(fc);
        }
        /// TEST CODE

        #endregion

        #region Methods

        /// <summary>
        /// Loads assemblies that are embedded into the project
        /// </summary>
        private void LoadDependencies()
        {
            //adds a hook to the event that is fired when an assembly name can't be resolved
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string name = new AssemblyName(args.Name).Name + ".dll"; //the name of the assembly
                string assembly = GetType().Assembly.GetManifestResourceNames().First(x => x.EndsWith(name)); //the loaded assemply
                using Stream stream = GetType().Assembly.GetManifestResourceStream(assembly); //the assembly stream from the embedded resources

                byte[] data = new byte[stream.Length]; //the byte data of the assembly
                stream.Read(data, 0, data.Length);

                return Assembly.Load(data); //loads the assembly from the raw bytes
            };
        }

        /// <summary>
        /// Loads the list of flashcards from the user's app data directory
        /// </summary>
        private void LoadCards()
        {
            try
            {
                //attempts to load every file in the card directory
                foreach (string name in Directory.EnumerateFiles(CARD_DIR))
                {
                    //if a flashcard can be loaded
                    if (Flashcard.TryLoad(name, out Flashcard card))
                    {
                        //add it to the list
                        Cards.Add(Path.GetFileName(name), card);
                    }
                    else
                    {
                        //otherwise, delete the file
                        if (File.Exists(name))
                        {
                            File.Delete(name);
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                //if the directory doesn't exist, create it
                Directory.CreateDirectory(CARD_DIR);
            }
        }

        /// <summary>
        /// Draws a card from the pool and reviews it
        /// </summary>
        private void DrawCard()
        {
            ///note: not final implementation
            if (CurrentCard != null)
            {
                UpdateCard(checkBox1.Checked);
            }
            NextCard();
            if (CurrentCard != null)
            {
                checkBox1.Text = "" + CurrentCard.level;
                Invalidate(fc.Bounds);
            }
            else
            {
                checkBox1.Text = "null";
            }
            fc.Width += RAND.Next(-50, 51);
        }

        /// <summary>
        /// Identifies and selects the next card to be reviewed
        /// </summary>
        private void NextCard()
        {
            ///note: not final implementation
            ///TEST CODE: CHANGE TimeSpan.FromSeconds TO TimeSpan.FromDays ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////         
            //returns a random card in the list whose last-reviewed time is at least 2^level days ago
            KeyValuePair<string, Flashcard> current = Cards.OrderBy(x => RAND.Next()).FirstOrDefault(x => DateTime.UtcNow - x.Value.date >= TimeSpan.FromSeconds(Math.Pow(2, x.Value.level)));

            //sets the directory and card
            CurrentDir = current.Key;
            CurrentCard = current.Value;

            //load the sprites of the selected card
            if (CurrentCard != null)
            {
                LoadCard(CurrentCard, CurrentDir);
            }
        }

        /// <summary>
        /// Updates the current card based on the review verdict
        /// </summary>
        /// <param name="levelUp">Whether the card is moving up a level</param>
        private void UpdateCard(bool levelUp)
        {
            ///note: not final implementation
            //sets the card's last review date and adjusts the card's level accordingly 
            CurrentCard.date = DateTime.UtcNow;
            CurrentCard.level = levelUp ? CurrentCard.level + 1 : 0;
            CurrentCard.Save(CurrentDir);
        }

        #endregion

        #region Events

        //no final events :(

        #endregion

        ///TEST CODE
        #region TEST CODE

        private void button1_Click(object sender, EventArgs e)
        {
            DrawCard();
        }

        Bitmap RenderFace(Flashcard.Face face)
        {
            ///note: not final implementation
            const int RADIUS = 24;
            const int DIAMETER = RADIUS * 2;
            const int OUTLINE = 2;
            const int OUT = OUTLINE / 2;

            const int IMGWIDTH = 480;
            const int IMGHEIGHT = 320;

            const int RIGHT = IMGWIDTH - DIAMETER - OUT;
            const int BOTTOM = IMGHEIGHT - DIAMETER - OUT;

            Bitmap bmp = new Bitmap(IMGWIDTH, IMGHEIGHT);
            using Graphics graphics = Graphics.FromImage(bmp);
            using GraphicsPath path = new GraphicsPath();
            using Brush backColor = new SolidBrush(face.backColor);
            using Brush foreColor = new SolidBrush(face.foreColor);
            using Pen forePen = new Pen(face.foreColor, OUTLINE);

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
                graphics.IntersectClip(face.textBox);
                graphics.DrawString(face.text, face.font ?? FONT_DEF, foreColor, face.textBox, face.textFormat);
            }

            void renderImage()
            {
                if (face.imagePath != null)
                {
                    graphics.SetClip(path);
                    graphics.IntersectClip(face.imageBox);
                    try
                    {
                        using Image img = Image.FromFile(face.imagePath);
                        graphics.DrawImage(img, face.imageBox);
                    }
                    catch
                    {
                        graphics.DrawImage(Properties.Resources.Missing, face.imageBox);
                    }
                }
            }

            if (face.imageTop)
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
            const double FACTOR = 0.25;

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

        readonly FlashcardControl fc = new FlashcardControl() { Bounds = new Rectangle(100, 100, 720, 160), Factor = 0.25f, Radius = 24 };

        void LoadCard(Flashcard card, string filename)
        {
            const int INTERVAL = 3;

            foreach (Image img in fc.visible)
            {
                img.Dispose();
            }
            foreach (Image img in fc.hidden)
            {
                img.Dispose();
            }
            fc.visible.Clear();
            fc.hidden.Clear();

            string vguid = Guid.NewGuid().ToString();
            string hguid = Guid.NewGuid().ToString();

            fc.visible.Add(null);
            fc.hidden.Add(null);

            for (int i = 0; i < 180; i += INTERVAL)
            {
                fc.flipped = i > 90;
                try
                {
                    fc.Sprites.Add(Image.FromFile(card.visible.sprites[i]));
                }
                catch
                {
                    Image img = RotateFace(fc.visible[0] ??= RenderFace(card.visible), fc.flipped ? i - 180 : i);
                    string path = Path.Combine(IMG_DIR, vguid, i + ".png");
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    img.Save(path);
                    card.visible.sprites[i] = path;
                    fc.Sprites.Add(img);
                }

                fc.flipped = !fc.flipped;
                try
                {
                    fc.Sprites.Add(Image.FromFile(card.hidden.sprites[i]));
                }
                catch
                {
                    Image img = RotateFace(fc.hidden[0] ??= RenderFace(card.hidden), fc.flipped ? i : i - 180);
                    string path = Path.Combine(IMG_DIR, hguid, i + ".png");
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    img.Save(path);
                    card.hidden.sprites[i] = path;
                    fc.Sprites.Add(img);
                }
            }

            if (fc.visible[0] != null || fc.hidden[0] != null)
            {
                card.Save(filename);
            }

            fc.visible.RemoveAt(0);
            fc.hidden.RemoveAt(0);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "" + (1 + int.Parse(label2.Text));
            if (fc.Flip())
            {
                Invalidate(fc.Bounds);
            }
        }

        List<FLERControl> controls = new List<FLERControl>();
        FLERControl hover;
        FLERControl selected;
        private void FLERForm_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;

            foreach (FLERControl f in controls)
            {
                if (f.Bounds.IntersectsWith(e.ClipRectangle) && f.Paint(e))
                {
                    Invalidate(f.Bounds);
                }
            }

            Cursor = hover?.Cursor ?? Cursors.Default;
        }

        private void FLERForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (hover?.MouseDown(e) == true)
            {
                Invalidate(hover.Bounds);
            }
            selected = hover;
        }

        private void FLERForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (hover?.MouseUp(e) == true)
            {
                Invalidate(hover.Bounds);
            }
            if (selected == hover && selected?.Click(e) == true)
            {
                Invalidate(selected.Bounds);
            }
            selected = null;
        }

        private void FLERForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (hover?.MouseMove(e) == true)
            {
                Invalidate(hover.Bounds);
            }

            FLERControl next = null;
            Point pointer = PointToClient(Cursor.Position);
            if (new Rectangle(Point.Empty, ClientRectangle.Size).Contains(pointer))
            {
                if (selected == null)
                {
                    foreach (FLERControl control in controls)
                    {
                        if (control.Bounds.Contains(pointer))
                        {
                            next = control;
                            break;
                        }
                    }
                }
                else
                {
                    if (selected.Bounds.Contains(pointer))
                    {
                        next = selected;
                    }
                }
            }

            if (next != hover)
            {
                if (hover?.MouseLeave(e) == true)
                {
                    Invalidate(hover.Bounds);
                }
                if (next?.MouseEnter(e) == true)
                {
                    Invalidate(next.Bounds);
                }
            }

            hover = next;
            Cursor = hover?.Cursor ?? Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void FLERForm_MouseLeave(object sender, EventArgs e)
        {
            if (hover?.MouseLeave(e) == true)
            {
                Invalidate(hover.Bounds);
            }
            hover = null;
        }

        #endregion

        ///TEST CODE
    }
}