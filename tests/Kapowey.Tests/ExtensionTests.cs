using Kapowey.Core.Common.Extensions;

namespace Kapowey.Tests
{
    public class ExtensionTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("batman", false)]
        [InlineData("Bruce Wayne", false)]
        [InlineData("bwayne@@statelymanner", false)]
        [InlineData("bwayne@statelymanner", false)]
        [InlineData("@statelymanner.org", false)]
        [InlineData("bwayne@statelymanner.org", true)]
        [InlineData("bwayne@statelymanner.edu", true)]
        [InlineData("bwayne@statelymanner.com", true)]
        [InlineData("bwayne@statelymanner.net", true)]
        public void IsValidEmail(string input, bool shouldBe) => Assert.Equal(shouldBe, input?.IsValidEmail() ?? false);

        [Fact]
        public void ToAndFromBase64()
        {
            var shouldBe = "Testing 12345 bob@test.com";

            var base64 = shouldBe.ToBase64();
            Assert.NotNull(base64);
            Assert.NotEqual(shouldBe, base64);

            var fromBas64 = base64.FromBase64();
            Assert.NotNull(base64);
            Assert.NotEqual(base64, fromBas64);

            Assert.Equal(shouldBe, fromBas64);
        }
    }
}
