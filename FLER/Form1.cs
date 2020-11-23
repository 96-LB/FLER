using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private readonly string CARD_DIR = Path.Combine(Application.CommonAppDataPath, "CARD");

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
            //this code loads assemblies that are embedded into the project
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = GetType().Assembly.GetManifestResourceNames().First(element => element.EndsWith(resourceName));

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InitializeComponent();

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

            /// TEST CODE
            foreach (var i in Cards)
            {
                MessageBox.Show(JsonConvert.SerializeObject(i));
            }

            var f = new Flashcard() { tags = new string[] { "first" }, visible = new Flashcard.Face() { text = "first card" } };
            f.Save(Path.Combine(CARD_DIR, "first.fler"));
            new Flashcard().Save(Path.Combine(CARD_DIR, "empty.fler"));

            /// TEST CODE
        }


        #endregion

        #region Methods

        /// <summary>
        /// Draws a card from the pool and reviews it
        /// </summary>
        private void DrawCard()
        {
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
            ///TEST CODE: CHANGE TimeSpan.FromSeconds TO TimeSpan.FromDays ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //returns the first card in the list whose last-reviewed time is at least 2^level days ago
            KeyValuePair<string, Flashcard> current = Cards.FirstOrDefault(x => DateTime.UtcNow - x.Value.date >= TimeSpan.FromSeconds(Math.Pow(2, x.Value.level)));
            
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
            //sets the card's last review date and adjusts the card's level accordingly 
            CurrentCard.date = DateTime.UtcNow;
            CurrentCard.level = levelUp ? CurrentCard.level + 1 : 0;
            CurrentCard.Save(CurrentDir);
        }

        #endregion

        #region Events

        private void button1_Click(object sender, EventArgs e)
        {
            DrawCard();
        }

        #endregion

    }
}