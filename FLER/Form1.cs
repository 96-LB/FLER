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
    public partial class Form1 : Form
    {
        public Form1()
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

            /// TEST CODE
            Flashcard f = new Flashcard() { hidden = new Flashcard.Face() { backColor = Color.FromArgb(96, 96, 96) } };
            MessageBox.Show(JsonConvert.SerializeObject(f));
            f.Save("test.fler");
            if(Flashcard.TryLoad("test.fler", out Flashcard g))
            {
                MessageBox.Show(JsonConvert.SerializeObject(g));
            }
            else
            {
                MessageBox.Show("x_x");
            }
            /// TEST CODE
        }
    }
}
