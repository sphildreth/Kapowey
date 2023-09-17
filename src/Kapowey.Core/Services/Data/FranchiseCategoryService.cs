using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;
using Kapowey.Core.Enums;
using Kapowey.Core.Persistance;
using LazyCache;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using API = Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public sealed class FranchiseCategoryService : ServiceBase, IFranchiseCategoryService
    {
        public ILogger<FranchiseCategoryService> Logger { get; set; }

        public FranchiseCategoryService(
            AppConfigurationSettings appSettings,
            ILogger<FranchiseCategoryService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
        }

        public async Task<IServiceResponse<API.FranchiseCategory>> ByIdAsync(User user, Guid apiKeyToGet)
        {
            var data = await DbContext.FranchiseCategory.FirstOrDefaultAsync(x => x.ApiKey == apiKeyToGet).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<API.FranchiseCategory>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKeyToGet }]", ServiceResponseMessageType.NotFound));
            }
            return new ServiceResponse<API.FranchiseCategory>(data.Adapt<API.FranchiseCategory>());
        }

        public async Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKey)
        {
            var data = await DbContext.FranchiseCategory.FirstOrDefaultAsync(x => x.ApiKey == apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            DbContext.FranchiseCategory.Remove(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogWarning($"User `{ user }` deleted: Franchise Category `{ data }`.");
            return new ServiceResponse<bool>(true);
        }

        public async Task<IServiceResponse<bool>> ModifyAsync(User user, API.FranchiseCategory modify)
        {
            var data = await DbContext.FranchiseCategory.FirstOrDefaultAsync(x => x.ApiKey == modify.ApiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ modify.ApiKey }]", ServiceResponseMessageType.NotFound));
            }
            data.Description = modify.Description;
            data.ParentFranchiseCategoryId = null;
            if (modify?.ParentFranchiseCategory?.ApiKey != null)
            {
                var parent = await ByIdAsync(user, modify.ParentFranchiseCategory.ApiKey.Value).ConfigureAwait(false);
                data.ParentFranchiseCategoryId = parent.Data.FranchiseCategoryId;
            }
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = user.Id;
            data.Name = modify.Name;
            data.ShortName = modify.ShortName;
            data.Status = Status.Edited;
            data.Tags = modify.Tags;
            data.Url = modify.Url;
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0);
        }

        public async Task<IServiceResponse<Guid>> AddAsync(User user, API.FranchiseCategory create)
        {
            var data = new FranchiseCategory
            {
                ApiKey = Guid.NewGuid(),
                Description = create.Description,
                CreatedDate = Instant.FromDateTimeUtc(DateTime.UtcNow),
                CreatedUserId = user.Id,
                Name = create.Name,
                ShortName = create.ShortName,
                Status = Status.New,
                Tags = create.Tags,
                Url = create.Url
            };
            if (create?.ParentFranchiseCategory?.ApiKey != null)
            {
                var parent = await ByIdAsync(user, create.ParentFranchiseCategory.ApiKey.Value).ConfigureAwait(false);
                data.ParentFranchiseCategoryId = parent.Data.FranchiseCategoryId;
            }
            await DbContext.FranchiseCategory.AddAsync(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<Guid>(data.ApiKey.Value);
        }

        public async Task<IPagedResponse<API.FranchiseCategory>> ListAsync(User user, PagedRequest request)
        {
            if (!request.IsValid)
            {
                return new PagedResponse<API.FranchiseCategory>(new ServiceResponseMessage("Invalid Request", ServiceResponseMessageType.Error));
            }
            return await CreatePagedResponse<FranchiseCategory, API.FranchiseCategory>(DbContext.FranchiseCategory, request).ConfigureAwait(false);
        }
    }
}