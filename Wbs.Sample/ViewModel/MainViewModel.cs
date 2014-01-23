using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using Wbs.Sample.Model;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Wbs.Sample.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private WBS.Net.WBS wbs;
    

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.wbs = new WBS.Net.WBS(Properties.Settings.Default.ApiKey, Properties.Settings.Default.ApiSecret);
            this.Metrics = new ObservableCollection<Metric>();

            if (IsInDesignMode)
            {
                Random rnd = new Random(DateTime.Now.Millisecond);

                for (int i = 0; i < 10; i++)
                {
                    int value = rnd.Next(50, 180);
                    this.Metrics.Add(new Metric() { Date = DateTime.Now, Name = "Heart Rate:", Value = value.ToString() });
                }
            }
            else
            {
                // Code runs "for real"
            }
        }

        public ObservableCollection<Metric> Metrics
        {
            get;
            private set;
        }

        public bool IsAuthorized
        {
            get
            {
                return wbs.IsValid;
            }
        }

        public async Task<string> Authenticate()
        {
            return await wbs.Connect();
        }

        public bool Authorize(Uri uri)
        {
            if (uri.OriginalString.Contains("oauth_verifier"))
            {
                var data = uri.OriginalString.Split(new char[] { '=', '&' });
                var verifier = data[5];
                var userId = data[1];

               wbs.Valididate(verifier, int.Parse(userId));
               return true;
            }

            return false;
        }

        public async void UpdateMeasures()
        {
            var list = await this.wbs.GetMeasures(DateTime.Now.AddYears(-1), DateTime.Now);

            var heartData = from x in list
                            from m in x.Measures
                            where m.MeasureType == WBS.Net.MeasureType.HeartPulse
                            select new { Date = x.Date, Pulse = m.Value };

            foreach(var metric in heartData)
            {
                this.Metrics.Add(
                    new Metric()
                    {
                        Name = "Heart Rate",
                        Value = metric.Pulse.ToString(),
                        Date = metric.Date
                    });
            }
        }
    }
}