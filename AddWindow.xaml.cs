using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private MainWindow parent;
        private byte[] imageData;
        public AddWindow(MainWindow mw)
        {
            InitializeComponent();
            parent = mw;
            imageData = null;
            this.StatusBox.SelectedIndex = parent.StatusTabControl.SelectedIndex;
            this.PreviewImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/NoImage.png"));
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ItemModel newitem = new ItemModel();
            newitem.Title = TitleBox.Text;
            newitem.ReadProgress = (float)ReadProgressBox.Value;
            newitem.ReadTotal = (float)ReadTotalBox.Value;
            newitem.Rating = (double)RatingBox.Value;
            newitem.Genre = GenreBox.Text;
            newitem.SubGenre1 = Subgenre1Box.Text;
            newitem.SubGenre2 = Subgenre2Box.Text;
            newitem.SubGenre3 = Subgenre3Box.Text;
            newitem.Status = (Status)StatusBox.SelectedIndex;
            newitem.ImageData = this.imageData;

            SqliteDataAccess.InsertItem(newitem);
            parent.RefreshAsync(parent.Page);
            this.Close();
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            ImageSelectionWindow isw = new ImageSelectionWindow();
            bool? ObtainedResult = isw.ShowDialog();
            if (ObtainedResult == true)
            {
                try
                {
                    this.imageData = isw.ImageData;
                    BitmapImage bitmap;
                    using (var stream = new MemoryStream(this.imageData))
                    {
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    this.PreviewImage.Source = bitmap;
                }
                catch
                {
                    this.PreviewImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/NoImage.png"));
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
