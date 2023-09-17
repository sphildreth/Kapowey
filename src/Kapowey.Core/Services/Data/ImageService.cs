using System.Diagnostics;
using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Helpers;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Enums;
using Kapowey.Core.Imaging;
using Kapowey.Core.Persistance;
using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NodaTime;
using Image = SixLabors.ImageSharp.Image;

namespace Kapowey.Core.Services.Data
{
    public class ImageService : ServiceBase, IImageService
    {
        protected IHttpEncoder HttpEncoder { get; }

        private ILogger<ImageService> Logger { get; }

        public ImageService(
            AppConfigurationSettings appSettings,
            ILogger<ImageService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext,
            IHttpEncoder httpEncoder)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
            HttpEncoder = httpEncoder;

            CheckMakeStoragePaths();
        }

        private void CheckMakeStoragePaths()
        {
            var paths = new List<string>
            {
                $@"{ AppSettings.StorageFolder }\images\collections\",
                $@"{ AppSettings.StorageFolder }\images\issues\",
                $@"{ AppSettings.StorageFolder }\images\publishers\",
                $@"{ AppSettings.StorageFolder }\images\series\",
                $@"{ AppSettings.StorageFolder }\images\users\"
            };
            foreach (var p in paths)
            {
                var path = Path.Combine(AppSettings.StorageFolder, p);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Logger.LogInformation($"Created Storage Folder Image Path [{ path }]");
                }
            }
        }

        protected FileOperationResponse<IImage> GenerateFileOperationResult(Guid id, IImage image, EntityTagHeaderValue etag = null, string contentType = "image/png")
        {
            var imageEtag = EtagHelper.GenerateETag(HttpEncoder, image.Bytes);
            if (EtagHelper.CompareETag(HttpEncoder, etag, imageEtag))
            {
                return new FileOperationResponse<IImage>(new ServiceResponseMessage(NotModifiedMessage, ServiceResponseMessageType.NotModified));
            }
            if (image?.Bytes?.Any() != true)
            {
                return new FileOperationResponse<IImage>(new ServiceResponseMessage($"ImageById Not Set [{id}]", ServiceResponseMessageType.NotFound));
            }
            return new FileOperationResponse<IImage>(image, new ServiceResponseMessage(ServiceResponseMessageType.Ok))
            {
                ContentType = contentType,
                LastModified = image.CreatedDate,
                ETag = imageEtag
            };
        }

        public string ImagePath(ImageType type, Guid apiKey) => ImagePath(type, apiKey.ToString());

        public string ImagePath(ImageType type, string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return null;
            }
            var ak = apiKey.ToString().ToLower();
            var part1 = ak.Substring(0, 2);
            var part2 = ak.Substring(2,2);
            switch (type)
            {
                case ImageType.UserAvatar:
                    return MakeImagePath(AppSettings, $@"images\users\{part1}\{part2}", $"{ ak }.png");

                case ImageType.Publisher:
                    return MakeImagePath(AppSettings, $@"images\publishers\{part1}\{part2}", $"{ ak }.png");
            }
            throw new NotImplementedException();
        }

        public async Task<IFileOperationResponse<IImage>> GetImageAsyncAction(ImageType imageType, string regionUrn, Guid id, int width, int height, Func<Task<IImage>> action, EntityTagHeaderValue etag = null)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var sizeHash = width + height;
                var result = await CacheManager.GetOrAddAsync($"urn:{imageType}_by_id_operation:{id}:{sizeHash}", action, Options).ConfigureAwait(false);
                if (result?.Bytes == null)
                {
                    result = DefaultImageForImageType(AppSettings, imageType);
                }
                var data = GenerateFileOperationResult(id, result, etag);
                if (data.ETag == etag && etag != null)
                {
                    return new FileOperationResponse<IImage>(new ServiceResponseMessage(NotModifiedMessage, ServiceResponseMessageType.NotModified));
                }
                if (data?.Data?.Bytes != null)
                {
                    var resized = ImageHelper.ResizeImage(data?.Data?.Bytes, width, height, true);
                    if (resized != null)
                    {
                        data.Data.Bytes = resized.Item2;
                        data.ETag = EtagHelper.GenerateETag(HttpEncoder, data.Data.Bytes);
                        data.LastModified = Instant.FromDateTimeUtc(DateTime.UtcNow);
                        if (resized.Item1)
                        {
                            Logger.LogInformation($"{imageType}: Resized [{id}], Width [{ width}], Height [{ height}]");
                        }
                    }
                    else
                    {
                        Logger.LogInformation($"{imageType}: Image [{id}] Request returned Null Image");
                    }
                }

                sw.Stop();
                return new FileOperationResponse<IImage>(data.Data, data.Messages)
                {
                    ETag = data.ETag,
                    LastModified = data.LastModified,
                    ContentType = data.ContentType
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GetImageFileOperation Error, Type [{imageType}], id [{id}]");
            }

            return new FileOperationResponse<IImage>(new ServiceResponseMessage("System Error", ServiceResponseMessageType.Error));
        }

        private static IImage DefaultImageForImageType(AppConfigurationSettings appSettings, ImageType type)
        {
            return type switch
            {
                ImageType.UserAvatar => MakeImageFromFile($@"{ appSettings.WebRootPath }\images\user.png"),
                ImageType.Publisher => MakeImageFromFile($@"{ appSettings.WebRootPath }\images\publisher.png"),
                _ => throw new NotImplementedException(),
            };
        }

        private static IImage MakeImageFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return new Common.Interfaces.Image();
            }
            var bytes = File.ReadAllBytes(filename);
            return new Common.Interfaces.Image
            {
                Bytes = bytes,
                CreatedDate = Instant.FromDateTimeUtc(DateTime.UtcNow)
            };
        }

        private static string MakeImagePath(AppConfigurationSettings appSettings, string typePath, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }
            var tp = Path.Combine(appSettings.StorageFolder, typePath);
            if (!Directory.Exists(tp))
            {                
                Directory.CreateDirectory(tp);
            }
            var path = Path.Combine(tp, filename);
            if(!File.Exists(path))
            {
                Trace.WriteLine($"Unable To Find File [{ filename }], Path [{ tp }] ContentPath [{ appSettings.StorageFolder }]");
            }
            return path;
        }
    }
}