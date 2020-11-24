using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private readonly string CARD_DIR = Path.Combine(Application.UserAppDataPath, "CARD");

        /// <summary>
        /// The default font to use when none is specified
        /// </summary>
        private readonly Font FONT_DEF = new Font("Arial", 18);

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

            MessageBox.Show(JsonConvert.SerializeObject(Cards.Keys));
            foreach (var i in Cards)
            {
                MessageBox.Show(JsonConvert.SerializeObject(i));
                RenderCard(i.Value.visible, i.Key + ".png");
            }

            var f = new Flashcard() { tags = new string[] { "first" }, hidden = new Flashcard.Face(), visible = new Flashcard.Face() { text = "first card", backColor = Color.SkyBlue, foreColor = Color.DeepSkyBlue, font = new Font("OCR A", 96, FontStyle.Italic | FontStyle.Underline), textBox = new Rectangle(0, 0, 1000, 1000), imagePath = @"C:\Users\Admin\Downloads\96LB_BR.png", imageBox = new Rectangle(200, 200, 400, 100), textFormat = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far } } };
            f.Save(Path.Combine(CARD_DIR, "first.fler"));
            new Flashcard() { hidden = new Flashcard.Face(), visible = new Flashcard.Face() }.Save(Path.Combine(CARD_DIR, "empty.fler"));

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

                //loads the assembly stream from the embedded resources
                using (Stream stream = GetType().Assembly.GetManifestResourceStream(assembly))
                {
                    byte[] data = new byte[stream.Length]; //the byte data of the assembly
                    stream.Read(data, 0, data.Length);
                    return Assembly.Load(data); //loads the assembly from the raw bytes
                }
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
                        Cards.Add(name, card);
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
            NextCard();
            if (CurrentCard != null)
            {
                UpdateCard(MessageBox.Show(JsonConvert.SerializeObject(CurrentCard), CurrentDir, MessageBoxButtons.YesNo) == DialogResult.Yes);
            }
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



        #endregion

        ///TEST CODE
        #region TEST CODE

        private void button1_Click(object sender, EventArgs e)
        {
            DrawCard();
        }

        void RenderCard(Flashcard.Face face, string name)
        {
            ///note: not final implementation
            ///note: c# 8.0 using syntax

            const int RADIUS = 48;
            const int DIAMETER = RADIUS * 2;
            const int OUTLINE = 4;
            const int OUT = OUTLINE / 2;

            const int IMGWIDTH = 960;
            const int IMGHEIGHT = 640;

            const int LEFT = RADIUS + OUT;
            const int RIGHT = IMGWIDTH - RADIUS - OUT;
            const int TOP = RADIUS + OUT;
            const int BOTTOM = IMGHEIGHT - RADIUS - OUT;

            const int ARCRIGHT = RIGHT - RADIUS;
            const int ARCBOTTOM = BOTTOM - RADIUS;

            using (Bitmap bmp = new Bitmap(IMGWIDTH, IMGHEIGHT))
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    GraphicsPath path = new GraphicsPath();
                    path.AddArc(OUT, OUT, DIAMETER, DIAMETER, 180, 90);
                    path.AddArc(ARCRIGHT, OUT, DIAMETER, DIAMETER, 270, 90);
                    path.AddArc(ARCRIGHT, ARCBOTTOM, DIAMETER, DIAMETER, 0, 90);
                    path.AddArc(OUT, ARCBOTTOM, DIAMETER, DIAMETER, 90, 90);
                    path.CloseFigure();
                    using (Brush p = new SolidBrush(face.backColor))
                    {
                        graphics.FillPath(p, path);
                    }
                    using (Pen p = new Pen(face.foreColor, OUTLINE))
                    {
                        graphics.DrawPath(p, path);
                    }

                    using (Region clip = new Region(path))
                    {  
                        graphics.TranslateTransform(LEFT, TOP);
                        graphics.Clip = clip;

                        using (Brush p = new SolidBrush(face.foreColor))
                        {
                            graphics.IntersectClip(face.textBox);
                            graphics.DrawString(face.text, face.font ?? FONT_DEF, p, face.textBox, face.textFormat);
                        }

                        if (face.imagePath != null)
                        {
                            try
                            {
                                graphics.Clip = clip;
                                graphics.IntersectClip(face.imageBox);
                                using (Image img = Image.FromFile(face.imagePath))
                                {
                                    graphics.DrawImage(img, face.imageBox);
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
                bmp.Save(name);
            }
        }

        #endregion
        ///TEST CODE

    }
}