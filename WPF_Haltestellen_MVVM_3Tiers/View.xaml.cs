using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_Haltestellen_MVVM_3Tiers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class View : Window
    {
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private DownloadHelper _downloadHelper;
        public View()
        {
            InitializeComponent();
            var csvHelper = new CSVHelper(); 
            DataContext = new ViewModel(csvHelper);

        }

    #region Sortierungslogik
        private void lv_clickOnHeader(object sender, RoutedEventArgs e)
        {
            ListSortDirection direction;

            if (e.OriginalSource is GridViewColumnHeader headerClicked)
            {
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }

                string header = (string)headerClicked.Column.Header;
                Sorting(header, direction);

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }
        }

        private void Sorting(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lv.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    #endregion

    #region Button-Click-Events
    private void MenuItem_ClickOnExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    #endregion

    private void MenuItem_ClickOnDLNewCSV(object sender, RoutedEventArgs e)
    {
        //_downloadHelper.DownloadFileAsync();
    }

    private void MenuItem_ClickOnInfo(object sender, RoutedEventArgs e)
    {
        var infoWindow = new InfoWindow();
        infoWindow.ShowDialog();
    }
    
}