using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace StashEdit.Class
{
    class FileChanges
    {
        /// <summary>
        /// Pass a SceneInfo and return a new name
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>

        private XmlSettings xf = new XmlSettings();
        public string GenNewName(GetSceneByID si, string ext)
        {
            String NewName = "";

            if (si.data != null)
            {
                if (xf != null)
                {
                    xf = xf.GetXmlSettings();
                    var nameStyle = xf.NewNameStyle.Split(";");
                    //Get first name style

                    List<string> CompileName = new List<string>();

                    foreach (string stylepattern in nameStyle)
                    {
                        CompileName.Add(NameFormat(si, stylepattern));
                    }

                    NewName = string.Join(" - ", CompileName) + ext;
                }
                else
                {
                    MessageBox.Show("Nothing found in settings file", "Error");
                }
              

            }
            else
            {
                MessageBox.Show("Nothing Found at Url");
            }

            return NewName;
        }

        private string NameFormat(GetSceneByID si, string pattern)
        {
            string val = "";
            if(pattern.ToLower() == "publisher")
            {
                val = si.data.site.name;
            }
            if(pattern.ToLower() == "actor")
            {
                List<string> ActorName = new List<string>();
                foreach (Performer acts in si.data.performers)
                {
                    if (acts.parent != null && acts.parent.extras.gender != null)
                    {
                        if (acts.parent.extras.gender.ToString().ToLower() == "female")
                        {
                            ActorName.Add(acts.parent.name);
                        }
                    }

                }
                if (ActorName.Count == 0)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("No actor found with scene data input manually", "Informational", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                val = string.Join(" - ", ActorName);
            }
            if (pattern.ToLower() == "title")
            {
                val = si.data.title.Replace("'", "").Replace("-", "").Trim();
            }


            return val;
        }
        public string CleanFileNameForSearch(FileInfo selFile)
        {
            string NewName = "";
            if (File.Exists(selFile.FullName))
            {
                NewName = Path.GetFileNameWithoutExtension(selFile.FullName);
                NewName = Regex.Replace(NewName, "[^a-zA-Z]+", " ").Replace("mp4", "");
            }
           
           

            return NewName;
        }
    }
}
