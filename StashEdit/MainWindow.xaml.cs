using Newtonsoft.Json;
using StashEdit.Class;
using StashEdit.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        oSceneInfo SceneByParse = new oSceneInfo();
        GetSceneByID SceneByID = new GetSceneByID();
        int idx = 0;
        string dbID, dbTitle = "";
        DbHandler hand = new DbHandler();
        private XmlSettings xf = new XmlSettings();

        public MainWindow()
        {
            
            try
            {
                InitializeComponent();
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\StarkSettings.xml"))
                {
                    xf.CheckForSettingsXmlFile();
                }

                xf = xf.GetXmlSettings();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
          
        }

        private void lbFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //When a file is selected place title into textbox to search
            if (lbFileList.SelectedItem != null)
            {
                idx = lbFileList.SelectedIndex + 1;
                DataRowView d1 = lbFileList.SelectedItem as DataRowView;
                FileChanges gnfn = new FileChanges();
                //d1["title"].ToString()
                FileInfo fi = new FileInfo(lbFileList.SelectedValue.ToString());
                var BrandNewName = gnfn.CleanFileNameForSearch(fi);
                txtSearchMetaAPI.Text = BrandNewName;


                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = "SELECT id, path, title, url, CAST(created_at as varchar) as dt FROM SCENES WHERE path = :pp";
                cmd.Parameters.Add("pp", DbType.String).Value = lbFileList.SelectedValue.ToString();
                //string qry = "SELECT id, path, title, url, CAST(created_at as varchar) as dt FROM SCENES WHERE path = " + lbFileList.SelectedValue.ToString() + ";
                DataTable dt = hand.RunCommand(cmd);
                if (dt.Rows.Count == 1)
                {
                    dbID = dt.Rows[0]["id"].ToString();
                    dbTitle = dt.Rows[0]["title"].ToString();
                    txtDbInfo.Content = "DB Results";
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Path: " + dt.Rows[0]["path"].ToString());
                    sb.AppendLine("ID: "  + dt.Rows[0]["id"].ToString());
                    sb.AppendLine("Title: " + dt.Rows[0]["title"].ToString());
                    sb.AppendLine("URL: " + dt.Rows[0]["url"].ToString());
                    sb.AppendLine("Created Date: " + dt.Rows[0]["dt"].ToString());
                    sb.AppendLine("");

                    txtDbInfo.Text = sb.ToString();

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
                    //Doesn't always work, does error frequently.
                    Clipboard.SetText("https://beta.metadataapi.net/scenes/" + lbSearchList.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                }
                var x = "https://metadataapi.net/scenes/" + lbSearchList.SelectedValue.ToString();
                SceneByID = GetSceneInfoByID(x);

                if (!chkSortFldr.IsChecked == true)
                {
                    txtDBTitle.Text = SceneByID.data.title;
                }
                
                if (SceneByID.data != null)
                {

                    if (lbFileList.SelectedValue != null && SceneByParse.data[0] != null)
                    {
                        var genName = new FileChanges();
                        string nn = genName.GenNewName(SceneByID, Path.GetExtension(lbFileList.SelectedValue.ToString()));
                        if (!File.Exists(nn))
                        {
                            txtNewName.Text = nn.Replace(": ", "").Replace(@"\", "").Replace(@"/", "");
                        }
                        else
                        {
                            MessageBox.Show("File already exists", "Informational", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No File Selected", "Warning", MessageBoxButton.OK);
                    }

                }
            }
        }
        private async Task SearchForMetaDataAsync()
        {
            string buildqry = "https://api.metadataapi.net/scenes?parse=" + txtSearchMetaAPI.Text + "&" + xf.MetaApiLimit;
            oSceneInfo si = await Task.Run(() => GetSceneInfoByParse(buildqry));
            if (si.data != null && si.data.Count >= 1)
            {
                //Return one match start writing some data includes updating stash db and allows a name change.
                if (lbFileList.SelectedValue != null)
                {
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
                    Xceed.Wpf.Toolkit.MessageBox.Show("No File Selected", "Informational", MessageBoxButton.OK, MessageBoxImage.Information);
                }

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
                        cont.id = dbID;
                        cont.NewPath = System.IO.Path.Combine(OldFileInfo.Directory.FullName, NewName);
                        cont.OldFile = OldFileInfo;
                        cont.scninfo = SceneByID;
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
                    if (chkSortFldr.IsChecked == true)
                    {
                        FileInfo nf = new FileInfo(System.IO.Path.Combine(OldFileInfo.Directory.FullName, NewName));
                        //Reset the listbox after saving
                        chkSortFldr.IsChecked = false;
                        chkSortFldr.IsChecked = true;

                        lbFileList.SelectedValue = nf.FullName;

                        //display message to move new saved to a destination folder location if available
                        SortFiles sf = new SortFiles();
                        var MoveFile = sf.MoveFileMatchingDestPaths(nf);
                        if (!File.Exists(MoveFile[1]))
                        {
                            //Ask to move
                            var d = Xceed.Wpf.Toolkit.MessageBox.Show("Would you like to move file to: " + Environment.NewLine + MoveFile[1], "Informational", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (d == MessageBoxResult.Yes)
                            {
                                File.Move(MoveFile[0],MoveFile[1]);
                                chkSortFldr.IsChecked = false;
                                chkSortFldr.IsChecked = true;
                                txtSearchMetaAPI.Text = "";
                                lbSearchList.ItemsSource = "";
                            }
                        }
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
                cont.id = dbID;
                cont.NewPath = stashFi.FullName;
                cont.OldFile = stashFi;
                cont.scninfo = SceneByID;
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
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Python\scrapeScenes"))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "powershell.exe";
                Process process = new Process();
                process.StartInfo = psi;
                psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\Python";
                String[] configpy = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Python\configuration.py");
                if (configpy[2].Contains("<IP_ADDRESS>"))
                {
                    //File has not been setup alert!
                    var setup = MessageBox.Show("Config file has not been setup, wish to do this now?", "Type", MessageBoxButton.YesNo);
                    if (setup == MessageBoxResult.Yes)
                    {
                        //"C:\Users\Branden\source\repos\StashEdit\StashEdit\bin\Debug\netcoreapp3.1\Python\configuration.py"
                        Process.Start("explorer.exe", "/select, " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Python", @"configuration.py"));
                    }
                }
                else
                {
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
                
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(@"scrapescenes.py not found in folder: " + Environment.NewLine +
                        AppDomain.CurrentDomain.BaseDirectory + @"\Python\scrapeScenes.py", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
        /// <summary>
        /// Get json data by ID
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private GetSceneByID GetSceneInfoByID(String URL)
        {
            try
            {
                var json = new WebClient().DownloadString(URL);
                var opts = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                SceneByID = JsonConvert.DeserializeObject<GetSceneByID>(json, opts);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return SceneByID;
        }

        /// <summary>
        /// Get json data by parse not ID
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private oSceneInfo GetSceneInfoByParse(String URL)
        {
            try
            {
                var json = new WebClient().DownloadString(URL);
                var opts = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                SceneByParse = JsonConvert.DeserializeObject<oSceneInfo>(json, opts);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return SceneByParse;
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
            btnSave.Content = "Save Filename";
            if (Directory.Exists(xf.SortFolderLocation))
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
          
        }
        private void chkRecent_Checked(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "Save New Name and Update Stash";
            RefreshListFiles();
        }
        private void chkNoURL_Checked(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "Save New Name and Update Stash";
            RefreshListFiles();
        }
        private void chkAll_Checked(object sender, RoutedEventArgs e)
        {
            btnSave.Content = "Save New Name and Update Stash";
            RefreshListFiles();
        }
        #endregion

       

    }
}
