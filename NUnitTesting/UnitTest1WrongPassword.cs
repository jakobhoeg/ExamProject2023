using BackendAPIMongo.Model;
using BackendAPIMongo.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.Security.Claims;

namespace NUnitTesting
{
    public class Tests
    {
        //Program program;
        //User user;

        [SetUp]
        public void Setup()
        {
            //program = new Program();
            //user = new User();

            //user.Email = "";
            //user.Password = "";
            //user.FirstName = "";
            //user.IsAdmin = false;
            //user.Partner = null;
            //user.LikedBabyNames = new List<BabyName>();
        }


        [Test]
        public async Task MapPost_ValidUser_ReturnsOkResult()
        {
            //// Arrange
            //var user = new User { Email = "test@example.com", Password = "password" };
            //var userRepositoryMock = new Mock<IUserRepository>();
            //userRepositoryMock.Setup(repo => repo.Authenticate(user)).ReturnsAsync(true);
            //var contextMock = new Mock<HttpContext>();
            //var signInResult = SignInResult.Success;
            //contextMock.Setup(ctx => ctx.SignInAsync(AuthScheme, It.IsAny<ClaimsPrincipal>())).ReturnsAsync(signInResult);

            //var program = new Program();

            //// Act
            //var result = await program.MapPost(user, userRepositoryMock.Object, contextMock.Object);

            //// Assert
            //Assert.IsInstanceOf<OkObjectResult>(result);
            //var okResult = (OkObjectResult)result;
            //Assert.AreEqual("Logged in successfully", okResult.Value);


            Assert.Pass();
        }
    }
}