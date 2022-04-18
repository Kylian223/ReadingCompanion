using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace ReadingCompanion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Page;
        public MainWindow()
        {
            if (!System.IO.File.Exists("ReadingCompanionDB.db"))
               SqliteDataAccess.CreateDB();

            InitializeComponent();
            Page = 0;
        }

        #region Events

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow aw = new AddWindow(this);
            aw.ShowDialog();
        }

        private async void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)sender;
            int index = 0;

            if (tab.Name == "StatusTab_0")
                index = 0;
            else if (tab.Name == "StatusTab_1")
                index = 1;
            else if (tab.Name == "StatusTab_2")
                index = 2;
            else if (tab.Name == "StatusTab_3")
                index = 3;
            else if (tab.Name == "StatusTab_4")
                index = 4;

            LoadItemsAsync((Status)index, 0); //Not awaiting intentionally to prevent the application from freezing up
            Page = 0;
        }

        private async void ItemAmountCombo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)ItemAmountCombo.SelectedItem;
            SqliteDataAccess.PageAmount = int.Parse(typeItem.Content.ToString());
            this.Page = 0;
            RefreshAsync(0);
        }

        private async void SearchParameter_Changed(object sender, RoutedEventArgs e)
        {
            this.Page = 0;
            RefreshAsync(0);
        }
        #endregion

        #region Functions

        /// <summary>
        /// Loads all items of a given tab and a given page.
        /// </summary>
        public async Task LoadItemsAsync(Status status, int page)
        {
            StackPanel panel = new StackPanel();
            int sorting = 0;
            string searchQuery = "";

            switch (status)
            {
                case Status.Reading:
                    panel = Reading_Panel;
                    searchQuery = SearchBox_0.Text;
                    sorting = SortCombo_0.SelectedIndex;
                    break;
                case Status.OnHold:
                    panel = Onhold_Panel;
                    searchQuery = SearchBox_1.Text;
                    sorting = SortCombo_1.SelectedIndex;
                    break;
                case Status.Backlog:
                    panel = Backlog_Panel;
                    searchQuery = SearchBox_2.Text;
                    sorting = SortCombo_2.SelectedIndex;
                    break;
                case Status.Completed:
                    panel = Completed_Panel;
                    searchQuery = SearchBox_3.Text;
                    sorting = SortCombo_3.SelectedIndex;
                    break;
                case Status.Dropped:
                    panel = Dropped_Panel;
                    searchQuery = SearchBox_4.Text;
                    sorting = SortCombo_4.SelectedIndex;
                    break;
            }

            List<ItemModel> items = await Task.Run(()=>SqliteDataAccess.SelectItems(status, page, searchQuery,(SortBy)sorting));
            int itemAmount = await Task.Run(() => SqliteDataAccess.SelectRowCount(status, searchQuery));
           
            panel.Children.Clear();

            foreach(ItemModel item in items)
            {
                panel.Children.Add(GenerateGrid(item));
            }
            GeneratePageButtons(itemAmount);

            if(itemAmount == 0) //Shows a label on screen when no items are found
            {
                Label noresultslabel = new Label();
                noresultslabel.FontSize = 25;
                noresultslabel.Margin = new Thickness(0,20,0,0);
                noresultslabel.Content = "No results found";
                noresultslabel.SetResourceReference(Control.ForegroundProperty, "SecondaryTextColor");
                noresultslabel.HorizontalAlignment = HorizontalAlignment.Center;
                panel.Children.Add(noresultslabel);
            }
        }

        /// <summary>
        /// Refreshes the current tab on a given page.
        /// </summary>
        public async Task RefreshAsync(int page)
        {
            LoadItemsAsync((Status)StatusTabControl.SelectedIndex, page); //Not awaiting intentionally to prevent the application from freezing up
        }

        #endregion

        #region Control Generating Functions

        /// <summary>
        /// Programmatically creates a grid element with all visualization for items in the application.
        /// TODO: find a way to optimize this, find a way to clean up the code
        /// </summary>
        private Grid GenerateGrid(ItemModel item)
        {
            Grid grid = new Grid();
            grid.Background = new SolidColorBrush(Color.FromRgb(86, 81, 84));
            grid.Margin = new Thickness(0, 15, 0, 0);
            grid.Height = 210;

            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();
            ColumnDefinition col4 = new ColumnDefinition();
            ColumnDefinition col5 = new ColumnDefinition();
            col1.Width = new GridLength(160);
            col2.Width = new GridLength(1,GridUnitType.Star);
            col3.Width = new GridLength(1,GridUnitType.Star);
            col4.Width = new GridLength(1,GridUnitType.Star);
            col5.Width = new GridLength(1,GridUnitType.Star);
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            grid.ColumnDefinitions.Add(col3);
            grid.ColumnDefinitions.Add(col4);
            grid.ColumnDefinitions.Add(col5);

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();
            RowDefinition row5 = new RowDefinition();
            row1.Height = new GridLength(1, GridUnitType.Star);
            row2.Height = new GridLength(1, GridUnitType.Star);
            row3.Height = new GridLength(1, GridUnitType.Star);
            row4.Height = new GridLength(1, GridUnitType.Star);
            row5.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);
            grid.RowDefinitions.Add(row5);

            Image image = new Image();
            try
            {
                if (item.ImageURL == "" || item.ImageURL == null)
                    image.Source = new BitmapImage(new Uri("pack://application:,,,/Images/NoImage.png"));
                else
                    image.Source = new BitmapImage(new Uri($"{item.ImageURL}"));
            }
            catch (Exception ex)
            {
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Images/NoImage.png"));
            }

            image.Height = 200;
            image.Width = 150;
            image.Margin = new Thickness(5, 0, 0, 0);
            image.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(image, 0);
            Grid.SetRowSpan(image, 5);
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);

            Label titleLabel = new Label();
            titleLabel.HorizontalAlignment = HorizontalAlignment.Left;
            titleLabel.FontSize = 24;
            titleLabel.FontWeight = FontWeights.DemiBold;
            titleLabel.Content = item.Title;
            titleLabel.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            Grid.SetRow(titleLabel, 0);
            Grid.SetColumn(titleLabel, 1);
            Grid.SetColumnSpan(titleLabel, 4);
            grid.Children.Add(titleLabel);

            TextBlock genreTextBlock = new TextBlock();
            genreTextBlock.Padding = new Thickness(5, 0, 0, 0);
            genreTextBlock.FontSize = 16;
            genreTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            genreTextBlock.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");

            if(!(item.Genre == null || item.Genre == ""))
                genreTextBlock.Text = $"Genre: {item.Genre}";
            if (!((item.SubGenre1 == null || item.SubGenre1 == "") && (item.SubGenre2 == null || item.SubGenre2 == "") && (item.SubGenre3 == null || item.SubGenre3 == "")))
                genreTextBlock.Text += "\n\nSubgenre(s):";
            if (item.SubGenre1 != null)
                genreTextBlock.Text += $"\n{item.SubGenre1}";
            if (item.SubGenre2 != null)
                genreTextBlock.Text += $"\n{item.SubGenre2}";
            if (item.SubGenre3 != null)
                genreTextBlock.Text += $"\n{item.SubGenre3}";
            genreTextBlock.TextWrapping = TextWrapping.Wrap;
            Grid.SetRow(genreTextBlock, 1);
            Grid.SetRowSpan(genreTextBlock, 4);
            Grid.SetColumn(genreTextBlock, 1);
            Grid.SetColumnSpan(genreTextBlock, 2);
            grid.Children.Add(genreTextBlock);

            StackPanel progressPanel = new StackPanel();
            progressPanel.Orientation = Orientation.Horizontal;
            progressPanel.HorizontalAlignment = HorizontalAlignment.Right;
            progressPanel.Margin = new Thickness(0, 0, 5, 0);
            Grid.SetRow(progressPanel, 1);
            Grid.SetColumn(progressPanel, 3);
            Grid.SetColumnSpan(progressPanel, 2);
            
            Label progressLabel1 = new Label();
            progressLabel1.Content = "Progress: ";
            progressLabel1.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            progressLabel1.FontSize = 12;
            progressLabel1.VerticalAlignment = VerticalAlignment.Center;
            progressPanel.Children.Add(progressLabel1);

            DecimalUpDown readProgress = new DecimalUpDown();
            readProgress.Background = new SolidColorBrush(Colors.Transparent);
            readProgress.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            readProgress.ShowButtonSpinner = false;
            readProgress.VerticalAlignment = VerticalAlignment.Center;
            readProgress.FormatString = "F1";
            readProgress.Maximum = 99999;
            readProgress.Minimum = 0;
            readProgress.ClipValueToMinMax = true;
            readProgress.Value = (decimal)item.ReadProgress;
            readProgress.Height = 20;
            readProgress.Width = 50;
            readProgress.FontSize = 12;
            readProgress.ValueChanged += (s, e) =>
            {
                try { 
                    SqliteDataAccess.UpdateItemReadProgress(item.Id, (float)readProgress.Value);
                }
                catch (Exception ex) { };
            };
            progressPanel.Children.Add(readProgress);

            Label progressLabel2 = new Label();
            progressLabel2.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            progressLabel2.Content = " / ";
            progressLabel2.FontSize = 12;
            progressLabel2.VerticalAlignment = VerticalAlignment.Center;
            progressPanel.Children.Add(progressLabel2);

            DecimalUpDown readTotal = new DecimalUpDown();
            readTotal.Background = new SolidColorBrush(Colors.Transparent);
            readTotal.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            readTotal.ShowButtonSpinner = false;
            readTotal.VerticalAlignment = VerticalAlignment.Center;
            readTotal.FormatString = "F1";
            readTotal.Maximum = 99999;
            readTotal.Minimum = 0;
            readTotal.ClipValueToMinMax = true;
            readTotal.Value = (decimal)item.ReadTotal;
            readTotal.Height = 20;
            readTotal.Width = 50;
            readTotal.FontSize = 12;
            readTotal.ValueChanged += (s, e) =>
            {
                try {
                    SqliteDataAccess.UpdateItemReadTotal(item.Id, (float)readTotal.Value);
                }
                catch (Exception ex) { };
                
            };
            progressPanel.Children.Add(readTotal);

            grid.Children.Add(progressPanel);

            StackPanel ratingPanel = new StackPanel();
            ratingPanel.Orientation = Orientation.Horizontal;
            ratingPanel.HorizontalAlignment = HorizontalAlignment.Right;
            ratingPanel.Margin = new Thickness(0, 0, 5, 0);
            Grid.SetRow(ratingPanel, 2);
            Grid.SetColumn(ratingPanel, 3);
            Grid.SetColumnSpan(ratingPanel, 2);

            Label ratingLabel1 = new Label();
            ratingLabel1.Content = "Rating: ";
            ratingLabel1.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            ratingLabel1.FontSize = 12;
            ratingLabel1.VerticalAlignment = VerticalAlignment.Center;
            ratingPanel.Children.Add(ratingLabel1);

            DecimalUpDown rating = new DecimalUpDown();
            rating.Background = new SolidColorBrush(Colors.Transparent);
            rating.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            rating.VerticalAlignment = VerticalAlignment.Center;
            rating.ShowButtonSpinner = false;
            rating.FormatString = "F1";
            rating.Maximum = 10;
            rating.Minimum = 0;
            rating.ClipValueToMinMax = true;
            rating.Value = (decimal)item.Rating;
            rating.Height = 20;
            rating.Width = 30;
            rating.FontSize = 12;
            rating.ValueChanged += (s, e) =>
            {
                try { 
                    SqliteDataAccess.UpdateItemRating(item.Id, (double)rating.Value);
                }
                catch (Exception ex) { };
            };
            ratingPanel.Children.Add(rating);

            Label ratingLabel2 = new Label();
            ratingLabel2.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            ratingLabel2.Content = " / 10";
            ratingLabel2.FontSize = 12;
            ratingLabel2.VerticalAlignment = VerticalAlignment.Center;
            ratingPanel.Children.Add(ratingLabel2);

            grid.Children.Add(ratingPanel);

            ComboBox statusCombo = new ComboBox();
            statusCombo.Items.Add(Status.Reading);
            statusCombo.Items.Add(Status.OnHold);
            statusCombo.Items.Add(Status.Backlog);
            statusCombo.Items.Add(Status.Completed);
            statusCombo.Items.Add(Status.Dropped);
            statusCombo.HorizontalAlignment = HorizontalAlignment.Right;
            statusCombo.VerticalAlignment = VerticalAlignment.Top;
            statusCombo.Margin = new Thickness(0, 10, 5, 0);
            statusCombo.Height = 22;
            statusCombo.Width = 88;
            statusCombo.FontSize = 12;
            statusCombo.SelectedIndex = this.StatusTabControl.SelectedIndex;
            statusCombo.SelectionChanged += (s, e) => 
            {
                this.Page = 0;
                SqliteDataAccess.UpdateItemStatus(item.Id, statusCombo.SelectedIndex);
                RefreshAsync(0);
            };
            Grid.SetRow(statusCombo, 3);
            Grid.SetColumn(statusCombo, 4);
            grid.Children.Add(statusCombo);

            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(buttonPanel, 4);
            Grid.SetColumn(buttonPanel, 3);
            Grid.SetColumnSpan(buttonPanel, 2);

            Button editButton = new Button();
            Image editImage = new Image();
            editImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/EditImage.png"));
            editImage.VerticalAlignment = VerticalAlignment.Center;
            RenderOptions.SetBitmapScalingMode(editImage, BitmapScalingMode.Fant);
            editButton.Content = editImage;
            editButton.SetResourceReference(Control.StyleProperty, "ButtonStyle");
            editButton.Width = 50;
            editButton.Margin = new Thickness(0, 5, 5, 5);
            editButton.Click += (s, e) =>
            {
                EditWindow ew = new EditWindow(this,item);
                ew.ShowDialog();
            };
            buttonPanel.Children.Add(editButton);

            Button removeButton = new Button();
            removeButton.Click += (s, e) => {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this entry?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SqliteDataAccess.DeleteItem(item.Id);
                    this.Page = 0;
                    this.RefreshAsync(0);
                }
            };
            Image removeImage = new Image();
            removeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/DeleteImage.png"));
            removeImage.VerticalAlignment = VerticalAlignment.Center;
            RenderOptions.SetBitmapScalingMode(removeImage,BitmapScalingMode.Fant);
            removeButton.Content = removeImage;
            removeButton.SetResourceReference(Control.StyleProperty, "ButtonStyle");
            removeButton.Width = 50;
            removeButton.Margin = new Thickness(0, 5, 5, 5);
            buttonPanel.Children.Add(removeButton);

            grid.Children.Add(buttonPanel);

            return grid;
        }

        /// <summary>
        /// Generates controls that form the page switch buttons on the bottom right of the application.
        /// </summary>
        private void GeneratePageButtons(int itemAmount)
        {
            this.PagePanel.Children.Clear();
            bool leftEnabled = false;
            bool rightEnabled = false;
            if (this.Page != 0)
                leftEnabled = true;
            if(itemAmount > (this.Page + 1) * SqliteDataAccess.PageAmount)
                rightEnabled= true;


            Label leftArrow = new Label();
            leftArrow.FontSize = 22;
            leftArrow.Content = "<";
            if (leftEnabled)
            {
                leftArrow.SetResourceReference(Control.ForegroundProperty, "SecondaryTextColor");
                leftArrow.MouseLeftButtonDown += (s, e) => 
                {
                    this.Page -= 1;
                    RefreshAsync(this.Page);
                };
                leftArrow.MouseEnter += (s, e) => { leftArrow.SetResourceReference(Control.ForegroundProperty, "HighlightColor"); };
                leftArrow.MouseLeave += (s, e) => { leftArrow.SetResourceReference(Control.ForegroundProperty, "SecondaryTextColor"); };
            }
            else
                leftArrow.SetResourceReference(Control.ForegroundProperty, "SecondaryBackgroundColor");
            this.PagePanel.Children.Add(leftArrow);

            Label currentPage = new Label();
            currentPage.FontSize = 22;
            currentPage.Content = this.Page + 1;
            currentPage.SetResourceReference(Control.ForegroundProperty, "PrimaryTextColor");
            this.PagePanel.Children.Add(currentPage);

            Label rightArrow = new Label();
            rightArrow.FontSize = 22;
            rightArrow.Content = ">";
            if (rightEnabled)
            {
                rightArrow.SetResourceReference(Control.ForegroundProperty, "SecondaryTextColor");
                rightArrow.MouseLeftButtonDown += (s, e) =>
                {
                    this.Page += 1;
                    RefreshAsync(this.Page);
                };
                rightArrow.MouseEnter += (s, e) => { rightArrow.SetResourceReference(Control.ForegroundProperty, "HighlightColor"); };
                rightArrow.MouseLeave += (s, e) => { rightArrow.SetResourceReference(Control.ForegroundProperty, "SecondaryTextColor"); };
            }
            else
                rightArrow.SetResourceReference(Control.ForegroundProperty, "SecondaryBackgroundColor");
            this.PagePanel.Children.Add(rightArrow);
        }
        #endregion
    }
}
