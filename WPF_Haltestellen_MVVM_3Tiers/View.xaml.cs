﻿using System;
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
    
    public View()
    {
        InitializeComponent();
        
        DataContext = new ViewModel(new CSVHelper());
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

    #endregion Sortierungslogik

    #region Button-Click-Events

    private void MenuItem_ClickOnExit(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
    private void MenuItem_ClickOnInfo(object sender, RoutedEventArgs e)
    {
        var infoWindow = new InfoWindow();
        infoWindow.ShowDialog();
    }

    private async void MenuItem_ClickOnDLNewCSV(object sender, RoutedEventArgs e)
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

            MessageBox.Show("Download erfolgreich abgeschlossen");
        }
        catch (TaskCanceledException)
        {
            MessageBox.Show("Download wurde abgebrochen.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Herunterladen: {ex.Message}");
        }
        finally
        {
            downloadProgressWindow.Close();
        }
    }
    #endregion Button-Click-Events

    private void lv_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        
        var selectedItem = (Haltestellen)lv.SelectedItem;
        if (selectedItem != null)
        {
           var detailsWindow = new DetailsWindow(selectedItem);
            detailsWindow.ShowDialog();
        }
                
        
    }
}