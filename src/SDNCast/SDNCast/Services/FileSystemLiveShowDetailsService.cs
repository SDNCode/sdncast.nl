using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using SDNCast.Models;

namespace SDNCast.Services
{
    public class FileSystemLiveShowDetailsService : ILiveShowDetailsService
    {
        private static readonly string CacheKey = nameof(FileSystemLiveShowDetailsService);
        private static readonly string FileName = "ShowDetails.json";

        private readonly IMemoryCache _cache;
        private readonly string _filePath;
        private readonly IWebHostEnvironment _env;

        public FileSystemLiveShowDetailsService(
            IWebHostEnvironment webHostEnvironment,
            IMemoryCache cache,
            IWebHostEnvironment env)
        {
            _cache = cache;
            _filePath = Path.Combine(webHostEnvironment.ContentRootPath, FileName);
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
                liveShowDetails = await LoadFromFile();

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
                return;
            }

            var fileContents = JsonConvert.SerializeObject(liveShowDetails);
            using (var fileWriter = new StreamWriter(new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true)))
            {
                await fileWriter.WriteAsync(fileContents);
            }

            _cache.Remove(CacheKey);
        }

        private async Task<LiveShowDetailsModel> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }

            string fileContents;
            using (var fileReader = new StreamReader(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read, 4096, useAsync: true)))
            {
                fileContents = await fileReader.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<LiveShowDetailsModel>(fileContents);
        }
    }
}
