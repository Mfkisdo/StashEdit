using System;
using System.IO;
using System.Xml.Serialization;

namespace StashEdit.Class
{
    //Class for handle xml settings file
    public class XmlSettings
    {
        [XmlElement(Namespace = "StashDBLoc")]
        public string StashDBLoc;

        [XmlElement(Namespace = "MetaApiLimit")]
        public string MetaApiLimit;

        [XmlElement(Namespace = "NewNameStyle")]
        public string NewNameStyle;

        [XmlElement(Namespace = "ImagesToTagsLocation")]
        public string ImagesToTagsLocation;

        [XmlElement(Namespace = "SortFolderLocation")]
        public string SortFolderLocation;

        [XmlElement(Namespace = "DestinationFolders")]
        public string DestinationFolders;

        [XmlElement(Namespace = "stashPornDBScrapper")]
        public string stashPornDBScrapper;



        public void WriteToXMLDoc(XmlSettings xSettings)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(XmlSettings));

            var path = AppDomain.CurrentDomain.BaseDirectory + @"\StarkSettings.xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, xSettings);
            file.Close();
        }

        public XmlSettings GetXmlSettings()
        {
            XmlSettings xf = new XmlSettings();
            //File Does Not Exist create it and fill with default values using a new xmlwriter
            XmlSerializer serializer = new XmlSerializer(typeof(XmlSettings));
            using (Stream reader = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"\StarkSettings.xml", FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                xf = (XmlSettings)serializer.Deserialize(reader);
            }
            return xf;
        }

        public void CheckForSettingsXmlFile()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\StarkSettings.xml"))
            {
                XmlSettings xm = defaultXmlSettings();
                
                //File Does Not Exist create it and fill with default values using a new xmlwriter
                System.Xml.Serialization.XmlSerializer writer =
                        new System.Xml.Serialization.XmlSerializer(typeof(XmlSettings));

                var path = AppDomain.CurrentDomain.BaseDirectory + @"\StarkSettings.xml";
                System.IO.FileStream file = System.IO.File.Create(path);
                System.Xml.XmlWriter wrt = System.Xml.XmlWriter.Create(file);
                wrt.WriteStartDocument();
                wrt.WriteComment("StarkSettings Version 1.0.0.0");
                wrt.WriteComment("For default settings delete this file, program will regen..");
                wrt.WriteComment("");
                wrt.WriteComment("");
                wrt.WriteComment("NewNameStyle only supports the three shown, maybe more support later");
                writer.Serialize(file, xm);
                file.Close();
            }
        }

        private XmlSettings defaultXmlSettings()
        {
            XmlSettings xm = new XmlSettings();
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            xm.StashDBLoc = @"set";
            xm.MetaApiLimit = "50";
            xm.NewNameStyle = "Publisher;Actor;Title";
            xm.ImagesToTagsLocation = @"set";
            xm.SortFolderLocation = @"set";
            xm.DestinationFolders = @"set;set";
            xm.stashPornDBScrapper = @"set";
            return xm;
        }
    }
}
