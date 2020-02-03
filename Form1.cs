using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json;

namespace clear
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
             InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var DIR = new DirectoryInfo(ConfigurationManager.AppSettings["pathToDirectoryFiles"]);
            var dicPathExt = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(ConfigurationManager.AppSettings["fileWithPathExt"]));

            foreach (FileInfo file in DIR.GetFiles())
            {
                foreach (KeyValuePair<string, string> kvp in dicPathExt)
                {
                    var dicPath = new Dictionary<string, string>();
                    string[] extention = kvp.Key.Split(',');
                    foreach (var ext in extention)
                        dicPath[ext] = kvp.Value;
                    if (dicPath.TryGetValue(file.Extension, out string outputPath))
                    {
                        if (outputPath == "Delete")
                        {
                            File.Delete(file.FullName);
                            break;
                        }
                        var filePath = Path.Combine(outputPath, file.Name);
                        try
                        {
                            if (File.Exists(filePath)) File.Delete(filePath);
                            File.Move(file.FullName, filePath);
                        }
                        catch (System.IO.IOException ex)
                        {
                            label1.Text = filePath + ex.Message;
                        }
                    }
                }
        }
        }
    }
}
