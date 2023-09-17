using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;
using Kapowey.Core.Persistance;
using LazyCache;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using API = Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public sealed class CollectionService : ServiceBase, ICollectionService, ICollectionIssueService
    {
        public ILogger<CollectionService> Logger { get; set; }

        public CollectionService(
            AppConfigurationSettings appSettings,
            ILogger<CollectionService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
        }

        public Task<IPagedResponse<API.Collection>> ListAsync(User user, PagedRequest request) => throw new NotImplementedException();

        public async Task<IServiceResponse<API.Collection>> ByIdAsync(User user, Guid apiKeyToGet)
        {
            var data = await DbContext.Collection.FirstOrDefaultAsync(x => x.ApiKey == apiKeyToGet).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<API.Collection>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKeyToGet }]", ServiceResponseMessageType.NotFound));
            }
            return new ServiceResponse<API.Collection>(data.Adapt<API.Collection>());
        }

        public Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKeyToDelete) => throw new NotImplementedException();

        public Task<IServiceResponse<bool>> ModifyAsync(User user, API.Collection modifyModel) => throw new NotImplementedException();

        public Task<IServiceResponse<Guid>> AddAsync(User user, API.Collection createModel) => throw new NotImplementedException();

        Task<IPagedResponse<API.CollectionIssueInfo>> IApiEntityListService<API.CollectionIssueInfo>.ListAsync(User user, PagedRequest request) => throw new NotImplementedException();

        Task<IServiceResponse<API.CollectionIssue>> IApiEntityService<API.CollectionIssue>.ByIdAsync(User user, Guid apiKeyToGet) => throw new NotImplementedException();

        public Task<IServiceResponse<bool>> ModifyAsync(User user, API.CollectionIssue modifyModel) => throw new NotImplementedException();

        public Task<IServiceResponse<Guid>> AddAsync(User user, API.CollectionIssue createModel) => throw new NotImplementedException();
    }
}