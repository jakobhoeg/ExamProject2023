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
        private static readonly AuthenticationService instance = new AuthenticationService();

        public static AuthenticationService Instance => instance;

        public HttpClient _httpClient { get; }
        public User User { get; set; }
        public string AccessToken { get; set; }


        private AuthenticationService()
        {
            var cookieContainer = new System.Net.CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("http://16.170.143.117:5000/api/");
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
                    // Get the access token from the response cookie
                    var cookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault();

                    // Get body from response and deserialize it to a User object
                    string responseContent = await response.Content.ReadAsStringAsync();
                    User = JsonConvert.DeserializeObject<User>(responseContent);

                    if (User.IsAdmin == false)
                    {
                        Debug.WriteLine("User is not an admin");
                        return null;
                    }

                    AccessToken = cookie;

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
