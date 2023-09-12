using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    public class CSVHelper
    {
        public ObservableCollection<Haltestellen> GetAllStations(string csvFilePath)
        {

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });
            var records = csv.GetRecords<Haltestellen>().ToList();
            return new ObservableCollection<Haltestellen>(records);

        }


    }
}
