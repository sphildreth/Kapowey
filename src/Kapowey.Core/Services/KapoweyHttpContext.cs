using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;

namespace Kapowey.Core.Services
{
    public class KapoweyHttpContext : IKapoweyHttpContext
    {
        private ISystemClock Clock { get; }

        public string BaseUrl { get; set; }

        public Uri ImageBaseUri { get; set; }

        private string CacheBuster
        {
            get
            {
                return Clock.UtcNow.ToUnixTimeSeconds().ToString();
            }
        }

        public KapoweyHttpContext(
            AppConfigurationSettings appSettings,
            IUrlHelper urlHelper,
            ISystemClock clock)
        {
            var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            if (appSettings.BehindSSLProxy)
            {
                scheme = "https";
            }

            var host = urlHelper.ActionContext.HttpContext.Request.Host;
            if (!string.IsNullOrEmpty(appSettings.ProxyIP))
            {
                host = new HostString(appSettings.ProxyIP);
            }

            Clock = clock;
            BaseUrl = $"{scheme}://{host}";
            ImageBaseUri = new Uri($"{BaseUrl}/images");

        }

        public string MakePathForTypeAndId(UriPathType type, Guid id, bool? doCacheBuster = true)
        {
            return $"{ImageBaseUri.AbsoluteUri}/{type.ToString().ToLower()}/{id}.png{( (doCacheBuster ?? true) ? $"?cb={ CacheBuster}" : String.Empty)}";
        }
    }
}