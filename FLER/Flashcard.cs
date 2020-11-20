using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLER
{
    /// <summary>
    /// Represents a two-sided flashcard
    /// </summary>
    class Flashcard
    {

        #region Inner class

        /// <summary>
        /// Represents one face of a flashcard
        /// </summary>
        public class Face
        {

            /// <summary>
            /// A list of file paths pointing to each of the pre-generated sprites
            /// </summary>
            public string[] sprites;

            /// <summary>
            /// The color used to fill the background
            /// </summary>
            public Color backColor;

            /// <summary>
            /// The bounds of the text box
            /// </summary>
            public Rectangle textBox;
            
            /// <summary>
            /// The content of the text box
            /// </summary>
            public string text;
            
            /// <summary>
            /// The font used to render the text box
            /// </summary>
            public Font font;
            
            /// <summary>
            /// The color of the text in the textbox
            /// </summary>
            public Color foreColor;
            
            /// <summary>
            /// The bounds of the image box
            /// </summary>
            public Rectangle imageBox;

            /// <summary>
            /// A file path pointing to content of the image box
            /// </summary>
            public string imagePath;

        }

        #endregion

        #region Fields

        /// <summary>
        /// The level at which this card currently resides
        /// </summary>
        public int level;

        /// <summary>
        /// The date at which this card was last reviewed
        /// </summary>
        public DateTime date;

        /// <summary>
        /// The tags associated with this card
        /// </summary>
        public string[] tags;

        /// <summary>
        /// The visible face of this card, displaying the prompt
        /// </summary>
        public Face visible;

        /// <summary>
        /// The hidden face of this card, displaying the answer
        /// </summary>
        public Face hidden;

        #endregion

    }
}
