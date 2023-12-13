using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using AdminClient.Command;
using Newtonsoft.Json;
using Wpf.Ui.Input;

namespace AdminClient.ViewModel.Pages
{
    public class UserAdminViewModel : INotifyPropertyChanged
    {
        private string _currentEmail;
        private string _newEmail;
        public ICommand ChangeEmailCommand { get; }


        public string CurrentEmail
        {
            get { return _currentEmail; }
            set
            {
                _currentEmail = value;
                OnPropertyChanged(nameof(CurrentEmail));
            }
        }

        public string NewEmail
        {
            get { return _newEmail; }
            set
            {
                _newEmail = value;
                OnPropertyChanged(nameof(NewEmail));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UserAdminViewModel()
        {
            ChangeEmailCommand = new RelayCommand<object>(ChangeEmail);
        }

        private async void ChangeEmail(object parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    CurrentEmail = CurrentEmail,
                    NewEmail = NewEmail
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("/api/user/email", content);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}