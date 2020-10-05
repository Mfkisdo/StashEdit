using StashEdit.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Interaction logic for SortFiles.xaml
    /// </summary>
    public partial class SortFiles : Window
    {
        XmlSettings xm = new XmlSettings();
        List<FileMove> fm = new List<FileMove>();
        public SortFiles()
        {
            InitializeComponent();
            xm = xm.GetXmlSettings();
            GetFolderLocations();
        }

        private void GetFolderLocations()
        {
            txtSourceFolder.Text = xm.SortFolderLocation;
            txtDestFolders.Text = xm.DestinationFolders.Replace(";", Environment.NewLine);
        }

        private void btnMoveFiles_Click(object sender, RoutedEventArgs e)
        {
            biBusy.IsBusy = true;
            biBusy.BusyContent = "Getting Files To Move";
            BackgroundWorker worker = new BackgroundWorker();
            FolderSearch fs = new FolderSearch();
            fs.SourceFolder = txtSourceFolder.Text;
            fs.DestFolders = txtDestFolders.Text.Split(Environment.NewLine);

           worker.RunWorkerAsync(fs);

            worker.ProgressChanged += (o, ea) =>
            {

            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                biBusy.IsBusy = false;
                var xfm = ea.Result;
                fm = (List<FileMove>)xfm;
            };

            worker.DoWork += (o, ea) =>
            {
                FolderSearch fs = ea.Argument as FolderSearch;
                List<FileMove> fml = MoveFiles(fs.DestFolders, fs.SourceFolder);
                List<string> list = new List<string>();

                foreach (var fm in fml)
                {
                    list.Add("From " +fm.FileFrom + Environment.NewLine + "To:"  + fm.FileTo + Environment.NewLine);
                }
                if (list.Count != 0)
                {
                    //use the Dispatcher to delegate the listOfStrings collection back to the UI
                    Dispatcher.Invoke((Action)(() => txtLog.Content = "Results"));
                    Dispatcher.Invoke((Action)(() => txtLog.Text = String.Join(Environment.NewLine, list)));
                    ea.Result = fml;
                }
                else
                {
                    Dispatcher.Invoke((Action)(() => txtLog.Content = "Nothing found"));
                }
                
            };
        }

        private List<FileMove> MoveFiles(string[] destfolders, string sortfolder)
        {
            List<FileMove> fml = new List<FileMove>();


            foreach (string path in destfolders)
            {
                if (Directory.Exists(path))
                {
                    if (Directory.Exists(sortfolder))
                    {
                        DirectoryInfo DestFolders = new DirectoryInfo(path);
                        DirectoryInfo SourceFiles = new DirectoryInfo(sortfolder);

                        foreach (var fi in SourceFiles.GetFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            String srcCleanName = fi.Name.Replace(".", " ").Replace("_", " ")
                                                                                .Replace("-", " ").Replace(",", " ").ToLower();
                            foreach (DirectoryInfo dir in DestFolders.GetDirectories("*", SearchOption.AllDirectories))
                            {
                                if (srcCleanName.Contains(dir.Name.ToLower()))
                                {
                                    //Found match move file to folder
                                    FileMove fm = new FileMove();
                                    fm.FileFrom = fi.FullName;
                                    fm.FileTo = System.IO.Path.Combine(dir.FullName, fi.Name);
                                    fml.Add(fm);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return fml;
        }

        public string[] MoveFileMatchingDestPaths(FileInfo fi)
        {
            string[] MoveToFolder = { "", "" };
            xm = xm.GetXmlSettings();
            Dictionary<string, string> EmployeeList = new Dictionary<string, string>();
            var paths = xm.DestinationFolders.Split(";");
            foreach(string path in paths)
            {
                DirectoryInfo pdir = new DirectoryInfo(path);
                String srcCleanName = fi.Name.Replace(".", " ").Replace("_", " ")
                                                                                .Replace("-", " ").Replace(",", " ").ToLower();
                foreach(DirectoryInfo dir in pdir.GetDirectories("*", SearchOption.AllDirectories))
                {
                    if (srcCleanName.Contains(dir.Name.ToLower()))
                    {
                        //Found match move file to folder
                        MoveToFolder[0] = fi.FullName;
                        MoveToFolder[1] = System.IO.Path.Combine(dir.FullName, fi.Name);
                    }
                }
                
            }

            return MoveToFolder;
            
        }

        private void btnCommit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mf in fm)
            {
                if (!File.Exists(mf.FileTo))
                { 
                    //Only move if the file doesn't exists if it does let the user know
                    File.Move(mf.FileFrom, mf.FileTo);
                    Thread.Sleep(500);
                }
                else
                {
                    MessageBox.Show("File Already Exists " + Environment.NewLine + mf.FileTo, "Notice");
                }
                //Clear the box and the list
                txtLog.Text = "";
                fm.Clear();
            }
        }
    }

}

public class FileMove
{
    public string FileFrom { get; set; }

    public string FileTo { get; set; }
}

public class FolderSearch
{
    public string SourceFolder { get; set; }
    public string[] DestFolders { get; set; }
}
