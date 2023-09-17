using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;
using Microsoft.Net.Http.Headers;
using API = Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IPublisherService : IApiEntityService<API.Publisher>, IApiEntityListService<API.PublisherInfo>
    {
        Task<IServiceResponse<ImportResult>> Import(API.ApiApplication application, User user);

        Task<IFileOperationResponse<IImage>> GetPublisherImageAsync(Guid id, int width, int height, EntityTagHeaderValue etag = null);
    }
}