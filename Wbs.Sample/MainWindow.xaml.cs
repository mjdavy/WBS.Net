using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wbs.Sample.ViewModel;

namespace Wbs.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            this.vm = this.DataContext as MainViewModel;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // already logged-in so we can directly get Measures
            if (vm.IsAuthorized)
            {
                vm.UpdateMeasures();
            }
            else
            {
                // if we don't have token, tokenSecret and userId, we need to get an oauth token
                this.MetricsList.Visibility = Visibility.Collapsed;
                this.WebBrowser.Visibility = Visibility.Visible;
                string url = await vm.Authenticate();
                this.WebBrowser.Navigate(url);
            }
        }

        // MJD Jan 22, 2014
        // Note - you may find that the allow buttons don't show up in the Withings page. 
        // This is because of IE compatibility defaulting in the WebBrowser control. You need to add a registry key 
        // for your application to force a later version of IE. I found IE 9 compatitibility works (value 9999)
        // On windows 8.1 I needed to add it to HKEY_CURRENT_USER. When I added it to HKEY_LOCAL_MACHINE is did not work.
        // More detail about this can be found here: http://msdn.microsoft.com/en-us/library/ee330730%28VS.85%29.aspx#browser_emulation 

        private async void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (vm.Authorize(e.Uri))
            {
                this.MetricsList.Visibility = Visibility.Visible;
                this.WebBrowser.Visibility = Visibility.Collapsed;
                vm.UpdateMeasures();
            }
        }
    }
}
