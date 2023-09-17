using System.Diagnostics;
using System.Linq.Dynamic.Core;
using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;
using Kapowey.Core.Persistance;
using LazyCache;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kapowey.Core.Services.Data
{
    public abstract class ServiceBase
    {
        protected const string ErrorOccured = "An error has occured";
        protected const string Key = "Bearer ";
        protected const string NewKey = "__new__";
        protected const string NoImageDataFound = "NO_IMAGE_DATA_FOUND";
        protected const string NotModifiedMessage = "NotModified";
        protected const string OkMessage = "OK";

        private static TimeSpan RefreshInterval => TimeSpan.FromHours(4);
        protected LazyCacheEntryOptions Options => new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);
        
        protected AppConfigurationSettings AppSettings { get; }

        protected IAppCache CacheManager { get; }

        protected KapoweyContext DbContext { get; }

        protected ServiceBase(AppConfigurationSettings appSettings, IAppCache cacheManager, KapoweyContext dbContext)
        {
            AppSettings = appSettings;
            CacheManager = cacheManager;
            DbContext = dbContext;
        }

        /// <summary>
        /// Creates a paged set of data for the given Queryable and Request.
        /// </summary>
        /// <typeparam name="T">The type of the source IQueryable.</typeparam>
        /// <typeparam name="TReturn">The type of the returned paged results.</typeparam>
        /// <param name="queryable">The source IQueryable.</param>
        /// <param name="request">Request model.</param>
        /// <returns>Returns a paged set of results.</returns>
        protected async Task<IPagedResponse<TReturn>> CreatePagedResponse<T, TReturn>(IQueryable<T> queryable, PagedRequest request, Func<List<TReturn>, List<TReturn>> resultsPostFunc = null) where T : class
        {
            string filter = null;
            int totalNumberOfRecords = 0;

            try
            {
                filter = request.FilterSql();
                totalNumberOfRecords = await queryable.Where(filter).CountAsync().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Trace.Write($"Filter [{ filter}], { ex }");
                return new PagedResponse<TReturn>(Enumerable.Empty<TReturn>());
                
            }
            var projection = queryable
                .Where(filter)
                .OrderBy(request.Ordering())
                .Skip(request.Skip)
                .Take(request.PageSize);
            var results = await projection.ProjectToType<TReturn>().ToListAsync().ConfigureAwait(false);
            if(resultsPostFunc != null)
            {
                results = resultsPostFunc(results);
            }
            return new PagedResponse<TReturn>(results)
            {
                PageNumber = request.Page,
                PageSize = results.Count,
                TotalNumberOfPages = (int)Math.Ceiling((double)totalNumberOfRecords / request.PageSize),
                TotalNumberOfRecords = totalNumberOfRecords,
            };
        }

        protected async Task<int?> IssueIdForIssueApiId(User user, Guid apiId) => (await DbContext.Issue.FirstOrDefaultAsync(x => x.ApiKey == apiId).ConfigureAwait(false))?.IssueId;
    }
}