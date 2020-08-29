using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public string GenNewName(SceneInfo si, string ext)
        {
            String NewName = "";

            if (si.data.Count != 0)
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

        private string NameFormat(SceneInfo si, string pattern)
        {
            string val = "";
            if(pattern.ToLower() == "publisher")
            {
                val = si.data[0].site.name;
            }
            if(pattern.ToLower() == "actor")
            {
                List<string> ActorName = new List<string>();
                foreach (Performer acts in si.data[0].performers)
                {
                    if (acts.parent != null && acts.parent.extras.gender != null)
                    {
                        if (acts.parent.extras.gender.ToString().ToLower() == "female")
                        {
                            ActorName.Add(acts.parent.name);
                        }
                    }

                }

                val = string.Join(" - ", ActorName);
            }
            if (pattern.ToLower() == "title")
            {
                val = si.data[0].title.Replace("'", "").Replace("-", "").Trim();
            }


            return val;
        }
    }
}
