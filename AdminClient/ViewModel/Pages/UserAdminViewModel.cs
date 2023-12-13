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
using System.Diagnostics;
using AdminClient.Services;

namespace AdminClient.ViewModel.Pages
{
    public class UserAdminViewModel : INotifyPropertyChanged
    {
        private string _currentEmail;
        private string _newEmail;
        public ICommand ChangeEmailCommand { get; }

        public string BaseURI = "http://51.20.73.95:5000/api";


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
            try
            {
                var authService = AuthenticationService.Instance;
                var requestBody = new
                {
                    currentEmail = CurrentEmail,
                    newEmail = NewEmail
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await authService._httpClient.PutAsync(BaseURI + "/user/email", content);
                if (response.StatusCode == HttpStatusCode.OK) 
                {
                    Debug.WriteLine("Email was successfully changed!");
                    response.EnsureSuccessStatusCode(); 
                }
                else
                {
                    Debug.WriteLine(response);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}