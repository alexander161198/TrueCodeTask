using BackgroundCurrencyService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedModels;

namespace BackgroundCurrencyService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TrueCodeContext>(opt =>
                opt.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

            services.AddHostedService<UpdateDataBaseService>();
        }
    }
}
