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
using System.Text.RegularExpressions;
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
        private Dictionary<string, Flashcard> Cards { get; set; } = new Dictionary<string, Flashcard>();

        /// <summary>
        /// The flashcard currently being reviewed
        /// </summary>
        private Flashcard CurrentCard { get; set; } = Flashcard.Default;

        /// <summary>
        /// Whether the program is in building mode
        /// </summary>
        private bool Building { get; set; } = false;

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
                Invalidate(sfc_builder.Bounds);
            }
            else
            {
                checkBox1.Text = "null";
            }
            sfc_builder.Width += RAND.Next(-50, 51);
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
            if (CurrentCard != null && sfc_builder.LoadCard(CurrentCard))
            {
                Invalidate(sfc_builder.Bounds);
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

        #region PickColor

        /// <summary>
        /// A dialog with which to choose a color
        /// </summary>
        static private readonly ColorDialog _colorDialog = new ColorDialog() { SolidColorOnly = true, FullOpen = true };

        /// <summary>
        /// Opens a dialog to choose a color
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An object that contains no event data</param>
        private void PickColor(object sender, EventArgs e)
        {
            //sets the dialog color to match the panel that was clicked
            _colorDialog.Color = ((Panel)sender).BackColor;

            //if the user clicked ok, set the control's color
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Panel)sender).BackColor = _colorDialog.Color;

                //updates the flashcard
                BeginUpdateBuilder(null, null);
            }
        }

        #endregion

        #region PickImage

        /// <summary>
        /// A string representing the list of valid image file extensions
        /// </summary>
        static private readonly string _extension = string.Join(";", ImageCodecInfo.GetImageEncoders().Select(x => x.FilenameExtension));

        /// <summary>
        /// A dialog with which to choose an image file
        /// </summary>
        static private readonly OpenFileDialog _fileDialog = new OpenFileDialog()
        {
            Filter = $"Image files ({_extension.ToLower().Replace(";", ", ")})|{_extension}"
        };

        /// <summary>
        /// Opens a dialog to choose an image file
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An object that contains no event data</param>
        private void PickImage(object sender, EventArgs e)
        {
            //if the user clicked ok, try to set the control's image
            if (_fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //try to read an image from the specified filepath
                    ((PictureBox)sender).Image = Image.FromFile(_fileDialog.FileName);
                    ((PictureBox)sender).ImageLocation = _fileDialog.FileName;
                    BeginUpdateBuilder(null, null);
                }
                catch
                {
                    //if it fails, the path is invalid
                    MessageBox.Show("The selected file is not a valid image.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region PickFont

        /// <summary>
        /// A dialog with which to choose a font
        /// </summary>
        static private readonly FontDialog _fontDialog = new FontDialog() { AllowScriptChange = false };

        /// <summary>
        /// Opens a dialog to choose a font
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An object that contains no event data</param>
        private void PickFont(object sender, EventArgs e)
        {
            Label label = sender as Label; //the label which fired this event

            //if the user clicked ok, set the label's font
            if (_fontDialog.ShowDialog() == DialogResult.OK)
            {
                //updates reversely by changing the card's font first and using that to update the control
                (sfc_builder.Flipped ? CurrentCard.Hidden : CurrentCard.Visible).Font = _fontDialog.Font;
                sfc_builder.LoadCard(CurrentCard);
                UpdateBuilderControls(null, null);
                Invalidate();
            }
        }

        #endregion

        #endregion

        ///TEST CODE
        #region TEST CODE

        /// TEST CODE
        void TESTCODE()
        {

            var f = new Flashcard()
            {
                Filename = "phil.fler",
                Tags = new string[] { "phil" },
                Hidden = new Flashcard.Face() { TextColor = Color.White, ImagePath = @"C:\Users\Admin\Downloads\2.png", ImageBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT) },
                Visible = new Flashcard.Face() { LineColor = Color.DarkGray, Text = "so, phil, is it?", TextColor = Color.White, Font = new Font("Times New Roman", 32), TextAlign = ContentAlignment.MiddleCenter, TextBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT), ImagePath = @"C:\Users\Admin\Downloads\1.png", ImageBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT) }
            };
            //f.Save();
            //new Flashcard() { Filename = "empty.fler", hidden = new Flashcard.Face(), visible = new Flashcard.Face() }.Save();
            f = new Flashcard()
            {
                Filename = "ff.fler",
                Tags = new string[] { "first" },
                Hidden = new Flashcard.Face() { Text = "first card", BackColor = Color.DarkMagenta, TextColor = Color.Magenta, Font = new Font("LaBuff_IMP3_Typeface", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), TextBox = new Rectangle(0, 0, 480, 320), ImageBox = new Rectangle(-250, -250, 750, 750), TextAlign = ContentAlignment.MiddleCenter },
                Visible = new Flashcard.Face() { Text = "you did it!", BackColor = Color.DarkMagenta, TextColor = Color.Magenta, Font = new Font("LaBuff_IMP3_Typeface", 48, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout), TextBox = new Rectangle(0, 0, 480, 320), ImagePath = @"C:\Users\Admin\Downloads\96LB_BR.png", TextAlign = ContentAlignment.MiddleCenter }
            };
            //f.Save("ff.fler");
            Flashcard.TryLoad("phil.fler", out f);
            if (sfc_builder.LoadCard(CurrentCard))
            {
                Invalidate(sfc_builder.Bounds);
            }
            controls.Add(sfc_builder);
            sfc_builder.OnClick += UpdateBuilderControls;
            UpdateBuilderControls(null, null);

            Building = true;
        }
        /// TEST CODE

        private void button1_Click(object sender, EventArgs e)
        {
            DrawCard();
        }

        readonly StaticFlashcardControl sfc_builder = new StaticFlashcardControl() { Bounds = new Rectangle(200, 100, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT) };

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label2.Text = "" + (1 + int.Parse(label2.Text));
            //if (sfc_builder.Tick())
            //{
            //    Invalidate(sfc_builder.Bounds);
            //}
        }


        private void BuildNewCard()
        {
            CurrentCard = Flashcard.Default;
            sfc_builder.LoadCard(CurrentCard);
            UpdateBuilderControls(null, null);
            Invalidate();
        }

        private void UpdateBuilderControls(object sender, EventArgs e)
        {
            Building = false;
            tim_builder.Stop();
            txt_tags.Text = string.Join(" ", CurrentCard.Tags ?? new string[0]);
            Flashcard.Face face = sfc_builder.Flipped != (sender == sfc_builder) ? CurrentCard.Hidden : CurrentCard.Visible;
            pnl_backcolor.BackColor = face.BackColor;
            pnl_linecolor.BackColor = face.LineColor;
            try
            {
                img_img.Image?.Dispose();
                img_img.Image = Image.FromFile(face.ImagePath);
                img_img.ImageLocation = face.ImagePath;
            }
            catch
            {
                img_img.Image = Properties.Resources.Missing;
                img_img.ImageLocation = null;
            }
            num_img_left.Value = face.ImageBox.Left;
            num_img_top.Value = face.ImageBox.Top;
            num_img_width.Value = face.ImageBox.Width;
            num_img_height.Value = face.ImageBox.Height;
            check_drawover.Checked = face.ImageTop;
            lbl_font.Text = $"{face.Font.FontFamily.Name}, {Math.Round(face.Font.Size)}pt";
            lbl_font.Font = new Font(face.Font.FontFamily, lbl_font.Font.Size, face.Font.Style);
            _fontDialog.Font = face.Font;
            txt_text.Text = face.Text;
            pnl_txt_color.BackColor = face.TextColor;
            num_txt_left.Value = face.TextBox.Left;
            num_txt_top.Value = face.TextBox.Top;
            num_txt_width.Value = face.TextBox.Width;
            num_txt_height.Value = face.TextBox.Height;
            foreach (Control control in pnl_builder.Controls)
            {
                if (control is RadioButton radio)
                {
                    radio.Checked = radio.TextAlign == face.TextAlign;
                }
            }
            Building = true;
        }

        private void UpdateBuilder(object sender, EventArgs e)
        {
            if (!Building)
            {
                return;
            }
            CurrentCard.Tags = txt_tags.Text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            Flashcard.Face face = sfc_builder.Flipped ? CurrentCard.Hidden : CurrentCard.Visible;
            face.BackColor = pnl_backcolor.BackColor;
            face.LineColor = pnl_linecolor.BackColor;
            face.ImagePath = img_img.ImageLocation;
            face.ImageBox = new Rectangle((int)Math.Round(num_img_left.Value), (int)Math.Round(num_img_top.Value), (int)Math.Round(num_img_width.Value), (int)Math.Round(num_img_height.Value));
            face.ImageTop = check_drawover.Checked;
            //note: font is omitted because it is directly set in PickFont()
            face.Text = txt_text.Text;
            face.TextColor = pnl_txt_color.BackColor;
            face.TextBox = new Rectangle((int)Math.Round(num_txt_left.Value), (int)Math.Round(num_txt_top.Value), (int)Math.Round(num_txt_width.Value), (int)Math.Round(num_txt_height.Value));
            foreach (Control control in pnl_builder.Controls)
            {
                if (control is RadioButton radio)
                {
                    if (radio.Checked)
                    {
                        face.TextAlign = radio.TextAlign;
                    }
                }
            }
            sfc_builder.LoadCard(CurrentCard);
            Invalidate(sfc_builder.Bounds);
        }

        List<FLERControl> controls = new List<FLERControl>();
        FLERControl hover;
        FLERControl selected;
        private void FLERForm_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (FLERControl control in controls)
            {
                if (control.Bounds.IntersectsWith(e.ClipRectangle))
                {
                    control.Paint(e);
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
            BuildNewCard();
        }

        private void FLERForm_MouseLeave(object sender, EventArgs e)
        {
            if (hover?.MouseLeave(e) == true)
            {
                Invalidate(hover.Bounds);
            }
            hover = null;
        }

        void BeginUpdateBuilder(object sender, EventArgs e)
        {
            tim_builder.Start();
        }

        private void tim_builder_Tick(object sender, EventArgs e)
        {
            UpdateBuilder(null, null);
            tim_builder.Stop();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (txt_filename.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must supply a filename!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CurrentCard.Filename = $"{txt_filename.Text}.fler";
                if (!File.Exists(Path.Combine(CARD_DIR, CurrentCard.Filename)) || MessageBox.Show("A card with this filename already exists! Do you want to overwrite it?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    CurrentCard.Save();
                }
            }
        }

        private void ValidateFilename(object sender, EventArgs e)
        {
            int a = txt_filename.SelectionStart - txt_filename.Text.Length;
            txt_filename.Text = Regex.Replace(txt_filename.Text, "[^a-zA-Z0-9-_]", "");
            txt_filename.SelectionStart = a + txt_filename.Text.Length;
        }

        private void ValidateFilenameKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '_' && !char.IsControl(e.KeyChar);
        }

        #endregion

        ///TEST CODE
    }
}