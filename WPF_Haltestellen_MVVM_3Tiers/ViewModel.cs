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
                // Toggle the sort direction.
                _lastDirection = (_lastDirection == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;

                // Update the status bar text to indicate sorting.
                StatusBarText = "Daten werden sortiert...";

                await Task.Run(() =>
                {
                    List<Haltestellen> sortedHaltestellen;

                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        // Sort in ascending order based on the selected header.
                        sortedHaltestellen = Haltestellen.OrderBy(item => GetPropertyValue(item, header)).ToList();
                    }
                    else
                    {
                        // Sort in descending order based on the selected header.
                        sortedHaltestellen = Haltestellen.OrderByDescending(item => GetPropertyValue(item, header)).ToList();
                    }

                    // Update the UI on the UI thread.
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Clear existing sort descriptions.
                        var dataView = CollectionViewSource.GetDefaultView(Haltestellen);
                        dataView.SortDescriptions.Clear();

                        // Set the new sort description.
                        SortDescription sd = new SortDescription(header, _lastDirection);
                        dataView.SortDescriptions.Add(sd);

                        // Update the ObservableCollection with the sorted list.
                        Haltestellen.Clear();
                        foreach (var item in sortedHaltestellen)
                        {
                            Haltestellen.Add(item);
                        }

                        // Clear the status bar text.
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
            // Use reflection to get the property value based on the property name (header).
            var propertyInfo = typeof(Haltestellen).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(item);
            }
            else
            {
                return null; // Handle unsupported headers or return null.
            }
        }


    }
}
