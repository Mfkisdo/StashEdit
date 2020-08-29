using Newtonsoft.Json;
using StashEdit.Class;
using StashEdit.Windows;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StashEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        SceneInfo si = new SceneInfo();
        int idx = 0;
        DbHandler hand = new DbHandler();
        private XmlSettings xf = new XmlSettings();

        public MainWindow()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\StarkSettings.xml"))
            {
                xf.CheckForSettingsXmlFile();
            }
            InitializeComponent();

            xf = xf.GetXmlSettings();
        }

        private void lbFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //When a file is selected place title into textbox to search
            if (lbFileList.SelectedItem != null)
            {
                idx = lbFileList.SelectedIndex + 1;
                DataRowView d1 = lbFileList.SelectedItem as DataRowView;
                txtSearchMetaAPI.Text = Regex.Replace(d1["title"].ToString(), "[^0-9a-zA-Z]+", " ").Replace("mp4", "");
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT id, path, title, url, CAST(created_at as varchar) as dt FROM SCENES WHERE path = :pp";
                cmd.Parameters.Add("pp", DbType.String).Value = lbFileList.SelectedValue.ToString();
                //string qry = "SELECT id, path, title, url, CAST(created_at as varchar) as dt FROM SCENES WHERE path = " + lbFileList.SelectedValue.ToString() + ";
                DataTable dt = hand.RunCommand(cmd);
                if (dt.Rows.Count == 1)
                {
                    txtDBFullpath.Text = dt.Rows[0]["path"].ToString();
                    txtDBid.Text = dt.Rows[0]["id"].ToString();
                    txtDBTitleName.Text = dt.Rows[0]["title"].ToString();
                    txtDBUrl.Text = dt.Rows[0]["url"].ToString();

                }
            }
        }
        private void cbSearchDB_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Searching for file and fill the list box
            string qry = "";
            if (chkAll.IsChecked == true)
            {
                qry = "SELECT title, path FROM SCENES WHERE PATH LIKE ('%" + cbSearchDB.Text + "%')";
                DbHandler hand = new DbHandler();
                lbFileList.ItemsSource = hand.RunQuery(qry).DefaultView;
            }
        }
        private void lbSearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get url and send to clipboard
            if (lbSearchList.SelectedValue != null)
            {
                try
                {
                    Clipboard.SetText("https://metadataapi.net/scene/" + lbSearchList.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                var x = "https://metadataapi.net/api/scenes?parse=" + lbSearchList.SelectedValue.ToString();
                var scni = GetSceneInfo(x);
                txtDBTitle.Text = scni.data[0].title;
                if (scni.data.Count == 1)
                {

                    if (lbFileList.SelectedValue != null && si.data != null)
                    {
                        var genName = new FileChanges();
                        string nn = genName.GenNewName(scni, Path.GetExtension(lbFileList.SelectedValue.ToString()));
                        if (!File.Exists(nn))
                        {
                            txtNewName.Text = nn.Replace(": ", "").Replace(@"\", "").Replace(@"/", "");
                        }
                        else
                        {
                            Xceed.Wpf.Toolkit.MessageBox.Show("File already exists", "Informational", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("No File Selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }
            }
        }
        private async Task SearchForMetaDataAsync()
        {
            string buildqry = "https://metadataapi.net/api/scenes?parse=" + txtSearchMetaAPI.Text + "&" + xf.MetaApiLimit;
            SceneInfo si = await Task.Run(() => GetSceneInfo(buildqry));
            if (si.data != null && si.data.Count != 0)
            {
                //Return one match start writing some data includes updating stash db and allows a name change.
                FileInfo fi = new FileInfo(lbFileList.SelectedValue.ToString());
                DataTable dt = new DataTable();
                dt.Columns.Add("id");
                dt.Columns.Add("title");
                dt.Columns.Add("url");

                foreach (var scn in si.data)
                {
                    DataRow dr = dt.NewRow();
                    dr["id"] = scn.id;
                    dr["title"] = scn.title;
                    dr["url"] = scn.background.full;
                    dt.Rows.Add(dr);

                }

                lbSearchList.ItemsSource = dt.DefaultView;
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("unable to find scene, edit the search params if possible", "Informational", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            biBusy.IsBusy = false;
        }




        #region Buttons

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Get old name first
            if (lbFileList.SelectedValue != null)
            {
                FileInfo OldFileInfo = new FileInfo(lbFileList.SelectedValue.ToString());
                string NewName = txtNewName.Text.Replace("?", "");

                if (!File.Exists(System.IO.Path.Combine(OldFileInfo.Directory.FullName, NewName)))
                {
                    //File Doesn't already exists update it
                    File.Move(OldFileInfo.FullName, System.IO.Path.Combine(OldFileInfo.Directory.FullName, NewName));
                    //Check if we are only fixing the sort folder typically this isnt in stash yet.
                    if (!chkSortFldr.IsChecked == true)
                    {
                        UpdateDbContents cont = new UpdateDbContents();
                        cont.id = txtDBid.Text;
                        cont.NewPath = System.IO.Path.Combine(OldFileInfo.Directory.FullName, NewName);
                        cont.OldFile = OldFileInfo;
                        cont.scninfo = si;
                        cont.title = txtDBTitle.Text;
                        hand.UpdateStashDB(cont);
                        RefreshListFiles();
                        ClearTextBoxesOnUpdate();
                        if (chkAll.IsChecked == true)
                        {
                            var cache = cbSearchDB.Text;
                            cbSearchDB.Text = "";
                            cbSearchDB.Text = cache;
                        }
                        Xceed.Wpf.Toolkit.MessageBox.Show("File Updated", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    var d = Xceed.Wpf.Toolkit.MessageBox.Show("File already exists click of to open directory", "Informational", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    if (d == MessageBoxResult.OK)
                    {
                        Process.Start("explorer.exe", "/select, " + OldFileInfo.FullName);
                    }
                }
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("No File Selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void btnUpdateDB_Click(object sender, RoutedEventArgs e)
        {
            if (lbFileList.SelectedValue != null)
            {
                FileInfo stashFi = new FileInfo(lbFileList.SelectedValue.ToString());
                UpdateDbContents cont = new UpdateDbContents();
                cont.id = txtDBid.Text;
                cont.NewPath = stashFi.FullName;
                cont.OldFile = stashFi;
                cont.scninfo = si;
                cont.title = txtDBTitle.Text;
                DbHandler hand = new DbHandler();
                hand.UpdateStashDB(cont);
                ClearTextBoxesOnUpdate();
            }

        }
        private void btnSearchMeta_Click(object sender, RoutedEventArgs e)
        {
            biBusy.IsBusy = true;
            SearchForMetaDataAsync();
        }
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder helpmsg = new StringBuilder();
            helpmsg.AppendLine("1. This is in development and will need to explore for errors");
            helpmsg.AppendLine("");
            helpmsg.AppendLine("");
            helpmsg.AppendLine("");
            helpmsg.AppendLine("Search for scene in the stash database");
            helpmsg.AppendLine("    Automatically Loads stash db in default location");
            helpmsg.AppendLine("Once you begin searching for a particular scene it will populate the list below");
            helpmsg.AppendLine("Select the according file and this program will attempt to clean the string");
            helpmsg.AppendLine("Search For scene url text box will populate and either edit the search title or hit search");
            helpmsg.AppendLine("From here the list will populate with results found from MetaDataAPI");
            helpmsg.AppendLine("Select the scene that matches your scene you want to import");
            helpmsg.AppendLine("Either save directly to Stash DB or rename/save to stash");
            helpmsg.AppendLine("Rename currently only supports the following ( {Publisher - Actor - Title})");
            helpmsg.AppendLine("");
            helpmsg.AppendLine("");

            MessageBox.Show(helpmsg.ToString(), "Help");
        }
        private void btnRemoveScanTag_Click(object sender, RoutedEventArgs e)
        {
            var rslt = MessageBox.Show("Delete scan tag from all videos?", "Double Check", MessageBoxButton.YesNo);
            if (rslt == MessageBoxResult.Yes)
            {
                SQLiteConnection con = new SQLiteConnection(@"Data Source=C:\Users\Branden\.stash\stash-go.sqlite;Version=3;");
                con.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = con;
                command.CommandText = "DELETE FROM scenes_tags WHERE tag_id = 880";
                command.ExecuteNonQuery();
                con.Close();
            }
        }
        private void btnStartStashRenamer_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(xf.stashPornDBScrapper, @"\scrapescenes.py")))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "powershell.exe";
                Process process = new Process();
                process.StartInfo = psi;
                psi.WorkingDirectory = xf.stashPornDBScrapper;
                var rslt = MessageBox.Show("Scan for only 'scan' tag?", "Type", MessageBoxButton.YesNo);
                if (rslt == MessageBoxResult.Yes)
                {
                    psi.Arguments = @"python .\scrapescenes.py --tags scan";
                }
                else
                {
                    psi.Arguments = @"python .\scrapescenes.py";
                }
                process.Start();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(@"scrapescenes.py not found in folder: " + Environment.NewLine + 
                        xf.stashPornDBScrapper, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void btnGenImagesForTags_Click(object sender, RoutedEventArgs e)
        {
            var tagimages = new ImagesToTags();
            tagimages.ShowDialog();
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings ss = new Settings();
            ss.ShowDialog();
            //On Close reload the xml file
            xf = xf.GetXmlSettings();
        }
        private void btnSortFiles_Click(object sender, RoutedEventArgs e)
        {
            //Open Sort Files Window
            SortFiles sf = new SortFiles();
            sf.ShowDialog();
        }
        #endregion

        #region CodeSnippets
        private void lbFileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbFileList.SelectedValue != null)
            {
                FileInfo fi = new FileInfo(lbFileList.SelectedValue.ToString());
                String v = "\"" + fi.FullName + "\"";
                System.Diagnostics.Process.Start(@"D:\Program Files\VideoLAN\VLC\vlc.exe", "\"" + fi.FullName + "\"");
            }
        }
        public string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            //handles the image to db bytes
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        private SceneInfo GetSceneInfo(String URL)
        {
            try
            {
                var json = new WebClient().DownloadString(URL);
                var opts = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                si = JsonConvert.DeserializeObject<SceneInfo>(json, opts);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return si;
        }
        private void ClearTextBoxesOnUpdate()
        {
            txtSearchMetaAPI.Text = "";
            lbSearchList.ItemsSource = "";
            txtNewName.Text = "";
            txtDBTitle.Text = "";
        }
        private void RefreshListFiles()
        {
            string qry = "";
            if (chkAll.IsChecked == true)
            {
                var name = cbSearchDB.Text;
                cbSearchDB.Text = "";
                cbSearchDB.Text = name;
            }
            if (chkNoURL != null)
            {
                if (chkNoURL.IsChecked == true)
                {
                    lbFileList.ItemsSource = "";
                    qry = "SELECT title, path FROM SCENES WHERE url IS NULL";
                }
            }

            if (chkRecent != null)
            {
                if (chkRecent.IsChecked == true)
                {
                    lbFileList.ItemsSource = "";
                    qry = "SELECT title, path FROM SCENES WHERE CREATED_AT > '" + DateTime.Today.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss") + "'" +
                        "and URL IS NULL";
                }
            }
            DbHandler hand = new DbHandler();
            lbFileList.ItemsSource = hand.RunQuery(qry).DefaultView;
        }
        private void LoadFileList()
        {
            //Get all scenes in Stash
            DataTable dt = new DataTable();
            string qry = "SELECT id, path FROM SCENES";

            cbSearchDB.ItemsSource = hand.RunQuery(qry).DefaultView;
        }
        #endregion

        #region Check Boxes
        private void chkSortFldr_Checked(object sender, RoutedEventArgs e)
        {
            //Load files found in the sort folder
            DirectoryInfo di = new DirectoryInfo(xf.SortFolderLocation);
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("path");
            foreach (FileInfo fi in di.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                DataRow dr = dt.NewRow();
                dr["title"] = fi.Name;
                dr["path"] = fi.FullName;
                dt.Rows.Add(dr);
            }
            lbFileList.ItemsSource = dt.DefaultView;
        }
        private void chkRecent_Checked(object sender, RoutedEventArgs e)
        {
            RefreshListFiles();
        }
        private void chkNoURL_Checked(object sender, RoutedEventArgs e)
        {
            RefreshListFiles();
        }
        private void chkAll_Checked(object sender, RoutedEventArgs e)
        {
            RefreshListFiles();
        }
        #endregion

    }
}
