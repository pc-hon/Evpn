using Evpn.Models;
using Evpn.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Evpn.ViewModels
{
    public class ApplicationViewModel : NotifyPropertyChanged
    {
        public const string DEFAULT_SERVER_URL = @"https://xvjune2014trial.apiary.io/locations";

        public const string ERROR_TEXT_UNKNOWN = "An unknown error happened";
        public const string ERROR_TEXT_TIMED_OUT = "The request timed out";
        public const string ERROR_TEXT_403 = "The API returned error code 403";

        public ApplicationViewModel()
        {
            Locations = new ObservableCollection<LocationViewModel>();
            BestLocation = new BestLocationViewModel();
            RefreshText = "Refresh";
            RestClient = new RestClient();
            ServerUrl = DEFAULT_SERVER_URL;
        }

        public ObservableCollection<LocationViewModel> Locations { get; private set; }
        public BestLocationViewModel BestLocation { get; set; }
        
        public virtual IRestClient RestClient { get; private set; } 
        public string ServerUrl { get; set; }

        private ExpressVpnResponse m_lastResponse;

        private string m_refreshText;
        public string RefreshText
        {
            get { return m_refreshText; }
            set
            {
                m_refreshText = value;
                OnPropertyChanged("RefreshText");
            }
        } 

        public void RefreshLocations()
        {
            BeginGetLocations();
        }

        #region MessageBox events

        public delegate void MessageBoxHandler(string message);
        private event MessageBoxHandler m_message;

        public void AttachMessageBoxEvent(MessageBoxHandler handler)
        {
            m_message += handler;
        }

        // Making this virtual so I can mock MessageBox.Show using JustMock Lite (Paid version can can avoid this)
        public virtual void ShowMessageBox(string text)
        {
            if (m_message != null)
                m_message(text);
        }
        #endregion


        #region Get Locations

        private void BeginGetLocations()
        {
            var getLocationTask = Task.Run(() => RestClient.GetLocations(ServerUrl));
            
            // Execute the UI updates at Current UI thread
            getLocationTask.ContinueWith(ProcessGetLocations, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ProcessGetLocations(Task<RestResult<ExpressVpnResponse>> completedTask)
        {
            if (completedTask.Result == null)
            {
                ShowMessageBox(ERROR_TEXT_UNKNOWN);
                return;
            }

            if (!completedTask.Result.IsSuccess)
            {
                if (completedTask.Result.IsTimeout)
                {
                    ShowMessageBox(ERROR_TEXT_TIMED_OUT);
                }
                else if (completedTask.Result.IsForbidden)
                {
                    ShowMessageBox(ERROR_TEXT_403);
                }
                else
                {
                    ShowMessageBox(ERROR_TEXT_UNKNOWN);
                }
                return;
            }

            if (completedTask.Result.Value == null)
            {
                ShowMessageBox(ERROR_TEXT_UNKNOWN);
                return;
            }

            m_lastResponse = completedTask.Result.Value;

            RefreshText = m_lastResponse.RefreshText;

            Locations.Clear();
            m_lastResponse.Locations
                .ConvertAll(loc => new LocationViewModel(loc))
                .ForEach(Locations.Add);

            BeginCalculateBestLocationAsync();
        }
        #endregion


        #region BestLocation Calculation

        private CancellationTokenSource m_cancellationTokenSource;
        private long m_bestLocationTime;
        private object m_lock = new object();

        // Making this virtual so I can mock this using JustMock Lite
        public virtual void BeginCalculateBestLocationAsync()
        {
            if (m_cancellationTokenSource != null)
                m_cancellationTokenSource.Cancel(); // cancel last task
            
            m_cancellationTokenSource = new CancellationTokenSource();
            var token = m_cancellationTokenSource.Token;

            Task.Run(() => CalculateBestLocation(token), token);
        }

        private void CalculateBestLocation(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            m_bestLocationTime = long.MaxValue;
            BestLocation.SetNoResult();

            // Run all Location.Ping in parallel
            m_lastResponse.Locations.AsParallel().ForAll(loc =>
                {
                    var time = loc.PingServers();
                    token.ThrowIfCancellationRequested();

                    lock (m_lock)
                    {
                        if (time < m_bestLocationTime)
                        {
                            m_bestLocationTime = time;
                            BestLocation.SetRunning(loc);
                        }
                    }
                });

            token.ThrowIfCancellationRequested();

            BestLocation.SetCompleted();
        }
        #endregion
    }
}
