using CoreADPS.MailModels;
using WPFSharpADPS.Helpers;

namespace WPFSharpADPS.Models
{
    public class AttachmentViewObject : PropertyChangedModel
    {
        private readonly Attachment _attachment;
        public readonly string Path;

        private bool _isSelected;

        public Attachment Attachment
        {
            get { return _attachment; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public AttachmentViewObject(Attachment attachment, string path = null)
        {
            _attachment = attachment;
            Path = path;
        }
    }
}
