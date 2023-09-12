using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_Haltestellen_MVVM_3Tiers;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class View : Window
{
    private GridViewColumnHeader? _lastHeaderClicked = null;
    private ListSortDirection _lastDirection = ListSortDirection.Ascending;
    private ViewModel viewModel; // Declare a class-level variable to store the ViewModel instance.


    public View()
    {
        InitializeComponent();

        viewModel = new ViewModel(new CSVHelper()); // Initialize the ViewModel and store it.

        DataContext = viewModel; // Set the DataContext to the ViewModel instance.
    }

    #region Sortierungslogik

    private async void lv_clickOnHeader(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is GridViewColumnHeader headerClicked)
        {
            string headerName = (string)headerClicked.Column.Header;

            // Call the SortData method asynchronously.
            await viewModel.SortData(headerName);
        }
    }


    #endregion Sortierungslogik

    #region Button-Click-Events

    private void MI_ClickOnBeenden(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MI_ClickOnInfo(object sender, RoutedEventArgs e)
    {
        var infoWindow = new InfoWindow();
        infoWindow.ShowDialog();
    }

    private async void MI_DownloadCSV(object sender, RoutedEventArgs e)
    {
        // Instanziierung eines Objektes aus der Klasse 'DownloadHelper.cs'
        var downloadHelper = new DownloadHelper();

        // Fordere den Benutzer auf, einen Speicherort für die heruntergeladene Datei auszuwählen
        string filePath = downloadHelper.OpenSaveDialog("CSV Datei|*.csv", "Speicherort für die CSV-Datei");
        if (string.IsNullOrEmpty(filePath))
        {
            // Wenn der Benutzer den Dialog abbricht, wird die Methode beendet
            return;
        }

        // Hier wird das Fenster für den Download-Fortschritt erstellt und angezeigt.
        // Wir 'instanzieren' ein neues "Fenster" aus der Klasse 'DownloadProgress.xaml.cs'

        var downloadProgressWindow = new DownloadProgress
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner //Setzt das Fenster in die Mitte des Hauptfensters.
        };

        /* Kommentar zu 'Owner = this'
         *
         * Wir setzten unser neues Fenster (var downloadProgressWindow = new DownloadProgress) als "Child" zu unserem
         * Hauptfenster. Damit bezwecken wir, dass das "Child"-Fenster immer im Vordergrund bleibt und nicht hinter ('always on top of owner')
         * und wenn das Hauptfenster geschlossen wird, wird auch das "Child"-Fenster geschlossen.
         * Das Hauptfenster wird außerdem 'disabled' (nicht mehr anklickbar) wenn das "Child"-Fenster geöffnet ist.
         *
         */

        // Ruft das neue Fenster auf
        downloadProgressWindow.Show();

        //var uri = new Uri("https://speed.hetzner.de/1GB.bin"); 1 GB Testdatei
        var uri = new Uri("https://download-data.deutschebahn.com/static/datasets/haltestellen/D_Bahnhof_2020_alle.CSV");
        // Setzt den Dateinamen im Download-Fenster
        downloadProgressWindow.SetFileName(System.IO.Path.GetFileName(uri.AbsolutePath));

        // Definiere einen Fortschrittsberichterstatter, der den Fortschritt an das Download-Fenster weitergibt
        var progressReporter = new Progress<double>(value => downloadProgressWindow.UpdateProgress(value));

        // Aufrufen unserer "DownloadFileAsync" Methode für den Download
        try
        {
            await DownloadHelper.DownloadAsync(
                uri.AbsoluteUri,
                filePath,
                progressReporter,
                downloadProgressWindow.CancellationTokenSource.Token
            );

            if (this.DataContext is ViewModel viewModel)
            {
                viewModel.StatusBarText = "Download erfolgreich.";
            }
        }
        catch (TaskCanceledException)
        {
            if (this.DataContext is ViewModel viewModel)
            {
                viewModel.StatusBarText = "Download abgebrochen.";
            }
        }
        catch (Exception ex)
        {
            if (this.DataContext is ViewModel viewModel)
            {
                viewModel.StatusBarText = $"Fehler beim Herunterladen: {ex.Message}";
            }
            
        }
        finally
        {
            downloadProgressWindow.Close();
        }
    }

    private void lv_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var selectedItem = (Haltestellen)lv.SelectedItem;
        if (selectedItem != null)
        {
            var detailsWindow = new DetailsWindow(selectedItem);
            detailsWindow.ShowDialog();
        }
    }

    private async void MI_LoadCSV(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is ViewModel viewModel)
        {
            await viewModel.LoadDataFromUserSelection();
        }
    }

    #endregion Button-Click-Events


}