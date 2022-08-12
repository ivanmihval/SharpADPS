using System.ComponentModel;
using System.Diagnostics;

namespace WPFSharpADPS.Helpers
{
    public abstract class PropertyChangedModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // http://www.journeyintocode.com/2013/04/callermembername-net-40.html
        public void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == null)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(1);
                var method = frame.GetMethod();
                propertyName = method.Name.Replace("set_", "");

            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
