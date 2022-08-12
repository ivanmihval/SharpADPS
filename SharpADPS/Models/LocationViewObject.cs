using CoreADPS.MailModels;
using WPFSharpADPS.Helpers;

namespace WPFSharpADPS.Models
{
    public class LocationViewObject: PropertyChangedModel
    {
        private readonly Coordinates _coordinates;
        private bool _isSelected;

        public Coordinates Coordinates
        {
            get { return _coordinates; }
        }

        public double Latitude { get { return _coordinates.Latitude; } }
        public double Longitude { get { return _coordinates.Longitude; } }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public LocationViewObject(Coordinates coordinates)
        {
            _coordinates = coordinates;
        }
    }
}
