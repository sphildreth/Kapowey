using Kapowey.Core.Common.Models;

namespace Kapowey.Core.Common.Interfaces;

public interface IUploadService
{
    Task<string> UploadAsync(UploadRequest request);
}
