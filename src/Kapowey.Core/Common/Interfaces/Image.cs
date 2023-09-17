using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Common.Interfaces
{
    public sealed class Image : IImage
    {
        public Guid Id { get; }
        public Status Status { get; set; }
        public short SortOrder { get; set; }
        public byte[] Bytes { get; set; }
        public Instant CreatedDate { get; set; }
        public Instant LastUpdated { get; set; }
        public string Signature { get; set; }
        public string Url { get; set; }

        public Image()
        {
        }

        public Image(Guid id)
        {
            Id = id;
        }

        public override string ToString() => $"Id [{Id}]";

        public string CacheKey => CacheUrn(Id);

        public string CacheRegion => CacheRegionUrn(Id);

        public static string CacheRegionUrn(Guid Id)
        {
            return $"urn:artist:{Id}";
        }

        public static string CacheUrn(Guid Id)
        {
            return $"urn:artist_by_id:{Id}";
        }
    }
}