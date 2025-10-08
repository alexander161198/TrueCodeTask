using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedModels;

namespace MigrationService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((ctx, services) =>
            {
                services.AddDbContext<TrueCodeContext>(options =>
                    options.UseNpgsql(
                        ctx.Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("MigrationService")));
            })
            .Build();
    }

    /* use commands:
    dotnet ef migrations add Init --startup-project MigrationService --project MigrationService
    dotnet ef database update --startup-project MigrationService --project MigrationService
    */
}