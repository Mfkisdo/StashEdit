using StashEdit.Class;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    /// 
    public partial class Settings : Window
    {
        XmlSettings xf = new XmlSettings();
        public Settings()
        {
            InitializeComponent();
            xf = xf.GetXmlSettings();
            FillTextBoxes();
        }

        private void FillTextBoxes()
        {
            txtSearchLimit.Text = xf.MetaApiLimit;
            txtStashLocation.Text = xf.StashDBLoc;
            txtNameStyle.Text = xf.NewNameStyle;
            txtImageToTag.Text = xf.ImagesToTagsLocation;
            txtSortFolder.Text = xf.SortFolderLocation;
            txtDestFolder.Text = xf.DestinationFolders.Replace(";", Environment.NewLine);
            txtstashscrapper.Text = xf.stashPornDBScrapper;
        }

        private void txtStashLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            xf.StashDBLoc = txtStashLocation.Text;
            xf.WriteToXMLDoc(xf);
        }

        private void txtSearchLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            xf.MetaApiLimit = txtSearchLimit.Text;
            xf.WriteToXMLDoc(xf);
        }

        private void txtNameStyle_TextChanged(object sender, TextChangedEventArgs e)
        {
            xf.NewNameStyle = txtNameStyle.Text;
            xf.WriteToXMLDoc(xf);
        }

        private void txtImageToTag_TextChanged(object sender, TextChangedEventArgs e)
        {
            xf.ImagesToTagsLocation = txtImageToTag.Text;
            xf.WriteToXMLDoc(xf);
        }

        private void txtSortFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            xf.SortFolderLocation = txtSortFolder.Text;
            xf.WriteToXMLDoc(xf);
        }

        private void txtDestFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            xf.DestinationFolders = txtDestFolder.Text.Replace(Environment.NewLine, ";");
            xf.WriteToXMLDoc(xf);
        }

        private void txtstashscrapper_TextChanged(object sender, TextChangedEventArgs e)
        {
            xf.stashPornDBScrapper = txtstashscrapper.Text.Replace(Environment.NewLine, ";");
            xf.WriteToXMLDoc(xf);
        }
    }
}
