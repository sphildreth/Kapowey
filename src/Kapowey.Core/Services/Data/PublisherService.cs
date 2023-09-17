using System.Diagnostics;
using System.Text.Json;
using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Extensions;
using Kapowey.Core.Common.Extensions.Entities;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;
using Kapowey.Core.Enums;
using Kapowey.Core.Persistance;
using LazyCache;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NodaTime;
using API = Kapowey.Core.Common.Models.API.Entities;
using Image = SixLabors.ImageSharp.Image;

namespace Kapowey.Core.Services.Data
{
    public sealed class PublisherService : ServiceBase, IPublisherService
    {
        private const string _publisherRegionKey = "urn:region:publishers";

        private const string _publisherByPublisherIdKey = "urn:publisher:by:id:{0}";

        private ILogger<PublisherService> Logger { get; }

        private IPublisherCategoryService PublisherCategoryService { get; }

        private IImageService ImageService { get; }

        private IClockProvider SystemClock { get; }

        private IKapoweyHttpContext KapoweyHttpContext { get; }

        public PublisherService(
            AppConfigurationSettings appSettings,
            ILogger<PublisherService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext,
            IPublisherCategoryService publisherCategoryService,
            IClockProvider systemClock,
            IImageService imageService,
            IKapoweyHttpContext kapoweyHttpContext)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
            PublisherCategoryService = publisherCategoryService;
            ImageService = imageService;
            SystemClock = systemClock;
            KapoweyHttpContext = kapoweyHttpContext;
        }

        private Task<Publisher> GetPublisherByPublisherIdAction(int publisherId)
        {
            return DbContext.Publisher.FirstOrDefaultAsync(x => x.PublisherId == publisherId);
        }

        public async Task<IServiceResponse<API.Publisher>> ByIdAsync(User user, Guid apiKey)
        {
            var data = await GetPublisherByApiKey(apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<API.Publisher>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            var result = data.Adapt<API.Publisher>();
            result.ImageUrl = KapoweyHttpContext.MakePathForTypeAndId(UriPathType.Publisher, data.ApiKey.Value, false);
            return new ServiceResponse<API.Publisher>(result);
        }

        public async Task<IPagedResponse<API.PublisherInfo>> ListAsync(User user, PagedRequest request)
        {
            if (!request.IsValid)
            {
                return new PagedResponse<API.PublisherInfo>(new ServiceResponseMessage("Invalid Request", ServiceResponseMessageType.Error));
            }
            return await CreatePagedResponse<Publisher, API.PublisherInfo>(DbContext.Publisher, request, (data) =>
             {
                 if (data != null && data.Any())
                 {
                     Parallel.ForEach(data, publisher =>
                     {
                         publisher.ImageUrl = KapoweyHttpContext.MakePathForTypeAndId(UriPathType.Publisher, publisher.ApiKey.Value, false);
                     });
                 }
                 return data;
             }).ConfigureAwait(false);
        }

        public async Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKey)
        {
            var data = await DbContext.Publisher.FirstOrDefaultAsync(x => x.ApiKey == apiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            DbContext.Publisher.Remove(data);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogWarning($"User `{ user }` deleted: Publisher `{ data }`.");
            return new ServiceResponse<bool>(true);
        }

        public async Task<IServiceResponse<bool>> ModifyAsync(User user, API.Publisher modify)
        {
            var data = await DbContext.Publisher.FirstOrDefaultAsync(x => x.ApiKey == modify.ApiKey).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ modify.ApiKey }]", ServiceResponseMessageType.NotFound));
            }
            data.CountryCode = modify.CountryCode;
            data.Description = modify.Description;
            data.GcdId = modify.GcdId;
            data.PublisherCategoryId = null;
            if (modify?.Category?.ApiKey != null)
            {
                var category = await PublisherCategoryService.ByIdAsync(user, modify.Category.ApiKey.Value).ConfigureAwait(false);
                data.PublisherCategoryId = category.Data.PublisherCategoryId;
            }
            data.ParentPublisherId = null;
            if (modify?.ParentPublisher?.ApiKey != null)
            {
                var parent = await ByIdAsync(user, modify.ParentPublisher.ApiKey.Value).ConfigureAwait(false);
                data.ParentPublisherId = parent.Data.PublisherId;
            }
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = user.Id;
            data.Name = modify.Name;
            data.ShortName = modify.ShortName;
            data.Status = Status.Edited;
            data.Tags = modify.Tags;
            data.Url = modify.Url;
            data.YearBegan = modify.YearBegan;
            data.YearEnd = modify.YearEnd;
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0);
        }

        private async Task<Publisher> SetupPublisher(User user, API.Publisher create)
        {
            var data = new Publisher
            {
                ApiKey = Guid.NewGuid(),
                CountryCode = create.CountryCode,
                Description = create.Description,
                GcdId = create.GcdId,
                CreatedDate = Instant.FromDateTimeUtc(DateTime.UtcNow),
                CreatedUserId = user.Id,
                Name = create.Name,
                ShortName = create.ShortName,
                Status = create.Status,
                Tags = create.Tags,
                Url = create.Url,
                YearBegan = create.YearBegan,
                YearEnd = create.YearEnd
            };
            if (create?.Category?.ApiKey != null)
            {
                var category = await PublisherCategoryService.ByIdAsync(user, create.Category.ApiKey.Value).ConfigureAwait(false);
                data.PublisherCategoryId = category.Data.PublisherCategoryId;
            }
            if (create?.ParentPublisher?.ApiKey != null)
            {
                var parent = await ByIdAsync(user, create.ParentPublisher.ApiKey.Value).ConfigureAwait(false);
                data.ParentPublisherId = parent.Data.PublisherId;
            }
            return data;
        }

        public async Task<IServiceResponse<Guid>> AddAsync(User user, API.Publisher create)
        {
            var data = await SetupPublisher(user, create);
            try
            {
                await DbContext.Publisher.AddAsync(data);
                await DbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                DbContext.Publisher.Remove(data);
                Logger.LogError(ex, "Error Adding Publisher");
                return new ServiceResponse<Guid>(new ServiceResponseMessage("An error occurred", ServiceResponseMessageType.Error));

            }
            return new ServiceResponse<Guid>(data.ApiKey.Value, id: data.PublisherId);
        }

        public Task<API.Publisher> GetPublisherById(int publisherId) 
            => CacheManager.GetOrAddAsync(_publisherByPublisherIdKey.ToCacheKey(publisherId), async () => (await GetPublisherByPublisherIdAction(publisherId)).Adapt<API.Publisher>(), Options);

        private async Task<API.Publisher> GetPublisherByApiKey(Guid? apiKey) => await GetPublisherById(await GetPublisherIdForPublisherApiKey(apiKey.Value).ConfigureAwait(false) ?? 0).ConfigureAwait(false);

        private Task<int?> GetPublisherIdForPublisherApiKey(Guid apiKey)
        {
            return DbContext.Publisher
                            .Where(x => x.ApiKey == apiKey)
                            .Select(x => (int?)x.PublisherId)
                            .FirstOrDefaultAsync();
        }

        public Task<IFileOperationResponse<IImage>> GetPublisherImageAsync(Guid id, int width, int height, EntityTagHeaderValue etag = null)
        {
            return ImageService.GetImageAsyncAction(ImageType.Publisher, API.Publisher.CacheRegionUrn(id), id, width, height, async () =>
            {
                var publisherData = await GetPublisherByApiKey(id).ConfigureAwait(false);
                if (publisherData == null)
                {
                    return null;
                }
                var publisher = publisherData.Adapt<API.Publisher>();
                IImage image = new Common.Interfaces.Image()
                {
                    CreatedDate = publisher.CreatedDate.Value
                };
                var publisherImageFilename = ImageService.ImagePath(ImageType.Publisher, publisher.ApiKey.Value);
                try
                {
                    if (File.Exists(publisherImageFilename))
                    {
                        image.Bytes = await File.ReadAllBytesAsync(publisherImageFilename).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error Reading Image File [{publisherImageFilename}]");
                }
                return image;
            }, etag);
        }

        public async Task<IServiceResponse<ImportResult>> Import(API.ApiApplication application, User user)
        {
            if(application?.Status == Status.Inactive || application?.Status == Status.Locked)
            {
                return new ServiceResponse<ImportResult>(new ServiceResponseMessage("Invalid Application", ServiceResponseMessageType.Authentication));
            }
            if(!user.IsManager())
            {
                return new ServiceResponse<ImportResult>(new ServiceResponseMessage("Access Denied", ServiceResponseMessageType.Authentication));
            }
            var result = new ServiceResponse<ImportResult>();
            var resultData = new ImportResult();
            var sw = Stopwatch.StartNew();
            var importFolder = Path.Combine(AppSettings.StorageFolder, "dataExchange", "publishers");
            if (!Directory.Exists(importFolder))
            {
                Directory.CreateDirectory(importFolder);
                Logger.LogInformation($"Created Data Exchange Folder [{ importFolder }]");
            }
            var now = Instant.FromDateTimeOffset(SystemClock.UtcNow);
            foreach (var importFile in Directory.GetFiles(importFolder, "*.json"))
            {
                using (FileStream openStream = File.OpenRead(importFile))
                {
                    var datasFromFile = await JsonSerializer.DeserializeAsync<List<API.Publisher>>(openStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var datasFromFilePublisherNames = datasFromFile.Select(x => x.Name.ToUpper()).ToArray();
                    var datasToImport = (from dff in datasFromFilePublisherNames
                                         join p in DbContext.Publisher on dff equals p.Name.ToUpper() into pg
                                         from p in pg.DefaultIfEmpty()
                                         where p == null
                                         select dff).ToArray();
                    if (datasToImport != null && datasToImport.Any())
                    {
                        var publishers = new List<Publisher>();
                        foreach (var dataToImport in datasToImport)
                        {
                            var tags = new List<string>();
                            var dataFromFile = datasFromFile.FirstOrDefault(x => x.Name.ToUpper() == dataToImport);
                            dataFromFile.Name = dataFromFile.Name?.Clean()?.Left(500);
                            var existsByName = publishers.FirstOrDefault(x => x.Name.ToUpper() == dataFromFile.Name.ToUpper());
                            if (existsByName != null)
                            {
                                dataFromFile = datasFromFile.Where(x => x.Name.ToUpper() == dataToImport || x.Name.ToUpper() == dataFromFile.Name.ToUpper()).Skip(1).Take(1).FirstOrDefault();
                                tags.Add("import-adjust-name");
                                if (dataFromFile.YearBegan.HasValue && dataFromFile.YearBegan.Value > 0)
                                {
                                    var oldName = dataFromFile.Name;
                                    dataFromFile.Name = $"{ dataFromFile.Name} ({ dataFromFile.YearBegan})";
                                    tags.Add("import-adjust-name-year");
                                    resultData.Messages.Add($"Adjusted Name from [{ oldName }] to [{ dataFromFile.Name }]");
                                }
                                else if (dataFromFile.GcdId.HasValue)
                                {
                                    var oldName = dataFromFile.Name;
                                    dataFromFile.Name = $"{ dataFromFile.Name} [{ dataFromFile.GcdId}]";
                                    tags.Add("import-adjust-name-gcdid");
                                    resultData.Messages.Add($"Adjusted Name from [{ oldName }] to [{ dataFromFile.Name }]");
                                }
                            }
                            dataFromFile.ShortName = (dataFromFile.ShortName?.Clean() ?? dataFromFile.Name)?.ToAlphanumericName()?.Left(10)?.ToUpper();
                            dataFromFile.CountryCode = dataFromFile.CountryCode?.Clean(false)?.Left(3);
                            dataFromFile.Url = dataFromFile.Url?.Clean(false)?.Left(1000)?.ToLower();
                            dataFromFile.Description = dataFromFile.Description?.Clean(false);
                            dataFromFile.Status = Status.Imported;
                            dataFromFile.CreatedDate = now;
                            dataFromFile.CreatedUserId = user.Id;
                            if (dataFromFile.GcdId.HasValue)
                            {
                                tags.Add("import-gcd");
                            }
                            dataFromFile.Tags = tags.ToArray();
                            publishers.Add(await SetupPublisher(user, dataFromFile));
                        }
                        if(publishers.Any(x => string.IsNullOrWhiteSpace(x.Name)))
                        {
                            foreach (var dd in publishers.Where(x => string.IsNullOrWhiteSpace(x.Name)))
                            {
                                resultData.Errors.Add($"Invalid Names found [{ dd }]");
                            }
                            result.SetData(resultData);
                            return result;
                        }
                        var duplicateByName = publishers.GroupBy(x => x.Name);
                        if(duplicateByName.Any(x => x.Count() > 1))
                        {
                            foreach(var dd in duplicateByName.Where(x => x.Count() > 1))
                            {
                                resultData.Errors.Add($"Duplicate Names found [{ dd.Key }]");
                            }
                            result.SetData(resultData);
                            return result;
                        }
                        try
                        {
                            DbContext.Publisher.AddRange(publishers);
                            var saveResult = await DbContext.SaveChangesAsync().ConfigureAwait(false);
                            resultData.TotalRecordsImported = publishers.Count;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Error Importing Publishers : Adding to database");
                        }
                        resultData.TotalFilesImported++;
                    }
                    openStream.Close();
                }
                try
                {
                    File.Move(importFile, Path.ChangeExtension(importFile, $".json.{ now.ToUnixTimeMilliseconds()}.imported"));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error Importing Publishers");
                }
            }
            sw.Stop();
            resultData.TotalDuration = sw.ElapsedMilliseconds;
            result.SetData(resultData);
            return result;
        }
    }
}