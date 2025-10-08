using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedModels;

namespace TestServices
{
    public static class TestConfig
    {
        public static TrueCodeContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TrueCodeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TrueCodeContext(options);
        }

        public static IOptions<JwtOptions> GetJwtOptions()
        {
            return Options.Create(new JwtOptions
            {
                Key = "This is custom key for jwt 1234567890",
                Issuer = "TestIssuer",
                ExpireHours = 1
            });
        }
    }
}
