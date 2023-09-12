using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _statusBarText;
        private readonly CSVHelper _csvHelper;
        private ObservableCollection<Haltestellen> _haltestellen;

        public event PropertyChangedEventHandler? PropertyChanged;

        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public ObservableCollection<Haltestellen> Haltestellen
        {
            get => _haltestellen;
            set
            {
                if (_haltestellen != value)
                {
                    _haltestellen = value;
                    OnPropertyChanged(nameof(Haltestellen));
                }
            }
        }

        public string? StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                OnPropertyChanged(nameof(StatusBarText));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModel(CSVHelper csvHelper)
        {
            _csvHelper = csvHelper;

            LoadData(DefaultCsvPath);
        }

        private static string DefaultCsvPath => Properties.Settings.Default.CSVPath;

        private async void LoadData(string csvFilePath)
        {
            if (File.Exists(csvFilePath))
            {
                var data = _csvHelper.GetAllStations(csvFilePath);
                Haltestellen = new ObservableCollection<Haltestellen>(data);
                StatusBarText = $"{Haltestellen.Count} Haltestellen geladen aus {csvFilePath}.";
            }
            else
            {
                StatusBarText = "Konnte CSV-Datei nicht laden. Bitte manuell nachladen.";
            }
        }

        public async Task LoadDataFromUserSelection()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedCsvFilePath = openFileDialog.FileName;
                Properties.Settings.Default.CSVPath = selectedCsvFilePath;
                Properties.Settings.Default.Save();

                LoadData(selectedCsvFilePath);
            }
            else
            {
                StatusBarText = "Es wurde keine Datei ausgewählt.";
            }
        }

        public async Task SortData(string header)
        {
            try
            {
                // Wechsel die Sortierrichtung
                _lastDirection = (_lastDirection == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;

                // Update die Statusbar, um anzuzeigen, dass sortiert wird.
                StatusBarText = "Daten werden sortiert...";

                await Task.Run(() =>
                {
                    List<Haltestellen> sortedHaltestellen;

                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        // Sortiere die Haltestellen nach dem ausgewählten Header in aufsteigender Reihenfolge.
                        sortedHaltestellen = Haltestellen.OrderBy(item => GetPropertyValue(item, header)).ToList();
                    }
                    else
                    {
                        // Sortiere die Haltestellen nach dem ausgewählten Header in absteigender Reihenfolge.
                        sortedHaltestellen = Haltestellen.OrderByDescending(item => GetPropertyValue(item, header)).ToList();
                    }

                    // Update die UI auf dem UI-Thread.
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Entferne alle Sortierungen, die bereits vorhanden sind.
                        var dataView = CollectionViewSource.GetDefaultView(Haltestellen);
                        dataView.SortDescriptions.Clear();

                        // Setze die neue Sortierung.
                        SortDescription sd = new SortDescription(header, _lastDirection);
                        dataView.SortDescriptions.Add(sd);

                        // Update die ObservableCollection mit der sortierten Liste.
                        Haltestellen.Clear();
                        foreach (var item in sortedHaltestellen)
                        {
                            Haltestellen.Add(item);
                        }

                        // Update die Statusbar, um anzuzeigen, dass sortiert wurde.
                        StatusBarText = $"{Haltestellen.Count} Haltestellen geladen.";
                    });
                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions here.
                StatusBarText = $"Error: {ex.Message}";
            }
        }

        private object GetPropertyValue(Haltestellen item, string propertyName)
        {
            // Helfermethode zum Abrufen des Header Values
            // aus dem Haltestellen-Objekt.

            var propertyInfo = typeof(Haltestellen).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(item);
            }
            else
            {
                return null;
            }
        }
    }
}