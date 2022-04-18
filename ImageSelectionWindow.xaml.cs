using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReadingCompanion
{
    /// <summary>
    /// Interaction logic for ImageSelectionWindow.xaml
    /// </summary>
    public partial class ImageSelectionWindow : Window
    {
        public byte[] ImageData { get; private set; }
        public ImageSelectionWindow()
        {
            InitializeComponent();
            ImageData = null;
        }

        private void FileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "image files (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    this.ImageData = bytes;
                    this.DialogResult = true;
                } 
                catch 
                {
                    this.ErrorLabel.Content = "The selected file could not be loaded";
                }
            } 
        }

        private void URLSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.URLBox.Text == null || this.URLBox.Text == "")) {
                byte[] imageBytes;
                try
                {
                    using (var webClient = new WebClient())
                    {
                        imageBytes = webClient.DownloadData(URLBox.Text);
                    }
                    this.ImageData = imageBytes;
                    this.DialogResult = true;
                } 
                catch
                {
                    this.ErrorLabel.Content = "Invalid image URL";
                }
            } else
            {
                this.ErrorLabel.Content = "Please provide an image URL before submitting";
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
