using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Enums;
using Microsoft.Net.Http.Headers;

namespace Kapowey.Core.Services.Data
{
    public interface IImageService
    {
        /// <summary>
        /// Path to image type and ApiKey in StorageFolder.
        /// </summary>
        string ImagePath(ImageType type, Guid apiKey);

        /// <summary>
        /// Path to image type and ApiKey in StorageFolder.
        /// </summary>
        string ImagePath(ImageType type, string apiKey);

        Task<IFileOperationResponse<IImage>> GetImageAsyncAction(ImageType imageType, string regionUrn, Guid id, int width, int height, Func<Task<IImage>> action, EntityTagHeaderValue etag = null);
    }
}