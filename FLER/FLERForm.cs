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
        /// A pseudorandom number generator
        /// </summary>
        private readonly Random _rand = new Random();

        /// <summary>
        /// A dialog with which to choose a color
        /// </summary>
        private readonly ColorDialog _colorDialog = new ColorDialog() { SolidColorOnly = true, FullOpen = true };

        /// <summary>
        /// A dialog with which to choose an image file
        /// </summary>
        private readonly OpenFileDialog _fileDialog = new OpenFileDialog() { Filter = $"Image files ({IMG_EXT.ToLower().Replace(";", ", ")})|{IMG_EXT}" };

        /// <summary>
        /// A dialog with which to choose a font
        /// </summary>
        private readonly FontDialog _fontDialog = new FontDialog() { AllowScriptChange = false };

        #region Fake Constants

        /// <summary>
        /// The flashcard directory location
        /// </summary>
        public static readonly string CARD_DIR = Path.Combine(Application.UserAppDataPath, "CARD");

        /// <summary>
        /// The image directory location
        /// </summary>
        public static readonly string IMG_DIR = Path.Combine(CARD_DIR, "IMG");

        /// <summary>
        /// A string representing the list of valid image file extensions
        /// </summary>
        public static readonly string IMG_EXT = string.Join(";", ImageCodecInfo.GetImageEncoders().Select(x => x.FilenameExtension));

        #endregion

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

        /// <summary>
        /// Whether the program is in reviewing mode
        /// </summary>
        private bool Reviewing { get; set; } = false;

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
        /// Identifies and selects the next flashcard to be reviewed
        /// </summary>
        private void NextCard()
        {
            ///note: not final implementation
            ///TEST CODE: CHANGE TimeSpan.FromSeconds TO TimeSpan.FromDays ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////         
            //chooses a random flashcard in the list whose last-reviewed time is at least 2^level days ago
            CurrentCard = Cards.Values.OrderBy(x => _rand.Next()).FirstOrDefault(x => DateTime.UtcNow - x.Date >= TimeSpan.FromSeconds(Math.Pow(2, x.Level)));

            //load the sprites of the selected flashcard
            if (CurrentCard != null)
            {
                if (dfc_reviewer.LoadCard(CurrentCard))
                {
                    Invalidate(dfc_reviewer.Bounds);
                }
            }
            else
            {
                //end reviewing mode if there are no more cards
                GoBack(null, null);
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

        #region Dialogs

        /// <summary>
        /// [Event] Opens a dialog to choose a color
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An object that contains no event data</param>
        private void PickColor(object sender, EventArgs e)
        {
            //sets the dialog color to match the panel that was clicked
            _colorDialog.Color = ((Control)sender).BackColor;

            //if the user clicked ok, set the control's color
            if (_colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Control)sender).BackColor = _colorDialog.Color;

                //updates the flashcard
                BeginUpdateBuilder(null, null);
            }
        }

        /// <summary>
        /// [Event] Opens a dialog to choose an image file
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

        /// <summary>
        /// [Event] Opens a dialog to choose a font
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An object that contains no event data</param>
        private void PickFont(object sender, EventArgs e)
        {
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
            sfc_builder.OnClick += UpdateBuilderControls;
            dfc_reviewer.OnClick += ShowReviewControls;
        }
        /// TEST CODE


        readonly StaticFlashcardControl sfc_builder = new StaticFlashcardControl() { Bounds = new Rectangle(200, 12, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT) };
        readonly DynamicFlashcardControl dfc_reviewer = new DynamicFlashcardControl() { Bounds = Rectangle.Round(new RectangleF(200 - (DynamicFlashcardControl.IMGWIDTH - StaticFlashcardControl.WIDTH) / 2, 12, DynamicFlashcardControl.IMGWIDTH, DynamicFlashcardControl.IMGHEIGHT)) };

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dfc_reviewer.Tick())
            {
                Invalidate(dfc_reviewer.Bounds);
            }
        }

        /// <summary>
        /// Begins building a new card
        /// </summary>
        private void BuildNewCard(object sender, EventArgs e)
        {
            if (!Building)
            {
                FLERControls.Add(sfc_builder);
            }
            Building = true;
            btn_build.Visible = btn_draw.Visible = false;
            btn_back.Visible = pnl_builder.Visible = true;
            CurrentCard = Flashcard.Default;
            if (sfc_builder.LoadCard(CurrentCard))
            {
                Invalidate(sfc_builder.Bounds);
            }
            UpdateBuilderControls(null, null);
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

        readonly List<FLERControl> FLERControls = new List<FLERControl>();
        FLERControl hover;
        FLERControl selected;
        private void FLERForm_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (FLERControl control in FLERControls)
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
                    foreach (FLERControl control in FLERControls)
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
                    CurrentCard.PurgeImageCache();
                    Cards[CurrentCard.Filename] = CurrentCard;
                    GoBack(null, null);
                }
            }
        }

        private void ValidateFilename(object sender, EventArgs e)
        {
            int a = txt_filename.SelectionStart;
            txt_filename.Text = Regex.Replace(txt_filename.Text, "\\s", "-");
            txt_filename.Text = Regex.Replace(txt_filename.Text, "[^a-zA-Z0-9-_]", "_");
            txt_filename.SelectionStart = a;
        }

        private void ValidateFilenameKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '_' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar);
        }

        /// <summary>
        /// Draws a flashcard from the pool and reviews it
        /// </summary>
        private void DrawCard(object sender, EventArgs e)
        {
            ///note: not final implementation
            if (Reviewing)
            {
                UpdateCard(sender as Button == btn_success);
            }
            else
            {
                FLERControls.Add(dfc_reviewer);
            }
            Reviewing = true;
            btn_build.Visible = btn_draw.Visible = btn_fail.Visible = btn_success.Visible = false;
            btn_back.Visible = true;
            NextCard();
        }


        private void ShowReviewControls(object sender, EventArgs e)
        {
            btn_fail.Visible = btn_success.Visible = true;
        }

        private void GoBack(object sender, EventArgs e)
        {
            btn_build.Visible = btn_draw.Visible = true;
            btn_back.Visible = btn_fail.Visible = btn_success.Visible = pnl_builder.Visible = false;
            if (Building)
            {
                FLERControls.Remove(sfc_builder);
                Building = false;
            }
            if (Reviewing)
            {
                FLERControls.Remove(dfc_reviewer);
                Reviewing = false;
            }
            Invalidate();
        }

        #endregion

        ///TEST CODE
    }
}