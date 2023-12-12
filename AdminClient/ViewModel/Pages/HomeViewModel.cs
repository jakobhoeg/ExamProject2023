using AdminClient.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdminClient.ViewModel.Pages
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _dailyCount;
        private string _weeklyCount;
        private string _monthlyCount;
        private string _yearlyCount;


        public string DailyCount
        {
            get => _dailyCount;
            set
            {
                _dailyCount = value;
                OnPropertyChanged(nameof(DailyCount));
            }
        }

        public string WeeklyCount
        {
            get => _weeklyCount;
            set
            {
                _weeklyCount = value;
                OnPropertyChanged(nameof(WeeklyCount));
            }
        }

        public string MonthlyCount
        {
            get => _monthlyCount;
            set
            {
                _monthlyCount = value;
                OnPropertyChanged(nameof(MonthlyCount));
            }
        }

        public string YearlyCount
        {
            get => _yearlyCount;
            set
            {
                _yearlyCount = value;
                OnPropertyChanged(nameof(YearlyCount));
            }
        }

        public async Task UpdateStatistics()
        {
            DailyCount = await MakeGetRequest("statistics/users/count/daily");
            WeeklyCount = await MakeGetRequest("statistics/users/count/weekly");
            MonthlyCount = await MakeGetRequest("statistics/users/count/monthly");
            YearlyCount = await MakeGetRequest("statistics/users/count/yearly");
        }

        private async Task<string> MakeGetRequest(string endpoint)
        {
            var authService = AuthenticationService.Instance;

            var response = await authService._httpClient.GetAsync("http://16.170.143.117:5000/api/" + endpoint);
            Debug.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                var count = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(count);
                return count;
            }
            else
            {
                return "N/A";
            }

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HomeViewModel()
        {
            UpdateStatistics();
        }
    }
}