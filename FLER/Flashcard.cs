using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

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
            /// The color used to fill the background
            /// </summary>
            public Color BackColor { get; set; }

            /// <summary>
            /// The color used to fill the outline
            /// </summary>
            public Color LineColor { get; set; }

            /// <summary>
            /// The bounds of the text box
            /// </summary>
            public Rectangle TextBox { get; set; }

            /// <summary>
            /// The content of the text box
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// The alignment of the text within the text box
            /// </summary>
            public ContentAlignment TextAlign { get; set; }

            /// <summary>
            /// The font used to render the text box
            /// </summary>
            public Font Font { get; set; }

            /// <summary>
            /// The color of the text in the textbox
            /// </summary>
            public Color TextColor { get; set; }

            /// <summary>
            /// The bounds of the image box
            /// </summary>
            public Rectangle ImageBox { get; set; }

            /// <summary>
            /// A file path pointing to content of the image box
            /// </summary>
            public string ImagePath { get; set; }

            /// <summary>
            /// Whether the image box should render overtop of the text box
            /// </summary>
            public bool ImageTop { get; set; }
        }

        #endregion

        #region Properties

        #region Public Static

        /// <summary>
        /// A default flashcard
        /// </summary>
        public static Flashcard Default => new Flashcard()
        {
            Visible = new Face()
            {
                BackColor = Color.LightGray,
                LineColor = Color.Gray,
                Text = "Visible",
                TextBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT),
                TextColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = StaticFlashcardControl.FONT_DEF,
                ImageBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT)
            },
            Hidden = new Face()
            {
                BackColor = Color.LightGray,
                LineColor = Color.Gray,
                Text = "Hidden",
                TextBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT),
                TextColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = StaticFlashcardControl.FONT_DEF,
                ImageBox = new Rectangle(0, 0, StaticFlashcardControl.WIDTH, StaticFlashcardControl.HEIGHT)
            }
        };

    #endregion

    #region Public Instance

    /// <summary>
    /// The file from which the flashcard was loaded
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// The level at which the flashcard currently resides
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// The date at which the flashcard was last reviewed
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The tags associated with the flashcard
    /// </summary>
    public string[] Tags { get; set; }

    /// <summary>
    /// The visible face of the flashcard, displaying the prompt
    /// </summary>
    public Face Visible { get; set; }

    /// <summary>
    /// The hidden face of the flashcard, displaying the answer
    /// </summary>
    public Face Hidden { get; set; }

    #endregion

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
    /// <param name="filepath">The path from which to load a flashcard</param>
    /// <param name="card">The loaded flashcard</param>
    /// <returns>Whether the operation was successful</returns>
    public static bool TryLoad(string filename, out Flashcard card)
    {
        //sets the output in case of early exit
        card = null;

        //navigates to the card directory
        string filepath = Path.Combine(FLERForm.CARD_DIR, filename);

        if (!File.Exists(filepath))
        {
            return false; //if the file doesn't exist, the load fails
        }

        using FileStream stream = File.OpenRead(filepath); //the file to be read

        if (!VerifyChecksum(stream))
        {
            return false; //if it doesn't have a valid checksum, the load fails
        }

        using MemoryStream copy = new MemoryStream(); //a copy of the data without the 96-checksum

        //cuts off the last 96 bits
        stream.Position = 0;
        stream.CopyTo(copy, (int)stream.Length - 12);
        copy.Position = 0;

        using GZipStream deflate = new GZipStream(copy, CompressionMode.Decompress); //a gzip decompression stream
        using StreamReader sr = new StreamReader(deflate, Encoding.UTF8); //a string reader to get the json data

        try
        {
            //parse the json data and set the output
            card = JsonConvert.DeserializeObject<Flashcard>(sr.ReadToEnd());

            //if either face doesn't exist, the load fails
            if (card.Visible == null || card.Hidden == null)
            {
                card = null;
            }

            //sets the filename from which the flashcard was loaded
            card.Filename = filename;

            return card != null; //returns whether the load was successful
        }
        catch (Exception)
        {
            return false; //if any errors occurred, the load fails
        }
    }

    #endregion

    #region Public Instance

    /// <summary>
    /// Saves the flashcard to the filename from which it was loaded
    /// </summary>
    public void Save()
    {
        Save(Filename);
    }

    /// <summary>
    /// Saves the flashcard to the specified file path
    /// </summary>
    /// <param name="filename">A path pointing to where the flashcard should be saved</param>
    public void Save(string filename)
    {
        string json = JsonConvert.SerializeObject(this); //serializes the flashcard in json format
        string path = Path.Combine(FLERForm.CARD_DIR, filename); //navigates to the the card directory

        //create a new file...
        using (FileStream file = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
        {
            using GZipStream deflate = new GZipStream(file, CompressionMode.Compress); //a gzip compression stream
            using StreamWriter sw = new StreamWriter(deflate, Encoding.UTF8); //a text reader

            //write the json as a string
            sw.Write(json);
        }

        //reopen the file to update its length property
        using (FileStream file = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
        {
            //append the checksum
            file.Write(Checksum(file), 0, 12);
        }

        //deletes the image directory if it exists
        path = Path.Combine(FLERForm.IMG_DIR, filename);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    #endregion

    #endregion

}
}
