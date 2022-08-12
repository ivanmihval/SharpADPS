using System.Linq;
using CoreADPS.Helpers;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class LocationFilter: IMailParamFilter
    {
        public readonly Coordinates FilterCoordinates;
        public readonly double RadiusMeters;

        public LocationFilter(Coordinates coordinates, double radiusMeters)
        {
            FilterCoordinates = coordinates;
            RadiusMeters = radiusMeters;
        }

        public bool IsFiltered(Mail mail)
        {
            return
                mail.RecipientsCoordinates.Any(
                    coords =>
                    DistanceCalculator.Distance(FilterCoordinates, coords) < RadiusMeters);
        }
    }
}
