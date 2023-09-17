using Kapowey.Core.Enums;

namespace Kapowey.Core.Common.Interfaces
{
    public interface IKapoweyHttpContext
    {
        string BaseUrl { get; set; }

        Uri ImageBaseUri { get; set; }

        public string MakePathForTypeAndId(UriPathType type, Guid id, bool? doCacheBuster = true);
    }
}