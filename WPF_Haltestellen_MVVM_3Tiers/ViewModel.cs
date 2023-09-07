using System.Collections.ObjectModel;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    public class ViewModel
    {
        private readonly ICSVHelper _csvHelper;
        public ObservableCollection<Haltestellen> Haltestellen { get; set; }

        public ViewModel(ICSVHelper csvHelper)
        {
            _csvHelper = csvHelper;
            LoadData("C:\\Users\\MWB\\Source\\Repos\\GidJoe\\WPF_Haltestellen_MVVM_3Tiers\\WPF_Haltestellen_MVVM_3Tiers\\HaltestellenDB.csv");
        }

        public void LoadData(string csvFilePath)
        {
            Haltestellen = _csvHelper.GetAllStations(csvFilePath);
        }

        
    }
}