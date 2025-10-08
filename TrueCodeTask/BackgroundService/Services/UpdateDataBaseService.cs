using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedModels;
using SharedModels.EntityModels;
using System.Xml.Linq;

namespace BackgroundCurrencyService.Services
{
    public class UpdateDataBaseService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient = new();

        public UpdateDataBaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var xml = await _httpClient.GetStringAsync("http://www.cbr.ru/scripts/XML_daily.asp");
                var doc = XDocument.Parse(xml);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<TrueCodeContext>();

                    foreach (var el in doc.Descendants("Valute"))
                    {
                        var name = el.Element("CharCode")!.Value;
                        var rate = decimal.Parse(el.Element("Value")!.Value);

                        var existing = db.Currencies.FirstOrDefault(c => c.Name == name);
                        if (existing == null)
                        {
                            db.Currencies.Add(new Currency { Name = name, Rate = rate });
                        }
                        else
                        {
                            existing.Rate = rate;
                        }
                    }

                    await db.SaveChangesAsync();
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}
