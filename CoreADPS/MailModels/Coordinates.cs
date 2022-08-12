using System;
using System.Globalization;
using Newtonsoft.Json;

namespace CoreADPS.MailModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Coordinates : IEquatable<Coordinates>
    {
        [JsonProperty(PropertyName = "lat")]
        public double Latitude;

        [JsonProperty(PropertyName = "lon")]
        public double Longitude;

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public bool Equals(Coordinates other)
        {
            const double epsilon = 0.000000001;
            var result = (Math.Abs(Latitude - other.Latitude) < epsilon) 
                && (Math.Abs(Longitude - other.Longitude) < epsilon);
            return result;
        }

        public static Coordinates FromString(string input)
        {
            const double maxAbsLat = 90;
            const double maxAbsLon = 180;

            if (input == null)
            {
                return null;
            }

            var groups = input.Split(',');
            if (groups.Length != 2)
            {
                return null;
            }

            double latitude;
            if (!double.TryParse(groups[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out latitude))
            {
                return null;
            }
            if (Math.Abs(latitude) > maxAbsLat)
            {
                return null;
            }

            double longitude;
            if (!double.TryParse(groups[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out longitude))
            {
                return null;
            }
            if (Math.Abs(longitude) > maxAbsLon)
            {
                return null;
            }

            return new Coordinates(latitude, longitude);
        }
    }
}
