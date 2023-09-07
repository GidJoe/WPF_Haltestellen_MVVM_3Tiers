using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WPF_Haltestellen_MVVM_3Tiers
{
    internal class CSVHelper : ICSVHelper
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
