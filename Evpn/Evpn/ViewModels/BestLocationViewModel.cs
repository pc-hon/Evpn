using Evpn.Models;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Evpn.ViewModels
{
    public class BestLocationViewModel : NotifyPropertyChanged
    {
        public const string COMPLETED = "According to speed tests I've been running in the background, the best location for you appears to be:";
        public const string RUNNING = "The speed tests is still running in the background, currently part of the tests report the best location for now is:";
        public const string NO_RESULT = "The speed tests is still running in the background, no results has been obtained yet.";

        protected Location Location { get; set; }

        public string Text { get; private set; }

        public string Name { get { return Location != null ? Location.Name : string.Empty;  } }

        public BitmapSource ImageSource { get { return Location != null ? Location.Icon.BitmapSource : null; } }

        public BestLocationViewModel()
        {
            SetNoResult();
        }

        public virtual void SetNoResult()
        {
            Set(null, NO_RESULT);
        }

        public virtual void SetRunning(Location location)
        {
            Set(location, RUNNING);
        }

        public virtual void SetCompleted()
        {
            Set(Location, COMPLETED);
        }

        private void Set(Location location, string text)
        {
            Location = location;
            Text = text;
            OnPropertyChanged("Text");
            OnPropertyChanged("Name");
            OnPropertyChanged("ImageSource");
        }
    }
}
