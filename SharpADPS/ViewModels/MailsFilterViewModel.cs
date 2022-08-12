using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CoreADPS.Filters;
using CoreADPS.Helpers;
using CoreADPS.MailModels;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.Helpers.MessageBoxProvider;
using WPFSharpADPS.Models.Translation;

namespace WPFSharpADPS.ViewModels
{
    public enum MailsFilterDialogResultStatus
    {
        NewSearch,
        RefineCurrentSearch,
        UniteResults,
        CancelClicked,
    }

    public class MailsFilterDialogResult
    {
        public readonly MailsFilterDialogResultStatus Status;
        public readonly ICollection<IMailParamFilter> Filters;

        public MailsFilterDialogResult(MailsFilterDialogResultStatus status, ICollection<IMailParamFilter> filters)
        {
            Status = status;
            Filters = filters;
        }
    }

    public class FilterModeComboboxItem
    {
        public TranslationViewObject TranslationViewObject;
        public MailsFilterDialogResultStatus Status { get; set; }

        public string Title
        {
            get
            {
                switch (Status)
                {
                    case MailsFilterDialogResultStatus.NewSearch:
                        return TranslationViewObject.MailsFilterSearchModeNew;
                    case MailsFilterDialogResultStatus.RefineCurrentSearch:
                        return TranslationViewObject.MailsFilterSearchModeRefine;
                    case MailsFilterDialogResultStatus.UniteResults:
                        return TranslationViewObject.MailsFilterSearchModeUnion;
                    default:
                        return "Undefined";
                }
            }
        }
    }

    public class MailsFilterViewModel: PropertyChangedModel
    {
        public MailsFilterDialogResult Result;

        private readonly bool _bigCitiesFileExists;

        public Window ThisWindow;
        public ICommand CancelCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand FindNearestBigCityCommand { get; set; }

        public IMessageBoxProvider MessageBoxProvider = new WinFormsMessageBoxProvider();

        public TranslationViewObject TranslationViewObject
        {
            get { return TranslationViewObject.LoadFromSettings(); }
        }

        public List<FilterModeComboboxItem> FilterModeComboboxItems
        {
            get
            {
                var availableStatuses = new[]
                    {
                        MailsFilterDialogResultStatus.NewSearch,
                        MailsFilterDialogResultStatus.RefineCurrentSearch,
                        MailsFilterDialogResultStatus.UniteResults
                    };

                return availableStatuses.Select(status => new FilterModeComboboxItem {Status = status, TranslationViewObject = TranslationViewObject}).ToList();

            }
        }

        private string _nameFilterValue;
        private bool _isNameFilterEnabled;

        private string _additionalNotesFilterValue;
        private bool _isAdditionalNotesFilterEnabled;

        private string _inlineMessageFilterValue;
        private bool _isInlineMessageFilterEnabled;

        private string _hashsumValue;
        private bool _isAttachmentFilterEnabled;

        private string _rawCoordinatesFilterValue;
        private int _radiusSliderValue = 591;
        private bool _isLocationFilterEnabled;

        private DateTime _dateTimeFromFilterValue = DateTimeGenerator.UtcNow() - TimeSpan.FromDays(30);
        private bool _isDateTimeFromFilterEnabled = true;

        private DateTime _dateTimeToFilterValue = DateTimeGenerator.UtcNow() + TimeSpan.FromDays(3);
        private bool _isDateTimeToFilterEnabled = true;

        private string _rawPopulationValue;
        private bool _isDampingDistanceFilterEnabled;

        private string _rawMaxAttachmentSizeValue;
        private bool _isAttachmentSizeFilterEnabled;

        private FilterModeComboboxItem _selectedFilterModeComboboxItem;
        public FilterModeComboboxItem SelectedFilterModeComboboxItem
        {
            get
            {
                return _selectedFilterModeComboboxItem ??
                       FilterModeComboboxItems.First(item => item.Status == MailsFilterDialogResultStatus.NewSearch);
            }
            set
            {
                _selectedFilterModeComboboxItem = value;
                OnPropertyChanged("SelectedFilterModeComboboxItem");
            }
        }

        public string NameFilterValue
        {
            get { return _nameFilterValue; }
            set
            {
                _nameFilterValue = value;
                OnPropertyChanged("NameFilterValue");
            }
        }

        public bool IsNameFilterEnabled
        {
            get { return _isNameFilterEnabled; }
            set
            {
                _isNameFilterEnabled = value;
                OnPropertyChanged("IsNameFilterEnabled");
            }
        }

        public string AdditionalNotesFilterValue
        {
            get { return _additionalNotesFilterValue; }
            set
            {
                _additionalNotesFilterValue = value;
                OnPropertyChanged("AdditionalNotesFilterValue");
            }
        }

        public bool IsAdditionalNotesFilterEnabled
        {
            get { return _isAdditionalNotesFilterEnabled; }
            set
            {
                _isAdditionalNotesFilterEnabled = value;
                OnPropertyChanged("IsAdditionalNotesFilterEnabled");
            }
        }

        public string InlineMessageFilterValue
        {
            get { return _inlineMessageFilterValue; }
            set
            {
                _inlineMessageFilterValue = value;
                OnPropertyChanged("InlineMessageFilterValue");
            }
        }

        public bool IsInlineMessageFilterEnabled
        {
            get { return _isInlineMessageFilterEnabled; }
            set
            {
                _isInlineMessageFilterEnabled = value;
                OnPropertyChanged("IsInlineMessageFilterEnabled");
            }
        }

        public string HashsumValue
        {
            get { return _hashsumValue; }
            set
            {
                _hashsumValue = value;
                OnPropertyChanged("HashsumValue");
            }
        }

        public bool IsAttachmentFilterEnabled
        {
            get { return _isAttachmentFilterEnabled; }
            set
            {
                _isAttachmentFilterEnabled = value;
                OnPropertyChanged("IsAttachmentFilterEnabled");
            }
        }

        public int RadiusSliderMinimum { get { return 1; } }
        public int RadiusSliderMaximum { get { return 1000; } }

        public double RadiusMetersValue
        {
            get
            {
                const double a1 = 10.0;
                // left slide - 10 meters, right - 10000 km (Geometric progression)
                var q = Math.Pow(1000000.0, 1.0/(RadiusSliderMaximum - RadiusSliderMinimum));
                return a1*Math.Pow(q, RadiusSliderValue - 1);
            }
        }

        public int RadiusSliderValue
        {
            get { return _radiusSliderValue; }
            set
            {
                _radiusSliderValue = value;
                OnPropertyChanged("RadiusSliderValue");
                OnPropertyChanged("RadiusMetersValue");
            }
        }

        public string RawCoordinatesFilterValue
        {
            get { return _rawCoordinatesFilterValue; }
            set
            {
                _rawCoordinatesFilterValue = value;
                OnPropertyChanged("RawCoordinatesFilterValue");
                OnPropertyChanged("IsValidCoordinatesFilterValue");
            }
        }

        public bool IsLocationFilterEnabled
        {
            get { return _isLocationFilterEnabled; }
            set
            {
                _isLocationFilterEnabled = value;
                if (!value && IsDampingDistanceFilterEnabled)
                {
                    IsDampingDistanceFilterEnabled = false;
                }

                OnPropertyChanged("IsLocationFilterEnabled");
                OnPropertyChanged("IsValidCoordinatesFilterValue");
            }
        }

        public bool IsValidCoordinatesFilterValue
        {
            get
            {
                if (IsLocationFilterEnabled)
                {
                    return Coordinates.FromString(RawCoordinatesFilterValue) != null;
                }
                return true;
            }
        }

        public DateTime DateTimeFromFilterValue
        {
            get { return _dateTimeFromFilterValue; }
            set
            {
                _dateTimeFromFilterValue = value;
                OnPropertyChanged("DateTimeFromFilterValue");
            }
        }

        public bool IsDateTimeFromFilterEnabled
        {
            get { return _isDateTimeFromFilterEnabled; }
            set
            {
                _isDateTimeFromFilterEnabled = value;
                OnPropertyChanged("IsDateTimeFromFilterEnabled");
            }
        }

        public DateTime DateTimeToFilterValue
        {
            get { return _dateTimeToFilterValue; }
            set
            {
                _dateTimeToFilterValue = value;
                OnPropertyChanged("DateTimeToFilterValue");
            }
        }

        public bool IsDateTimeToFilterEnabled
        {
            get { return _isDateTimeToFilterEnabled; }
            set
            {
                _isDateTimeToFilterEnabled = value;
                OnPropertyChanged("IsDateTimeToFilterEnabled");
            }
        }

        public string RawPopulationValue
        {
            get { return _rawPopulationValue; }
            set
            {
                _rawPopulationValue = value;
                OnPropertyChanged("RawPopulationValue");
                OnPropertyChanged("IsValidPopulationValue");
            }
        }

        public bool IsDampingDistanceFilterEnabled
        {
            get { return _isDampingDistanceFilterEnabled; }
            set
            {
                _isDampingDistanceFilterEnabled = value;
                if (value && !IsLocationFilterEnabled)
                {
                    IsLocationFilterEnabled = true;
                }

                OnPropertyChanged("IsDampingDistanceFilterEnabled");
                OnPropertyChanged("IsValidPopulationValue");
            }
        }

        public bool IsValidPopulationValue
        {
            get
            {
                if (!IsDampingDistanceFilterEnabled)
                {
                    return true;
                }

                int value;
                var isValidInt = int.TryParse(RawPopulationValue, out value);

                return isValidInt;
            }
        }

        public string RawMaxAttachmentSizeValue
        {
            get { return _rawMaxAttachmentSizeValue; }
            set
            {
                _rawMaxAttachmentSizeValue = value;
                OnPropertyChanged("RawMaxAttachmentSizeValue");
                OnPropertyChanged("IsValidMaxAttachmentSizeValue");
            }
        }

        public bool IsAttachmentSizeFilterEnabled
        {
            get { return _isAttachmentSizeFilterEnabled; }
            set
            {
                _isAttachmentSizeFilterEnabled = value;

                OnPropertyChanged("IsAttachmentSizeFilterEnabled");
                OnPropertyChanged("IsValidMaxAttachmentSizeValue");
            }
        }

        public bool IsValidMaxAttachmentSizeValue
        {
            get
            {
                if (!IsAttachmentSizeFilterEnabled)
                {
                    return true;
                }

                ulong value;
                var isValidUlong = ulong.TryParse(RawMaxAttachmentSizeValue, out value);

                return isValidUlong;
            }
        }

        private List<IMailParamFilter> _getFilters()
        {
            var filters = new List<IMailParamFilter>();
            
            if (IsNameFilterEnabled && !String.IsNullOrWhiteSpace(NameFilterValue))
            {
                filters.Add(new NameFilter(NameFilterValue));
            }

            if (IsAdditionalNotesFilterEnabled && !String.IsNullOrWhiteSpace(AdditionalNotesFilterValue))
            {
                filters.Add(new AdditionalNotesFilter(AdditionalNotesFilterValue));
            }

            if (IsInlineMessageFilterEnabled && !String.IsNullOrWhiteSpace(InlineMessageFilterValue))
            {
                filters.Add(new InlineMessageFilter(InlineMessageFilterValue));
            }

            if (IsAttachmentFilterEnabled && !String.IsNullOrWhiteSpace(HashsumValue))
            {
                filters.Add(new AttachmentFilter(HashsumValue));
            }

            if (IsLocationFilterEnabled && IsValidCoordinatesFilterValue)
            {
                filters.Add(new LocationFilter(Coordinates.FromString(RawCoordinatesFilterValue),
                                               RadiusMetersValue));
            }

            DateTime? dateTimeFrom = null;
            if (IsDateTimeFromFilterEnabled)
            {
                dateTimeFrom = DateTimeFromFilterValue;
            }

            DateTime? dateTimeTo = null;
            if (IsDateTimeToFilterEnabled)
            {
                dateTimeTo = DateTimeToFilterValue;
            }

            if (IsDateTimeFromFilterEnabled || IsDateTimeToFilterEnabled)
            {
                filters.Add(new DateTimeCreatedRangeFilter(dateTimeFrom, dateTimeTo));
            }

            if (IsDampingDistanceFilterEnabled && IsValidPopulationValue)
            {
                var population = ulong.Parse(RawPopulationValue);
                filters.Add(new DampingDistanceFilter(Coordinates.FromString(RawCoordinatesFilterValue),
                                                      population: population));
            }

            if (IsAttachmentSizeFilterEnabled && IsValidMaxAttachmentSizeValue)
            {
                var maxAttachmentSize = ulong.Parse(RawMaxAttachmentSizeValue);
                filters.Add(new AttachmentFileSizeFilter(maxAttachmentSize));
            }

            return filters;
        }

        private bool _canGetFilters(object o = null)
        {
            if (IsLocationFilterEnabled && !IsValidCoordinatesFilterValue)
            {
                return false;
            }

            if (IsDampingDistanceFilterEnabled && !IsValidPopulationValue)
            {
                return false;
            }

            if (IsAttachmentSizeFilterEnabled && !IsValidMaxAttachmentSizeValue)
            {
                return false;
            }

            return true;
        }

        private void _getFiltersCommand(object o = null)
        {
            var filters = _getFilters();
            Result = new MailsFilterDialogResult(SelectedFilterModeComboboxItem.Status, filters);

            if (ThisWindow != null)
            {
                ThisWindow.Close();
            }
        }

        private bool _canFindNearestBigCity(object o = null)
        {
            return _bigCitiesFileExists && IsValidCoordinatesFilterValue;
        }

        private void _findNearestBigCity(object o = null)
        {
            const double rangeDistance = 50000.0;
            var foundCity = false;
            var resultedCity = new WorldCitiesRow();

            foreach (var worldCitiesRow in WorldCitiesParser.GetRows())
            {
                if ((DistanceCalculator.Distance(Coordinates.FromString(RawCoordinatesFilterValue),
                                                     new Coordinates(worldCitiesRow.Latitude, worldCitiesRow.Longitude)) < rangeDistance)
                       && (!foundCity || worldCitiesRow.Population > resultedCity.Population))
                {
                    foundCity = true;
                    resultedCity = worldCitiesRow;
                }
            }

            if (foundCity)
            {
                RawPopulationValue = resultedCity.Population.ToString(CultureInfo.InvariantCulture);
                MessageBoxProvider.Show(
                    String.Format(TranslationViewObject.MailsFilterFoundCityTemplate, resultedCity.City, resultedCity.Country,
                                  resultedCity.Population), TranslationViewObject.OverviewMessageBoxNotificationCaption);
            }
            else
            {
                MessageBoxProvider.Show(TranslationViewObject.MailsFilterNotFoundCityMessage, TranslationViewObject.OverviewMessageBoxNotificationCaption);
            }
        }

        public MailsFilterViewModel()
        {
            CancelCommand = new CommandHandler(_ =>
                {
                    Result = new MailsFilterDialogResult(MailsFilterDialogResultStatus.CancelClicked, null);
                    ThisWindow.Close();
                }, _ => true);

            SearchCommand = new CommandHandler(_getFiltersCommand, _canGetFilters);
            FindNearestBigCityCommand = new CommandHandler(_findNearestBigCity, _canFindNearestBigCity);

            _bigCitiesFileExists = File.Exists(WorldCitiesParser.GetCsvFilePath());
        }
    }
}
