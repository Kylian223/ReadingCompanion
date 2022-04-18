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

namespace ReadingCompanion
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        MainWindow parent;
        int id;

        public EditWindow(MainWindow mw, ItemModel item)
        {
            InitializeComponent();
            this.parent = mw;
            this.id = item.Id;
            this.TitleBox.Text = item.Title; 
            this.GenreBox.Text = item.Genre; 
            this.Subgenre1Box.Text = item.SubGenre1; 
            this.Subgenre2Box.Text = item.SubGenre2; 
            this.Subgenre3Box.Text = item.SubGenre3; 
            this.ReadProgressBox.Value = (decimal)item.ReadProgress; 
            this.ReadTotalBox.Value = (decimal)item.ReadTotal; 
            this.RatingBox.Value = (decimal)item.Rating;
            this.StatusBox.SelectedIndex = (int)item.Status; 
            this.ImageURLBox.Text = item.ImageURL;
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
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
            newitem.ImageURL = ImageURLBox.Text;
            newitem.Id = this.id;

            SqliteDataAccess.UpdateItem(newitem);
            parent.RefreshAsync(parent.Page);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
