using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using SharedModels.EntityModels;
using System.Security.Claims;

namespace FinanceService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FinanceController : ControllerBase
    {
        private readonly TrueCodeContext _dbContext;

        public FinanceController(TrueCodeContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Добавить валюту в избранное
        /// </summary>
        [HttpPost("favorites/{currencyId:int}")]
        [Authorize]
        public async Task<IActionResult> AddFavorite(int currencyId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized("Не удалось распознать пользователя");
            }

            var currency = await _dbContext.Currencies.FindAsync(currencyId);
            if (currency == null)
            {
                return NotFound($"Валюта с id {currencyId} не найдена");
            }

            var exists = await _dbContext.UserCurrencies
                .AnyAsync(uc => uc.UserId == userId && uc.CurrencyId == currencyId);

            if (exists)
            {
                return Ok("Валюта уже в избранном");
            }

            var usercurFavorite = new UserCurrency
            {
                UserId = userId.Value,
                CurrencyId = currencyId
            };

            _dbContext.UserCurrencies.Add(usercurFavorite);
            await _dbContext.SaveChangesAsync();

            return Ok("Добавлено в избранное");
        }

        /// <summary>
        /// Получить список избранных валют пользователя
        /// </summary>
        [HttpGet("GetFavorites")]
        [Authorize]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            if (userId == null)
            { 
                return Unauthorized("Не удалось распознать пользователя");
            }

            var favorites = await _dbContext.UserCurrencies
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Currency)
                .Select(uc => new
                {
                    uc.Currency.Id,
                    uc.Currency.Name,
                    uc.Currency.Rate
                })
                .ToListAsync();

            return Ok(favorites);
        }

        private int? GetUserId()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return null;

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(id, out var parsed) ? parsed : null;
        }
    }
}
