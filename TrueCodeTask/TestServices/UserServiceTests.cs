using Microsoft.AspNetCore.Mvc;
using SharedModels;
using UserService.Controllers;
using UserService.Managers;

namespace TestServices
{
    public class UserServiceTests
    {
        private readonly TrueCodeContext _dbContext;
        private readonly UserController _userController;

        public UserServiceTests() 
        {
            _dbContext = TestConfig.GetDbContext();

            var jwtOptions = TestConfig.GetJwtOptions();
            var tokenManager = new TokenManager(jwtOptions);
            _userController = new UserController(_dbContext, tokenManager);
        }


        [Fact]
        public async Task RegisterTestNewUser()
        {
            var result = await _userController.Register(new("testUser", "password123"));

            Assert.NotNull(result);
            Assert.Equal(1, _dbContext.Users.Count());
            Assert.Equal("testUser", _dbContext.Users.First().Name);
        }

        [Fact]
        public async Task RegisterTestOldUser()
        {
            await _userController.Register(new("testUser", "12345"));

            var result = await _userController.Register(new("testUser", "123456"));

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Пользователь с таким именем уже существует", badRequest.Value as string);
        }

        [Fact]
        public async Task LoginTest()
        {
            await _userController.Register(new("testUser", "password123"));
            var loginResult = await _userController.Login(new("testUser", "password123"));

            Assert.NotNull(loginResult);

            var okResult = Assert.IsType<OkObjectResult>(loginResult);

            var type = okResult.Value.GetType();
            var prop = type.GetProperty("token");
            Assert.NotNull(prop?.GetValue(okResult.Value));
        }

        [Fact]
        public async Task LoginTestUserDoesntExist()
        {
            var result = await _userController.Login(new("newUser", "12345"));

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task LogintestInvalidPass()
        {
            await _userController.Register(new("testUser", "rightpass"));
            var result = await _userController.Login(new("testUser", "badpass"));

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Неверный логин или пароль", unauthorized.Value as string);
        }
    }
}