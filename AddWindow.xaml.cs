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
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private MainWindow parent;
        public AddWindow(MainWindow mw)
        {
            InitializeComponent();
            parent = mw;
            this.StatusBox.SelectedIndex = parent.StatusTabControl.SelectedIndex;
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
            newitem.ImageURL = ImageURLBox.Text;

            SqliteDataAccess.InsertItem(newitem);
            parent.RefreshAsync(parent.Page);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
