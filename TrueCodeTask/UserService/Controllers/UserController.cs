using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using SharedModels.EntityModels;
using UserService.Managers;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TrueCodeContext _dbContext;
        private readonly TokenManager _tokenManager;

        public UserController(TrueCodeContext dbContext, TokenManager tokenManager)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Name == req.Name))
            {
                return BadRequest("Пользователь с таким именем уже существует");
            }

            var hashed = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var user = new User
            {
                Name = req.Name,
                Password = hashed
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok($"id {user.Id}, name: {user.Name}");
        }

        /// <summary>
        /// Авторизация и получение JWT токена
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Неверный логин или пароль");
            }

            var token = _tokenManager.GenerateJwtToken(user);
            return Ok(new { token });
        }

        /// <summary>
        /// Логаут пользователя
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Чтобы выполнить логаут, удалите токен на клиенте");
        }
    }
}
