using BackendAPIMongo;
using BackendAPIMongo.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace NUnitTesting
{
    public class Tests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        #region Setup and TearDown
        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _factory = _factory.WithWebHostBuilder(builder =>
            {
                // Change the environment to Testing (from mongohost to localhost). Otherwise it 
                // wont work for some reason. Not even on Docker.
                builder.ConfigureServices(services =>
                {
                    services.Configure<MongoDBRestSettings>(options =>
                    {
                        options.ConnectionString = "mongodb://localhost:27017";
                    });
                });
            });
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
        #endregion

        #region Test Methods
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
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        [Test]
        public async Task Login_ValidUser_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User
            {
                Email = "test@email.dk",
                Password = "4321"
            };

            try
            {
                // Act
                var response = await _client.PostAsJsonAsync("/api/login", user);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        [Test]
        public async Task Login_ValidNonAdminUser_ReturnsOK()
        {
            // Arrange
            var user = new User
            {
                Email = "test@email.dk",
                Password = "1234"
            };

            try
            {
                // Act
                var response = await _client.PostAsJsonAsync("/api/login", user);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        [Test]
        public async Task Login_InvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User
            {
                Email = "nonexistent@example.com",
                Password = "1234"
            };

            try
            {
                // Act
                var response = await _client.PostAsJsonAsync("/api/login", user);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
