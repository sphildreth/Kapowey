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
    public sealed class SeriesService : ServiceBase, ISeriesService
    {
        private ILogger<SeriesService> Logger { get; }

        private IGenreService GenreService { get; }

        private ISeriesCategoryService SeriesCategoryService { get; }

        public SeriesService(
            AppConfigurationSettings appSettings,
            ILogger<SeriesService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext,
            IGenreService genreService,
            ISeriesCategoryService seriesCategoryService)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
            GenreService = genreService;
            SeriesCategoryService = seriesCategoryService;
        }

        public async Task<IServiceResponse<API.Series>> ByIdAsync(User user, Guid apiKey)
        {
            var data = await DbContext.Series.FirstOrDefaultAsync(x => x.ApiKey == apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<API.Series>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            return new ServiceResponse<API.Series>(data.Adapt<API.Series>());
        }

        public async Task<IPagedResponse<API.SeriesInfo>> ListAsync(User user, PagedRequest request)
        {
            if (!request.IsValid)
            {
                return new PagedResponse<API.SeriesInfo>(new ServiceResponseMessage("Invalid Request", ServiceResponseMessageType.Error));
            }
            return await CreatePagedResponse<Series, API.SeriesInfo>(DbContext.Series, request).ConfigureAwait(false);
        }

        public async Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKey)
        {
            var data = await DbContext.Series.FirstOrDefaultAsync(x => x.ApiKey == apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            DbContext.Series.Remove(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogWarning($"User `{ user }` deleted: Series `{ data }`.");
            return new ServiceResponse<bool>(true);
        }

        public async Task<IServiceResponse<bool>> ModifyAsync(User user, API.Series modify)
        {
            var data = await DbContext.Series.FirstOrDefaultAsync(x => x.ApiKey == modify.ApiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ modify.ApiKey }]", ServiceResponseMessageType.NotFound));
            }
            data.CultureCode = modify.CultureCode;
            data.Description = modify.Description;
            if (modify.FirstIssueId.HasValue)
            {
                data.FirstIssueId = await IssueIdForIssueApiId(user, modify.FirstIssueId.Value).ConfigureAwait(false);
            }
            if (modify.LastIssueId.HasValue)
            {
                data.LastIssueId = await IssueIdForIssueApiId(user, modify.LastIssueId.Value).ConfigureAwait(false);
            }
            data.GcdId = modify.GcdId;
            if (modify?.Genre?.ApiKey != null)
            {
                var genre = await GenreService.ByIdAsync(user, modify.Genre.ApiKey.Value).ConfigureAwait(false);
                data.GenreId = genre.Data.GenreId;
            }
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = user.Id;
            data.Name = modify.Name;
            if (modify?.SeriesCategory?.ApiKey != null)
            {
                var category = await SeriesCategoryService.ByIdAsync(user, modify.SeriesCategory.ApiKey.Value).ConfigureAwait(false);
                data.SeriesCategoryId = category.Data.SeriesCategoryId;
            }
            data.ShortName = modify.ShortName;
            data.Status = Status.Edited;
            data.Tags = modify.Tags;
            data.Url = modify.Url;
            data.YearBegan = modify.YearBegan;
            data.YearEnd = modify.YearEnd;
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0);
        }

        public async Task<IServiceResponse<Guid>> AddAsync(User user, API.Series create)
        {
            var data = new Series
            {
                ApiKey = Guid.NewGuid(),
                CultureCode = create.CultureCode,
                Description = create.Description,
                GcdId = create.GcdId,
                CreatedDate = Instant.FromDateTimeUtc(DateTime.UtcNow),
                CreatedUserId = user.Id,
                Name = create.Name,
                Rating = create.Rating,
                ShortName = create.ShortName,
                Status = Status.New,
                Tags = create.Tags,
                Url = create.Url,
                YearBegan = create.YearBegan,
                YearEnd = create.YearEnd
            };
            if (create?.Genre?.ApiKey != null)
            {
                var genre = await GenreService.ByIdAsync(user, create.Genre.ApiKey.Value).ConfigureAwait(false);
                data.GenreId = genre.Data.GenreId;
            }
            if (create.FirstIssueId.HasValue)
            {
                data.FirstIssueId = await IssueIdForIssueApiId(user, create.FirstIssueId.Value).ConfigureAwait(false);
            }
            if (create.LastIssueId.HasValue)
            {
                data.LastIssueId = await IssueIdForIssueApiId(user, create.LastIssueId.Value).ConfigureAwait(false);
            }
            if (create?.SeriesCategory?.ApiKey != null)
            {
                var category = await SeriesCategoryService.ByIdAsync(user, create.SeriesCategory.ApiKey.Value).ConfigureAwait(false);
                data.SeriesCategoryId = category.Data.SeriesCategoryId;
            }
            await DbContext.Series.AddAsync(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<Guid>(data.ApiKey.Value);
        }
    }
}