using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace clear
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (ConfigurationManager.AppSettings["hidden"] == "False" | ConfigurationManager.AppSettings["hidden"] == "false")
            {
                Application.Run(new Form1());
            }
            else { Clear(); }
           
            void Clear()
            {
                var DIR = new DirectoryInfo(ConfigurationManager.AppSettings["pathToDirectoryFiles"]);
                var listFolders = ConfigurationManager.AppSettings ["listFolders"].Split(',');

                CreateFolders(DIR.FullName,listFolders);

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
                            catch (System.IO.IOException){}
                        }
                    }
                }
            }
            
            void CreateFolders(string rootDirectory, string [] listFolders)
            {
                foreach (var folder in listFolders)
                {
                    var folderPath = Path.Combine(rootDirectory, folder);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(Path.Combine(folderPath));
                    }

                }
            }
        }
    }
}
