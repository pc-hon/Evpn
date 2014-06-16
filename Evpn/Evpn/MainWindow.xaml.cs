using Evpn.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace Evpn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ViewModel.AttachMessageBoxEvent(s => MessageBox.Show(s));

            ViewModel.RefreshLocations();
        }

        protected ApplicationViewModel ViewModel
        {
            get
            {
                return DataContext as ApplicationViewModel;
            }
        }

        private void OnClickRefreshButton(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshLocations();
        }

        private void OnClickConnectButton(object sender, RoutedEventArgs e)
        {
            BestLocationPopup.IsOpen = true;
        }

        private void OnClickBestLocationPopup(object sender, MouseEventArgs e)
        {
            BestLocationPopup.IsOpen = false;
        }
    }
}
