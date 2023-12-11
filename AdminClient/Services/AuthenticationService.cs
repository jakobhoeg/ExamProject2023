using AdminClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.Services
{
    public class AuthenticationService
    {
        private HttpClient _httpClient;
        public User User { get; set; }


        public AuthenticationService()
        {
            var cookieContainer = new System.Net.CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("http://51.20.138.229:5000/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<User> LoginAsync(string jsonData)
        {
            try
            {
                // Make the POST request to the login endpoint
                HttpResponseMessage response = await _httpClient.PostAsync("login", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Store the received cookies
                    var cookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault();

                    string responseContent = await response.Content.ReadAsStringAsync();

                    User = JsonConvert.DeserializeObject<User>(responseContent);

                    return User;
                }
                else
                {
                    // Unsuccessful login
                    Debug.WriteLine($"Login failed. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
