using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _statusBarText;

        private readonly CSVHelper _csvHelper;
        public ObservableCollection<Haltestellen> Haltestellen { get; set; }
        public string? StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                OnPropertyChanged(nameof(StatusBarText));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ViewModel(CSVHelper csvHelper) 
        {
            _csvHelper = csvHelper;
            LoadData("C:\\Users\\marc1\\source\\repos\\WPF_Haltestellen_MVVM_3Tiers\\WPF_Haltestellen_MVVM_3Tiers\\HaltestellenDB.csv");
        }

        public void LoadData(string csvFilePath)
        {
            Haltestellen = _csvHelper.GetAllStations(csvFilePath);

            StatusBarText = Haltestellen.Count.ToString() + " Haltestellen geladen.";

        }
    }
}