using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CoreADPS;

namespace WPFSharpADPS.Helpers
{
    public struct WorldCitiesRow
    {
        public string City;
        public double Latitude;
        public double Longitude;
        public string Country;
        public ulong Population;
    }

    public class WorldCitiesParser
    {
        public const string CsvFileName = "big_cities.csv";

        private const int ColumnsNumber = 11;
        private const int CityColumnIdx = 1;
        private const int LatitudeColumnIdx = 2;
        private const int LongitudeColumnIdx = 3;
        private const int CountryColumnIdx = 5;
        private const int PopulationColumnIdx = 9;

        private static string _removeQuotes(string input)
        {
            return input.Substring(1, input.Length - 2);
        }

        private static bool _isEnclosedWithQuotes(string input)
        {
            return input[0] == '"' && input[input.Length - 1] == '"';
        }

        public static string GetCsvFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CsvFileName);
        }

        public static IEnumerable<WorldCitiesRow> GetRows(bool skipFirstLine=true)
        {
            var lines = File.ReadAllLines(GetCsvFilePath(), Unicode.Utf8WithouBom);
            for (var i = 0; i < lines.Length; i++)
            {
                if (skipFirstLine && i == 0)
                {
                    continue;
                }

                var line = lines[i];
                var values = line.Split(',');
                if (values.Length != ColumnsNumber)
                {
                    continue;
                }

                var shouldBreakIteration = false;
                foreach (var columnIndex in new[] { CityColumnIdx, LatitudeColumnIdx, LongitudeColumnIdx, CountryColumnIdx, PopulationColumnIdx })
                {
                    if (!_isEnclosedWithQuotes(values[columnIndex]))
                    {
                        shouldBreakIteration = true;
                        break;
                    }
                }
                if (shouldBreakIteration)
                {
                    continue;
                }

                ulong populationValue;
                
                var isPopulationParsed = ulong.TryParse(_removeQuotes(values[PopulationColumnIdx]), out populationValue);
                if (!isPopulationParsed)
                {
                    continue;
                }

                double latitudeValue;
                var isLatitudeParsed = double.TryParse(_removeQuotes(values[LatitudeColumnIdx]), NumberStyles.Any, CultureInfo.InvariantCulture, out latitudeValue);
                if (!isLatitudeParsed)
                {
                    continue;
                }

                double longitudeValue;
                var isLongitudeParsed = double.TryParse(_removeQuotes(values[LongitudeColumnIdx]), NumberStyles.Any, CultureInfo.InvariantCulture, out longitudeValue);
                if (!isLongitudeParsed)
                {
                    continue;
                }

                var cityValue = _removeQuotes(values[CityColumnIdx]);
                var countryValue = _removeQuotes(values[CountryColumnIdx]);

                yield return new WorldCitiesRow
                    {
                        City = cityValue,
                        Country = countryValue,
                        Latitude = latitudeValue,
                        Longitude = longitudeValue,
                        Population = populationValue,
                    };
            }
            
        }
    }
}
