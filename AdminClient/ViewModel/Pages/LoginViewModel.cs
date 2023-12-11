using AdminClient.Command;
using AdminClient.Models;
using AdminClient.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AdminClient.ViewModel.Pages
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email))); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password))); }
        }


        private RelayCommand _loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin));
            }
        }

        private bool CanLogin()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }

            if (!Email.Contains("@"))
            {
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task<bool> LoginAsync()
        {
            var loginData = new
            {
                Email = Email,
                Password = _password
            };

            var authService = AuthenticationService.Instance;
            User loggedInUser = await authService.LoginAsync(JsonConvert.SerializeObject(loginData));

            if (loggedInUser != null)
            {
                OnSuccessfulLogin();

                Debug.WriteLine($"Login successful. Welcome, {loggedInUser.FirstName}!");
                return true;
            }
            else
            {
                Debug.WriteLine("Login failed.");
                return false;
            }
        }

        public event EventHandler<EventArgs> SuccessfulLogin;

        protected virtual void OnSuccessfulLogin()
        {
            SuccessfulLogin?.Invoke(this, EventArgs.Empty);
        }

    }
}
