using BackendAPIMongo.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NUnitTesting
{
    public class Tests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Dispose of resources created for the tests.
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Login_ValidUser_ReturnsOK()
        {
            // Arrange
            var user = new User
            {
                Email = "jakobfsd@hotmail.dk",
                Password = "1234"
            };

            try
            {
                // Act
                var response = await _client.PostAsJsonAsync("/api/login", user);

                // Assert
                response.EnsureSuccessStatusCode();
            } catch  (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
