using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kapowey.Core.Common.Extensions;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class ApiApplication : EntityBase
    {
        public const int MaximumTokenAgeInDays = 1;

        [Required]
        [StringLength(500)]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public virtual int ApiApplicationId { get; set; }

        [Required]
        [StringLength(10)]
        public virtual string ShortName { get; set; }

        public virtual Instant? LastActivity { get; set; }

        public virtual string Salt { get; set; }

        public override Status Status { get; set; }

        public Uri BuildUrl(string url)
        {
            if (string.IsNullOrEmpty(Url))
            {
                return new Uri(url);
            }
            return new Uri(new Uri(Url), url);
        }

        public static string BuildHMACTokenData(string apiKey, Instant instant, string url)
        {
            var md5Part = $"{apiKey}│{ instant.ToUnixTimeMilliseconds() }│{url}".ToMD5();
            return $"{instant.ToUnixTimeMilliseconds()}│{ md5Part}";
        }

        /// <summary>
        /// Returns a Base64 encoded HMAC token for this application for the given Url.
        /// </summary>
        public string GenerateHMACToken(string url) => GenerateHMACToken(SystemClock.Instance.GetCurrentInstant(), url);

        /// <summary>
        /// Returns a Base64 encoded HMAC token for this application for the given time instant and Url.
        /// </summary>
        public string GenerateHMACToken(Instant instant, string url)
        {
            if (Status == Status.Locked || Status == Status.Inactive)
            {
                return null;
            }
            var tokenData = BuildHMACTokenData(ApiKey.ToString(), instant, url);
            return $"{ tokenData.ToSHA512($"{ Salt }{ tokenData}") }│{ tokenData} ".ToBase64();
        }

        public bool IsValidToken(string hmacToken, string url)
        {
            if (string.IsNullOrEmpty(hmacToken))
            {
                return false;
            }
            if(Status == Status.Locked || Status == Status.Inactive)
            {
                return false;
            }
            var parts = hmacToken.FromBase64()?.Split("│") ?? Array.Empty<string>();
            if (parts.Length != 3)
            {
                return false;
            }
            var signature = parts[0];

            var instant = Instant.FromUnixTimeMilliseconds(long.Parse(parts[1]));
            var tokenAge = Instant.FromDateTimeUtc(DateTime.UtcNow) - instant;
            if (tokenAge.TotalDays > MaximumTokenAgeInDays)
            {
                return false;
            }
            return string.Equals(hmacToken, GenerateHMACToken(instant, url));
        }
    }
}