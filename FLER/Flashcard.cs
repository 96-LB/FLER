using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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

        #region IMPORT/EXPORT //mostly copied from C-4 as of now

        private static byte[] Checksum(Stream stream, long start = 0, long end = long.MaxValue)
        {
            end = Math.Min(stream.Length, end);
            byte[] bytes = new byte[12] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            byte[] nbytes = new byte[12];
            stream.Position = start;

            int n = stream.Read(nbytes, 0, Math.Min(12, (int)Math.Min(int.MaxValue, end - stream.Position)));
            while (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    bytes[i] ^= nbytes[i];
                }
                byte temp = (byte)((bytes[11] >> 7) & 1);
                for (int i = 11; i > 0; i--)
                {
                    bytes[i] <<= 1;
                    bytes[i] |= (byte)((bytes[i - 1] >> 7) & 1);
                }
                bytes[0] <<= 1;
                bytes[0] |= temp;
                n = stream.Read(nbytes, 0, Math.Min(12, (int)Math.Min(int.MaxValue, end - stream.Position)));
            }

            return bytes;
        }

        private static bool VerifyChecksum(Stream stream)
        {
            try
            {
                stream.Position = stream.Length - 12;
                byte[] c = Checksum(stream, 0, stream.Length - 12);
                byte[] b = new byte[12];
                stream.Read(b, 0, 12);
                if (c.Length == b.Length && c.SequenceEqual(b))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to load a flashcard from the given path
        /// </summary>
        /// <param name="filename">The path from which to load a card</param>
        /// <param name="card">The loaded card</param>
        /// <returns>Whether the operation was successful</returns>
        public static bool TryLoad(string filename, out Flashcard card)
        {
            card = null;
            if (!File.Exists(filename))
            {
                return false;
            }
            using (FileStream stream = File.OpenRead(filename))
            {
                if (!VerifyChecksum(stream))
                {
                    return false;
                }
                using (MemoryStream copy = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(copy, (int)stream.Length - 12);
                    copy.Position = 0;
                    using (GZipStream deflate = new GZipStream(copy, CompressionMode.Decompress))
                    {
                        using (StreamReader sr = new StreamReader(deflate, Encoding.UTF8))
                        {
                            string json;
                            try
                            {
                                json = sr.ReadToEnd();
                                card = JsonConvert.DeserializeObject<Flashcard>(json);
                            }
                            catch (InvalidDataException e)
                            {
                                return false;
                            }
                            catch (JsonReaderException e)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves this flashcard to the specified file path
        /// </summary>
        /// <param name="filename">A path pointing to where the card should be saved</param>
        public void Save(string filename)
        {
            string json = JsonConvert.SerializeObject(this); //serializes this card in json format

            //create a new file...
            using(FileStream file = File.Open(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                ///with gzip compression...
                using(GZipStream deflate = new GZipStream(file, CompressionMode.Compress))
                {
                    //and encoded text...
                    using(StreamWriter sw = new StreamWriter(deflate, Encoding.UTF8))
                    {
                        //write the json as a string
                        sw.Write(json);
                    }
                }
            }
            //reopen the file to update its length property
            using (FileStream file = File.Open(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                //append the checksum
                file.Write(Checksum(file), 0, 12);
            }
        }

        #endregion
    }
}
