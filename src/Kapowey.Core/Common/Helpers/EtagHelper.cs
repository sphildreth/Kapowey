using Kapowey.Core.Common.Interfaces;
using Microsoft.Net.Http.Headers;

namespace Kapowey.Core.Common.Helpers
{
    public static class EtagHelper
    {
        public static bool CompareETag(IHttpEncoder encoder, EntityTagHeaderValue eTagLeft, EntityTagHeaderValue eTagRight)
        {
            if (encoder == null)
            {
                return false;
            }
            if (eTagLeft == null && eTagRight == null) return true;
            if (eTagLeft == null && eTagRight != null) return false;
            if (eTagRight == null && eTagLeft != null) return false;
            return eTagLeft == eTagRight;
        }

        public static bool CompareETag(IHttpEncoder encoder, EntityTagHeaderValue eTag, byte[] bytes)
        {
            if (encoder == null)
            {
                return false;
            }
            if (eTag == null && (bytes == null || !bytes.Any())) return true;
            if (eTag == null && bytes != null || bytes.Any()) return false;
            if (eTag != null && (bytes == null || !bytes.Any())) return false;
            var etag = GenerateETag(encoder, bytes);
            return eTag == etag;
        }

        public static EntityTagHeaderValue GenerateETag(IHttpEncoder encoder, byte[] bytes)
        {
            if (encoder == null || bytes == null || !bytes.Any()) return null;
            return new EntityTagHeaderValue($"\"{encoder.UrlEncodeBase64(HashHelper.CreateMD5(bytes))}\"");
        }
    }
}