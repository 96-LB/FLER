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

        #region Methods

        #region Private Static

        /// <summary>
        /// Calculates the 96-checksum of a stream of bytes
        /// </summary>
        /// <param name="stream">The stream whose checksum should be calculated</param>
        /// <param name="start">The starting position of the section to be considered</param>
        /// <param name="end">The ending position of the section to be considered</param>
        /// <returns>The 96-checksum of the given byte stream</returns>
        private static byte[] Checksum(Stream stream, long start = 0, long end = long.MaxValue)
        {
            //clips the start and end to the stream size
            start = Math.Max(start, 0);
            end = Math.Max(start, Math.Min(end, stream.Length));
            stream.Position = start;

            byte[] checksum = new byte[12] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }; //the output
            byte[] bytes = new byte[12]; //bytes read from the stream

            int n = stream.Read(bytes, 0, Math.Min(12, (int)Math.Min(int.MaxValue, end - stream.Position))); //the number of bytes read

            //read up to 96 bits at a time until there are no more to read
            while (n > 0)
            {
                //xor the checksum with the given byte data
                for (int i = 0; i < n; i++)
                {
                    checksum[i] ^= bytes[i];
                }

                byte temp = (byte)((checksum[11] >> 7) & 1); //the last bit in the checksum

                //left-shifts the checksum by one byte
                for (int i = 11; i > 0; i--)
                {
                    checksum[i] <<= 1; //left-shifts each int by one byte
                    checksum[i] |= (byte)((checksum[i - 1] >> 7) & 1); //sets the lsb to the last bit of the previous int
                }
                checksum[0] <<= 1; //left-shifts the first int by one byte
                checksum[0] |= temp; //sets the lsb to the old msb

                //read more bytes
                n = stream.Read(bytes, 0, Math.Min(12, (int)Math.Min(int.MaxValue, end - stream.Position)));
            }

            return checksum; //return the calculated checksum
        }

        /// <summary>
        /// Checks whether a stream's byte data matches its 96-checksum
        /// </summary>
        /// <param name="stream">A stream with its 96-checksum appended</param>
        /// <returns>True if the 96-checksum was successfully validated</returns>
        private static bool VerifyChecksum(Stream stream)
        {
            try
            {
                byte[] calculated = Checksum(stream, 0, stream.Length - 12);  //the 96-checksum calculated from the byte data
                byte[] appended = new byte[12]; //the 96-checksum appended to the end of the stream
                stream.Read(appended, 0, 12);
                return appended.Length == calculated.Length && appended.SequenceEqual(calculated); //returns whether the two are equal
            }
            catch
            {
                return false; //if any error occurred, the checksum is invalid
            }
        }

        #endregion

        #region Public Static

        /// <summary>
        /// Attempts to load a flashcard from the given path
        /// </summary>
        /// <param name="filename">The path from which to load a card</param>
        /// <param name="card">The loaded card</param>
        /// <returns>Whether the operation was successful</returns>
        public static bool TryLoad(string filename, out Flashcard card)
        {
            //sets the output in case of early exit
            card = null;
            
            if (!File.Exists(filename))
            {
                return false; //if the file doesn't exist, the load fails
            }

            //otherwise, open the file
            using (FileStream stream = File.OpenRead(filename))
            {
                if (!VerifyChecksum(stream))
                {
                    return false; //if it doesn't have a valid checksum, the load fails
                }

                //otherwise, copy the data (excluding the checksum) to a memory stream
                using (MemoryStream copy = new MemoryStream())
                {
                    //cuts off the last 96 bits
                    stream.Position = 0;
                    stream.CopyTo(copy, (int)stream.Length - 12);
                    copy.Position = 0;

                    //open a gzip stream to decompress the data
                    using (GZipStream deflate = new GZipStream(copy, CompressionMode.Decompress))
                    {
                        //open a reader to get the json data
                        using (StreamReader sr = new StreamReader(deflate, Encoding.UTF8))
                        {
                            try
                            {
                                //parse the json data and set the output
                                card = JsonConvert.DeserializeObject<Flashcard>(sr.ReadToEnd());
                                return true; //if no errors, the load was successful
                            }
                            catch (Exception)
                            {
                                return false; //if any errors occurred, the load fails
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Instance

        /// <summary>
        /// Saves this flashcard to the specified file path
        /// </summary>
        /// <param name="filename">A path pointing to where the card should be saved</param>
        public void Save(string filename)
        {
            string json = JsonConvert.SerializeObject(this); //serializes this card in json format

            //create a new file...
            using (FileStream file = File.Open(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                ///with gzip compression...
                using (GZipStream deflate = new GZipStream(file, CompressionMode.Compress))
                {
                    //and encoded text...
                    using (StreamWriter sw = new StreamWriter(deflate, Encoding.UTF8))
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

        #endregion

    }
}
