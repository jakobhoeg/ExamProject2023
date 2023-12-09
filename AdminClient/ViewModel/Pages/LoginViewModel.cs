using AdminClient.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

            // Convert loginData to JSON
            string jsonData = JsonConvert.SerializeObject(loginData);

            // Set up HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Set the base URL for the API
                client.BaseAddress = new Uri("http://localhost:5000/");

                // Set the content type to JSON
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                
                // Create the request body as StringContent
                var body = new StringContent(jsonData, Encoding.UTF8, "application/json");

                try
                {
                    // Make the POST request to the login endpoint
                    HttpResponseMessage response = await client.PostAsync("login", body);
                    Debug.WriteLine(response);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        

                        string responseContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(responseContent);
                        return true; // Login successful
                    }
                    else
                    {
                        // Unsuccessful login logic here
                        // For example, you might display an error message based on the response status code
                        Debug.WriteLine($"Login failed. Status code: {response.StatusCode}");
                        return false; // Login failed
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception (e.g., network error)
                    Debug.WriteLine($"Error: {ex.Message}");
                    return false; // Login failed
                }
            }
        }

        
    }
}
