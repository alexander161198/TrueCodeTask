using FinanceService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using SharedModels.EntityModels;
using System.Security.Claims;

namespace TestServices
{
    public class FinanceServiceTests
    {
        private FinanceController GetControllerWithContext(TrueCodeContext dbContext)
        {
            var controller = new FinanceController(dbContext);

            // Моделируем HttpContext с авторизованным пользователем
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testUser")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        [Fact]
        public async Task AddFavoriteTest()
        {
            var dbContext = TestConfig.GetDbContext();
            dbContext.Users.Add(
                new User 
                { 
                    Id = 1, 
                    Name = "testUser", 
                    Password = "pass123" 
                });
            dbContext.Currencies.Add(
                new Currency 
                { 
                    Id = 100, 
                    Name = "USD", 
                    Rate = 1 
                });
            await dbContext.SaveChangesAsync();

            var controller = GetControllerWithContext(dbContext);
            var result = await controller.AddFavorite(100);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Добавлено в избранное", ok.Value as string);

            Assert.Single(dbContext.UserCurrencies);
        }

        [Fact]
        public async Task GetFavoritesTest()
        {
            var dbContext = TestConfig.GetDbContext();
            dbContext.Users.Add(
                new User
                {
                    Id = 1,
                    Name = "testUser",
                    Password = "pass123"
                });
            dbContext.Currencies.Add(
                new Currency
                {
                    Id = 200,
                    Name = "EUR",
                    Rate = 0.9m
                });
            dbContext.UserCurrencies.Add(
                new UserCurrency 
                { 
                    UserId = 1, 
                    CurrencyId = 200 
                });
            await dbContext.SaveChangesAsync();

            var controller = GetControllerWithContext(dbContext);
            var result = await controller.GetFavorites();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetFavoritesTestReturnEmpty()
        {
            var dbContext = TestConfig.GetDbContext();

            dbContext.Currencies.Add(new Currency { Name = "USD", Rate = 1 });
            dbContext.Currencies.Add(new Currency { Name = "EUR", Rate = 0.9m });
            await dbContext.SaveChangesAsync();

            var controller = GetControllerWithContext(dbContext);
            var result = await controller.GetFavorites();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            // т.к. не было добавлено ни одной валюты для пользователя, должен вернуться пустой список
            Assert.Empty(list);
        }

        [Fact]
        public async Task AddFavoriteTestNegaive()
        {
            var dbContext = TestConfig.GetDbContext();
            dbContext.Users.Add(
                new User
                {
                    Id = 1,
                    Name = "testUser",
                    Password = "pass123"
                });
            await dbContext.SaveChangesAsync();

            var controller = GetControllerWithContext(dbContext);
            // пытаемся добавить неесуществующую валюту
            var result = await controller.AddFavorite(999); 

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Валюта с id {999} не найдена", notFound.Value as string);
        }

        [Fact]
        public async Task AddFavoriteTestAlreadyInFavorites()
        {
            var dbContext = TestConfig.GetDbContext();
            dbContext.Users.Add(
                new User
                {
                    Id = 1,
                    Name = "testUser",
                    Password = "pass123"
                });
            dbContext.Currencies.Add(
                new Currency
                {
                    Id = 100,
                    Name = "USD",
                    Rate = 1
                });
            dbContext.UserCurrencies.Add(
                new UserCurrency
                {
                    UserId = 1,
                    CurrencyId = 100
                });
            await dbContext.SaveChangesAsync();

            var controller = GetControllerWithContext(dbContext);
            var result = await controller.AddFavorite(100);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Валюта уже в избранном", ok.Value as string);
        }

        [Fact]
        public async Task GetFavoritesTestUnauthorized()
        {
            var dbContext = TestConfig.GetDbContext();
            var controller = new FinanceController(dbContext); // без HttpContext

            var result = await controller.GetFavorites();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
