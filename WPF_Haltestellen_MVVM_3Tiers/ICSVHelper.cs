using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    public interface ICSVHelper
    {
        ObservableCollection<Haltestellen> GetAllStations(string csvFilePath);
        
    }
}
