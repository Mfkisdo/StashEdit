using StashEdit.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StashEdit.Windows
{
    /// <summary>
    /// Interaction logic for ImagesToTags.xaml
    /// </summary>
    public partial class ImagesToTags : Window
    {
        SQLiteConnection con = new SQLiteConnection(@"Data Source=C:\Users\Branden\.stash\stash-go.sqlite;Version=3;");
        List<FileInfo> filist = new List<FileInfo>();
        BackgroundWorker bgw = new BackgroundWorker();
        XmlSettings xm = new XmlSettings();
        public ImagesToTags()
        {
            InitializeComponent();
            xm = xm.GetXmlSettings();
            txtFolderPath.Text = xm.ImagesToTagsLocation;
        }

        private void txtFolderPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetFiles();
        }

        private void GetFiles()
        {
            //get images from path
            if (Directory.Exists(txtFolderPath.Text))
            {
                DirectoryInfo dirfiles = new DirectoryInfo(txtFolderPath.Text);
                foreach (var fi in dirfiles.GetFiles("*"))
                {
                    filist.Add(fi);
                }
                lbImages.DisplayMemberPath = "Name";
                lbImages.SelectedValuePath = "FullName";
                lbImages.ItemsSource = filist;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            GetFiles();
            bgw.RunWorkerAsync();
        }

        private DataTable RunQuery(String qry)
        {
            DataTable dt = new DataTable();
            SQLiteCommand com = new SQLiteCommand();
            com.CommandText = qry;
            com.Connection = con;
            con.Open();
            var adpt = new SQLiteDataAdapter(com);
            adpt.Fill(dt);
            con.Close();
            return dt;
        }

        private void RunNonQuery(String qry, object id, byte[] img)
        {
            SQLiteCommand com = new SQLiteCommand();
            com.CommandText = qry;
            com.Parameters.Add("id", DbType.Int32).Value = id;
            com.Parameters.Add("img", DbType.Binary).Value = img;
            com.Connection = con;
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            int cnt = 0;
            foreach (var fi in filist)
            {
                byte[] imageBytes = File.ReadAllBytes(fi.FullName);
                var filename = System.IO.Path.GetFileNameWithoutExtension(fi.FullName);
                DataTable dtTagList = RunQuery("SELECT id, name FROM TAGS WHERE name ='" + filename + "'");
                if (dtTagList.Rows.Count == 1)
                {
                    int percents = ( cnt * 100) / filist.Count;
                    bgw.ReportProgress(percents, cnt);
                    //Found tag
                    DataRow dr = dtTagList.Rows[0];
                    string qry = "INSERT OR REPLACE INTO tags_image(tag_id, image) VALUES (:id, :img)";
                    RunNonQuery(qry, dr["id"], imageBytes);
                    cnt++;
                }
            }
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProg.Value = e.ProgressPercentage;
            lbProg.Content = String.Format("Progress: {0} %", e.ProgressPercentage);
            lbContent.Content = String.Format("Total items updated: {0}", e.UserState);
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //do the code when bgv completes its work
            lbProg.Content = "Finished Updating Images";
            pbProg.Value = 0;
        }
    }
}
