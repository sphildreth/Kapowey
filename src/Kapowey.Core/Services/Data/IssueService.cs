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
    public sealed class IssueService : ServiceBase, IIssueService
    {
        public ILogger<IssueService> Logger { get; set; }

        private ISeriesService SeriesService { get; }

        private IUserService UserService { get; }

        private IIssueTypeService IssueTypeService { get; }

        public IssueService(
            AppConfigurationSettings appSettings,
            ILogger<IssueService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext,
            ISeriesService seriesService,
            IIssueTypeService issueTypeService,
            IUserService userService)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
            SeriesService = seriesService;
            IssueTypeService = issueTypeService;
            UserService = userService;
        }

        public async Task<IServiceResponse<API.Issue>> ByIdAsync(User user, Guid apiKey)
        {
            var data = await DbContext.Issue.FirstOrDefaultAsync(x => x.ApiKey == apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<API.Issue>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            return new ServiceResponse<API.Issue>(data.Adapt<API.Issue>());
        }

        public async Task<IPagedResponse<API.IssueInfo>> ListAsync(User user, PagedRequest request)
        {
            if (!request.IsValid)
            {
                return new PagedResponse<API.IssueInfo>(new ServiceResponseMessage("Invalid Request", ServiceResponseMessageType.Error));
            }
            return await CreatePagedResponse<Issue, API.IssueInfo>(DbContext.Issue, request).ConfigureAwait(false);
        }

        public async Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKey)
        {
            var data = await DbContext.Issue.FirstOrDefaultAsync(x => x.ApiKey == apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            DbContext.Issue.Remove(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogWarning($"User `{ user }` deleted: Issue `{ data }`.");
            return new ServiceResponse<bool>(true);
        }

        public async Task<IServiceResponse<bool>> ModifyAsync(User user, API.Issue modify)
        {
            var data = await DbContext.Issue.FirstOrDefaultAsync(x => x.ApiKey == modify.ApiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ modify.ApiKey }]", ServiceResponseMessageType.NotFound));
            }
            data.Barcode = modify.Barcode;
            data.CoverPrice = modify.CoverPrice;
            data.CultureCode = modify.CultureCode;
            data.Description = modify.Description;
            if (modify?.Series?.ApiKey != null)
            {
                var series = await SeriesService.ByIdAsync(user, modify.Series.ApiKey.Value).ConfigureAwait(false);
                data.SeriesId = series.Data.SeriesId;
            }
            if (modify?.IssueType?.ApiKey != null)
            {
                var issueType = await IssueTypeService.ByIdAsync(user, modify.IssueType.ApiKey.Value).ConfigureAwait(false);
                data.IssueTypeId = issueType.Data.IssueTypeId;
            }
            if (modify?.ReprintOfIssue?.ApiKey != null)
            {
                var reprintIssue = await ByIdAsync(user, modify.ReprintOfIssue.ApiKey.Value).ConfigureAwait(false);
                data.ReprintOfIssueId = reprintIssue.Data.IssueTypeId;
            }
            data.GcdId = modify.GcdId;
            data.Isbn = modify.ISBN;
            data.KeyDate = modify.KeyDate.Value.ToDateTimeUtc();
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = user.Id;
            data.Number = modify.Number;
            if (modify?.ReviewedUser?.ApiKey != null)
            {
                var reviewedUser = await UserService.ByIdAsync(user, modify.ReviewedUser.ApiKey.Value).ConfigureAwait(false);
                data.ReviewedDate ??= Instant.FromDateTimeUtc(DateTime.UtcNow);
                data.ReviewedUserId = reviewedUser.Data.UserId;
            }
            data.Rating = modify.Rating;
            if (modify?.Series?.ApiKey != null)
            {
                var series = await SeriesService.ByIdAsync(user, modify.Series.ApiKey.Value).ConfigureAwait(false);
                data.SeriesId = series.Data.SeriesId;
            }
            data.ShortTitle = modify.ShortTitle;
            data.SortOrder = modify.SortOrder;
            data.Status = Status.Edited;
            data.Title = modify.Title;
            data.Tags = modify.Tags;
            data.Url = modify.Url;
            data.VariantTitle = modify.VariantTitle;
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0);
        }

        public async Task<IServiceResponse<Guid>> AddAsync(User user, API.Issue create)
        {
            var data = new Issue
            {
                ApiKey = Guid.NewGuid(),
                Barcode = create.Barcode,
                CoverPrice = create.CoverPrice,
                CultureCode = create.CultureCode,
                CreatedDate = Instant.FromDateTimeUtc(DateTime.UtcNow),
                CreatedUserId = user.Id,
                Description = create.Description,
                GcdId = create.GcdId,
                Isbn = create.ISBN,
                KeyDate = create.KeyDate.Value.ToDateTimeUtc(),
                Number = create.Number,
                Rating = create.Rating,
                ShortTitle = create.ShortTitle,
                SortOrder = create.SortOrder,
                Status = Status.New,
                Tags = create.Tags,
                Title = create.Title,
                Url = create.Url,
                VariantTitle = create.VariantTitle
            };
            if (create?.Series?.ApiKey != null)
            {
                var series = await SeriesService.ByIdAsync(user, create.Series.ApiKey.Value).ConfigureAwait(false);
                data.SeriesId = series.Data.SeriesId;
            }
            if (create?.IssueType?.ApiKey != null)
            {
                var issueType = await IssueTypeService.ByIdAsync(user, create.IssueType.ApiKey.Value).ConfigureAwait(false);
                data.IssueTypeId = issueType.Data.IssueTypeId;
            }
            if (create?.ReprintOfIssue?.ApiKey != null)
            {
                var reprintIssue = await ByIdAsync(user, create.ReprintOfIssue.ApiKey.Value).ConfigureAwait(false);
                data.ReprintOfIssueId = reprintIssue.Data.IssueTypeId;
            }
            if (create?.ReviewedUser?.ApiKey != null)
            {
                var reviewedUser = await UserService.ByIdAsync(user, create.ReviewedUser.ApiKey.Value).ConfigureAwait(false);
                data.ReviewedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
                data.ReviewedUserId = reviewedUser.Data.UserId;
            }
            if (create?.Series?.ApiKey != null)
            {
                var series = await SeriesService.ByIdAsync(user, create.Series.ApiKey.Value).ConfigureAwait(false);
                data.SeriesId = series.Data.SeriesId;
            }
            await DbContext.Issue.AddAsync(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<Guid>(data.ApiKey.Value);
        }
    }
}