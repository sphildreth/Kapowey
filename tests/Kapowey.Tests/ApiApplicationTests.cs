using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Tests
{
    public class ApiApplicationTests
    {
        private ApiApplication NewApplication()
        {
            return new()
            {
                Name = "Testing Application",
                Salt = "234fNeOcIWwIp#E?WDU.=TTR*0opuKskS/j3duD:zGT1!*w5Q-XZ:!Tj#NSv-pb:eoIHKbkr!Vm6vYKCv4@05TPSu(w32X)(vYXW*lN-SyY(h+YUoYQax-HBoaSQasdf",
                ApiKey = Guid.NewGuid(),
                Url = "http://localhost:3000",
                ApiApplicationId = 1,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public void BuildUrlShouldPass()
        {
            var url = "/user/passwordreset?e=bob@bob.com";
            var app = NewApplication();
            var appUrl = app.BuildUrl(url);
            Assert.Equal($"http://localhost:3000{url}", appUrl.AbsoluteUri);

            url = "user/passwordreset?e=bob@bob.com";
            appUrl = app.BuildUrl(url);
            Assert.Equal($"http://localhost:3000/{url}", appUrl.AbsoluteUri);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("b=1")]
        public void IsValidTokenShouldFail(string token) => Assert.False(NewApplication().IsValidToken(token, token));

        [Fact]
        public void TestHMACGenerationShouldFail()
        {
            var url = "/user/passwordreset?e=bob@bob.com";
            var app = NewApplication();
            var appUrl = app.BuildUrl(url);
            var hmac = app.GenerateHMACToken(NodaTime.SystemClock.Instance.GetCurrentInstant(), url);
            Assert.NotNull(hmac);
            Assert.False(app.IsValidToken(hmac, null));
            Assert.False(app.IsValidToken(hmac, string.Empty));
            Assert.False(app.IsValidToken(hmac, $"{url}&b=14"));
            Assert.False(app.IsValidToken(hmac, hmac));
            Assert.False(app.IsValidToken(null, hmac));
        }

        [Fact]
        public void TestHMACGenerationShouldPass()
        {
            var url = "/user/passwordreset?e=bob@bob.com";
            var app = NewApplication();
            var appUrl = app.BuildUrl(url);
            var hmac = app.GenerateHMACToken(NodaTime.SystemClock.Instance.GetCurrentInstant(), url);
            Assert.NotNull(hmac);
            Assert.True(app.IsValidToken(hmac, url));
        }
    }
}
