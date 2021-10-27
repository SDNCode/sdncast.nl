using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;

using SDNCast.Models;

namespace SDNCast.Services
{
    public class AzureStorageLiveShowDetailsService : ILiveShowDetailsService
    {
        private static readonly string CacheKey = nameof(AzureStorageLiveShowDetailsService);

        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;

        public AzureStorageLiveShowDetailsService(IOptions<AppSettings> appSettings, IMemoryCache cache, IWebHostEnvironment env)
        {
            _appSettings = appSettings.Value;
            _cache = cache;
            _env = env;
        }

        public void ClearCache()
        {
            _cache.Remove(CacheKey);
        }

        public async Task<LiveShowDetailsModel> LoadAsync()
        {
            LiveShowDetailsModel liveShowDetails = _cache.Get<LiveShowDetailsModel>(CacheKey);

            if (liveShowDetails == null)
            {
                liveShowDetails = await LoadFromAzureStorage();

                // if liveShowDetails == null -> Initialize
                if (liveShowDetails == null)
                {
                    liveShowDetails = new LiveShowDetailsModel();
                }

                bool isDevelopment = _env.EnvironmentName.Equals("Development", StringComparison.InvariantCultureIgnoreCase);
                TimeSpan timespan = isDevelopment ? TimeSpan.FromMinutes(360) : TimeSpan.FromDays(1);

                _cache.Set(CacheKey, liveShowDetails, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timespan
                });
            }

            return liveShowDetails;
        }

        public async Task SaveAsync(LiveShowDetailsModel liveShowDetails)
        {
            if (liveShowDetails == null)
            {
                throw new ArgumentNullException(nameof(liveShowDetails));
            }

            await SaveToAzureStorage(liveShowDetails);

            // Update the cache
            _cache.Set(CacheKey, liveShowDetails, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.MaxValue
            });
        }

        private async Task<LiveShowDetailsModel> LoadFromAzureStorage()
        {
            var container = GetStorageContainer();

            if (!await container.ExistsAsync())
            {
                return null;
            }

            var blockBlob = container.GetBlockBlobReference(_appSettings.AzureStorageBlobName);
            if (!await blockBlob.ExistsAsync())
            {
                return null;
            }

            var fileContents = await blockBlob.DownloadTextAsync();

            return JsonConvert.DeserializeObject<LiveShowDetailsModel>(fileContents);
        }

        private async Task SaveToAzureStorage(LiveShowDetailsModel liveShowDetails)
        {
            var container = GetStorageContainer();

            await container.CreateIfNotExistsAsync();

            var blockBlob = container.GetBlockBlobReference(_appSettings.AzureStorageBlobName);

            var fileContents = JsonConvert.SerializeObject(liveShowDetails);

            // var started = Timing.GetTimestamp();
            await blockBlob.UploadTextAsync(fileContents);
        }

        private CloudBlobContainer GetStorageContainer()
        {
            var account = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionString);
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(_appSettings.AzureStorageContainerName);

            return container;
        }
    }
}
