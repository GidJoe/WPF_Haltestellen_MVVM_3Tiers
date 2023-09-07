using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    public class ViewModel
    {
        private readonly ICSVHelper _csvHelper;
        public ObservableCollection<Haltestellen> Haltestellen { get; set; }


        public ViewModel(ICSVHelper csvHelper)
        {
            _csvHelper = csvHelper;
            LoadData("C:\\Users\\marc1\\source\\repos\\WPF_Haltestellen_MVVM_3Tiers\\WPF_Haltestellen_MVVM_3Tiers\\HaltestellenDB.csv");
        }

        public void LoadData(string csvFilePath)
        {
            Haltestellen = _csvHelper.GetAllStations(csvFilePath);
        }



    }
}