using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace StashEdit.Class
{
    
    class DbHandler
    {
        XmlSettings xm = new XmlSettings();
        public DataTable RunQuery(String qry)
        {
            DataTable dt = new DataTable();
            xm = xm.GetXmlSettings();
            if (xm.StashDBLoc == "set")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Set stash database path", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                SQLiteConnection con = new SQLiteConnection("Data Source=" + xm.StashDBLoc);
                dt.Columns.Add("imgcheck");
                SQLiteCommand com = new SQLiteCommand();
                com.CommandText = qry;
                com.Connection = con;
                con.Open();
                var adpt = new SQLiteDataAdapter(com);
                adpt.Fill(dt);
                con.Close();
            }

            return dt;
        }
        public DataTable RunCommand(SQLiteCommand cmd)
        {
            xm = xm.GetXmlSettings();
            SQLiteConnection con = new SQLiteConnection("Data Source=" + xm.StashDBLoc);
            DataTable dt = new DataTable();
            dt.Columns.Add("imgcheck");
            cmd.Connection = con;
            con.Open();
            var adpt = new SQLiteDataAdapter(cmd);
            adpt.Fill(dt);
            con.Close();
            return dt;
        }
        public void UpdateStashDB(UpdateDbContents NewInfo)
        {
            xm = xm.GetXmlSettings();
            SQLiteConnection con = new SQLiteConnection("Data Source=" + xm.StashDBLoc);
            con.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = con;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("UPDATE scenes");
            sql.AppendLine("SET path = :path, title = :title,");
            sql.AppendLine("    details = :detail, url = :url,");
            sql.AppendLine("    date = :dt");
            sql.AppendLine(" WHERE ID=" + NewInfo.id);
            string NewUrl = "";

            if (NewInfo.scninfo.data[0].url.ToLower().Contains("brazzers"))
            {
                NewUrl = NewInfo.scninfo.data[0].url.Replace("site-ma", "www").Replace("scene", "video");
            }
            else
            {
                if (!NewInfo.scninfo.data[0].url.Contains("www"))
                {
                    //add it
                    NewUrl = NewInfo.scninfo.data[0].url.Insert(8, "www.");
                }
            }

            command.CommandText = sql.ToString();
            command.Parameters.Add("path", System.Data.DbType.String).Value = NewInfo.NewPath;
            command.Parameters.Add("title", System.Data.DbType.String).Value = NewInfo.title;
            command.Parameters.Add("detail", System.Data.DbType.String).Value = NewInfo.scninfo.data[0].description;
            command.Parameters.Add("dt", System.Data.DbType.String).Value = NewInfo.scninfo.data[0].created;
            command.Parameters.Add("url", System.Data.DbType.String).Value = NewUrl;

            command.ExecuteNonQuery();
            command.Parameters.Clear();
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(NewInfo.scninfo.data[0].background.full);
            command.CommandText = "INSERT OR REPLACE INTO SCENES_COVER (scene_id, cover) VALUES (:id, :pic)";
            command.Parameters.Add("id", System.Data.DbType.Int32).Value = NewInfo.id;
            command.Parameters.Add("pic", System.Data.DbType.Binary).Value = imageBytes;
            command.ExecuteNonQuery();
            command.CommandText = "INSERT OR REPLACE INTO SCENES_TAGS (scene_id, tag_id) VALUES (:id, :tid)";
            command.Parameters.Add("id", System.Data.DbType.Int32).Value = NewInfo.id;
            command.Parameters.Add("tid", System.Data.DbType.Int32).Value = 880;
            command.ExecuteNonQuery();
            con.Close();
        }
    }
    public class UpdateDbContents
    {
        public string title { get; set; }
        public string id { get; set; }
        public SceneInfo scninfo { get; set; }
        public FileInfo OldFile { get; set; }
        public string NewPath { get; set; }
    }
}
