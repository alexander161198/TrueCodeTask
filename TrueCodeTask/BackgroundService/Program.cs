using Microsoft.Extensions.Hosting;
using System.Text;

namespace BackgroundCurrencyService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostCtx, services) =>
                {
                    var startup = new Startup(hostCtx.Configuration);
                    startup.ConfigureServices(services);
                });
    }
}