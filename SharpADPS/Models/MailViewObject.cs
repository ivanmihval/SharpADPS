using System;
using System.Globalization;
using System.Linq;
using CoreADPS.Helpers;
using CoreADPS.MailModels;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.Models.Translation;

namespace WPFSharpADPS.Models
{
    public class MailViewObject : PropertyChangedModel
    {
        private const ulong OneKiBBytes = 1024;
        private const ulong OneMiBBytes = OneKiBBytes*1024;
        private const ulong OneGiBBytes = OneMiBBytes*1024;

        private readonly Mail _mail;
        private readonly TranslationViewObject _translation;

        public readonly string MsgPath;

        public string DateCreated
        {
            get
            {
                if (_mail.DateCreated.Date <= DateTime.Today)
                {
                    var daysPassed = (int) Math.Floor((DateTimeGenerator.UtcNow() - _mail.DateCreated).TotalDays);
                    return String.Format(_translation.OverviewDataGridDaysAgoTemplate, daysPassed);
                }
                return _mail.DateCreated.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        public string RecipientsCoordinates
        {
            get
            {
                //return String.Join("; ",
                //                   _mail.RecipientsCoordinates.Select(
                //                       coordinates =>
                //                       String.Format("{0:f4},{1:f4}", coordinates.Latitude, coordinates.Longitude)));

                switch (_mail.RecipientsCoordinates.Count)
                {
                    case 0:
                        return _translation.OverviewDataGridNoCoordinates;
                    case 1:
                        return
                            _mail.RecipientsCoordinates.Select(
                                c => String.Format("{0:f4}, {1:f4}", c.Latitude, c.Longitude)).First();
                }

                return String.Format(_translation.OverviewDataGridManyCoordinatesTemplate, _mail.RecipientsCoordinates.Count);
            }
        }


        public string Name
        {
            get { return _mail.Name; }
        }

        public string AdditionalNotes
        {
            get { return _mail.AdditionalNotes; }
        }

        public string InlineMessage
        {
            get { return _mail.InlineMessage; }
        }

        public string Attachments
        {
            get
            {
                var count = 0;
                UInt64 sumSizeBytes = 0;
                foreach (var attachment in _mail.Attachments)
                {
                    count += 1;
                    sumSizeBytes += attachment.SizeBytes;
                }

                if (count == 0)
                {
                    return _translation.OverviewDataGridNoFiles;
                }

                var memoryUnit = "B";
                double number = sumSizeBytes;
                if (sumSizeBytes >= OneKiBBytes && sumSizeBytes < OneMiBBytes)
                {
                    memoryUnit = "KiB";
                    number /= OneKiBBytes;
                }
                else if (sumSizeBytes >= OneMiBBytes && sumSizeBytes < OneGiBBytes)
                {
                    memoryUnit = "MiB";
                    number /= OneMiBBytes;
                }
                else if (sumSizeBytes >= OneGiBBytes)
                {
                    memoryUnit = "GiB";
                    number /= OneGiBBytes;
                }

                return String.Format("{0}, {1:#.#} {2}", String.Format(_translation.OverviewDataGridManyFilesTemplate, count), number, memoryUnit);
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            { 
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public MailViewObject(Mail mail, string msgPath, TranslationViewObject translation)
        {
            _mail = mail;
            MsgPath = msgPath;
            _translation = translation;
        }
    }
}
