using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Extensions;
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
    public sealed class ApiApplicationService : ServiceBase, IApiApplicationService
    {
        private const string _applicationByApplicationApiKey = "urn:application:by:apiKey:{0}";
        private const string _applicationByApplicationIdKey = "urn:application:by:id:{0}";

        public ILogger<ApiApplicationService> Logger { get; set; }

        public ApiApplicationService(
            AppConfigurationSettings appSettings,
            ILogger<ApiApplicationService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
        }

        public Task<IPagedResponse<API.ApiApplication>> ListAsync(User user, PagedRequest request) => throw new NotImplementedException();

        private Task<int?> GetApiApplicationIdByApiKey(Guid apiKey)
        {
            if(apiKey == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(apiKey));
            }
            return CacheManager.GetOrAddAsync(_applicationByApplicationApiKey.ToCacheKey(apiKey.ToString()), async () =>
            {
                return await DbContext.ApiApplication.Where(x => x.ApiKey == apiKey).Select(x => (int?)x.ApiApplicationId).FirstOrDefaultAsync();
            }, Options);
        }

        private Task<ApiApplication> GetApplicationByIdAction(int id)
        {
            if(id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }            
            return CacheManager.GetOrAddAsync(_applicationByApplicationIdKey.ToCacheKey(id), async () =>
            {
                return await DbContext.ApiApplication.FirstOrDefaultAsync(x => x.ApiApplicationId == id);
            }, Options);
        }

        public async Task<IServiceResponse<API.ApiApplication>> ByIdAsync(User user, Guid apiKey)
        {
            var applicationId = await GetApiApplicationIdByApiKey(apiKey).ConfigureAwait(false);
            var applicationData = await GetApplicationByIdAction(applicationId.Value).ConfigureAwait(false);
            if (applicationData == null)
            {
                return new ServiceResponse<API.ApiApplication>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            return new ServiceResponse<API.ApiApplication>(applicationData.Adapt<API.ApiApplication>());
        }

        public Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKeyToDelete) => throw new NotImplementedException();

        public Task<IServiceResponse<bool>> ModifyAsync(User user, API.ApiApplication modifyModel) => throw new NotImplementedException();

        public Task<IServiceResponse<Guid>> AddAsync(User user, API.ApiApplication createModel) => throw new NotImplementedException();
    }
}