using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SDNCast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SDNCast.Services
{
    public class FileSystemLiveShowDetailsService : ILiveShowDetailsService
    {
        private static readonly string CacheKey = nameof(FileSystemLiveShowDetailsService);
        private static readonly string FileName = "ShowDetails.json";

        private readonly IMemoryCache _cache;
        private readonly string _filePath;

        public FileSystemLiveShowDetailsService(IWebHostEnvironment webHostEnvironment, IMemoryCache cache)
        {
            _cache = cache;
            _filePath = Path.Combine(webHostEnvironment.ContentRootPath, FileName);
        }

        public async Task<LiveShowDetailsModel> LoadAsync()
        {
            var result = _cache.Get<LiveShowDetailsModel>(CacheKey);

            if (result == null)
            {
                result = await LoadFromFile();

                _cache.Set(CacheKey, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.MaxValue
                });
            }

            return result;
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
