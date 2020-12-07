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
                Filename = "first.fler",
                Tags = new string[] { "first" },
                Hidden = new Flashcard.Face() { Text = "first card", BackColor = Color.SkyBlue, ForeColor = Color.DeepSkyBlue, Font = new Font("OCR A Extended", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), TextBox = new Rectangle(0, 0, 500, 500), ImagePath = @"C:\Users\Admin\Downloads\96LB_BRR.png", ImageBox = new Rectangle(-250, -250, 750, 750) },
                Visible = new Flashcard.Face() { Text = "first card", BackColor = Color.SkyBlue, ForeColor = Color.DeepSkyBlue, Font = new Font("OCR A Extended", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), TextBox = new Rectangle(0, 0, 500, 500), ImagePath = @"C:\Users\Admin\Downloads\96LB_BR.png", ImageBox = new Rectangle(-250, -250, 750, 750) }
            };
            //f.Save();
            //new Flashcard() { Filename = "empty.fler", hidden = new Flashcard.Face(), visible = new Flashcard.Face() }.Save();
            f = new Flashcard()
            {
                Filename = "ff.fler",
                Tags = new string[] { "first" },
                Hidden = new Flashcard.Face() { Text = "first card", BackColor = Color.DarkMagenta, ForeColor = Color.Magenta, Font = new Font("LaBuff_IMP3_Typeface", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), TextBox = new Rectangle(0, 0, 480, 320), ImageBox = new Rectangle(-250, -250, 750, 750), TextFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center } },
                Visible = new Flashcard.Face() { Text = "you did it!", BackColor = Color.DarkMagenta, ForeColor = Color.Magenta, Font = new Font("LaBuff_IMP3_Typeface", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), TextBox = new Rectangle(0, 0, 480, 320), ImagePath = @"C:\Users\Admin\Downloads\96LB_BR.png", TextFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center } }
            };
            //f.Save("ff.fler");
            Flashcard.TryLoad("ff.fler", out f);
            if (fc.LoadCard(f))
            {
                Invalidate();
            }
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
                    if (Flashcard.TryLoad(Path.GetFileName(name), out Flashcard card))
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
        /// Draws a flashcard from the pool and reviews it
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
                checkBox1.Text = "" + CurrentCard.Level;
                Invalidate(fc.Bounds);
            }
            else
            {
                checkBox1.Text = "null";
            }
            fc.Width += RAND.Next(-50, 51);
        }

        /// <summary>
        /// Identifies and selects the next flashcard to be reviewed
        /// </summary>
        private void NextCard()
        {
            ///note: not final implementation
            ///TEST CODE: CHANGE TimeSpan.FromSeconds TO TimeSpan.FromDays ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////         
            //chooses a random flashcard in the list whose last-reviewed time is at least 2^level days ago
            CurrentCard = Cards.Values.OrderBy(x => RAND.Next()).FirstOrDefault(x => DateTime.UtcNow - x.Date >= TimeSpan.FromSeconds(Math.Pow(2, x.Level)));

            //load the sprites of the selected flashcard
            if (CurrentCard != null && fc.LoadCard(CurrentCard))
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Updates the current flashcard based on the review verdict
        /// </summary>
        /// <param name="levelUp">Whether the flashcard is moving up a level</param>
        private void UpdateCard(bool levelUp)
        {
            ///note: not final implementation
            //sets the flashcard's last review date and adjusts the flashcard's level accordingly 
            CurrentCard.Date = DateTime.UtcNow;
            CurrentCard.Level = levelUp ? CurrentCard.Level + 1 : 0;
            CurrentCard.Save();
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

        readonly FlashcardControl fc = new FlashcardControl() { Bounds = new Rectangle(100, 100, (int)FlashcardControl.IMGWIDTH, (int)FlashcardControl.IMGHEIGHT) };

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