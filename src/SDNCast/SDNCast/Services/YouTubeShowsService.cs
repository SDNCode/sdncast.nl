﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using SDNCast.Models;

namespace SDNCast.Services
{
    public class YouTubeShowsService : IShowsService
    {
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;

        public YouTubeShowsService(
            IOptions<AppSettings> appSettings,
            IMemoryCache memoryCache,
            IWebHostEnvironment env)
        {
            _appSettings = appSettings.Value;
            _cache = memoryCache;
            _env = env;
        }

        public async Task<ShowList> GetRecordedShowsAsync(ClaimsPrincipal user, bool disableCache, string playlist)
        {
            if (string.IsNullOrEmpty(_appSettings.YouTubeApiKey))
            {
                return new ShowList { PreviousShows = DesignData.Shows };
            }

            if (user.Identity.IsAuthenticated && disableCache)
            {
                return await GetShowsList(playlist);
            }

            string CacheKey = playlist;

            ShowList showList = _cache.Get<ShowList>(CacheKey);

            if (showList == null)
            {
                showList = await GetShowsList(playlist);

                bool isDevelopment = _env.EnvironmentName.Equals("Development", StringComparison.InvariantCultureIgnoreCase);
                TimeSpan timespan = isDevelopment ? TimeSpan.FromMinutes(360) : TimeSpan.FromDays(1);

                _cache.Set(CacheKey, showList, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timespan
                });
            }

            return showList;
        }

        private async Task<ShowList> GetShowsList(string playlist)
        {
            using var client = new YouTubeService(new BaseClientService.Initializer
            {
                ApplicationName = _appSettings.YouTubeApplicationName,
                ApiKey = _appSettings.YouTubeApiKey
            });

            var listRequest = client.PlaylistItems.List("snippet");
            listRequest.PlaylistId = playlist;
            listRequest.MaxResults = 5 * 3; // 5 rows of 3 episodes

            var playlistItems = await listRequest.ExecuteAsync();

            var result = new ShowList
            {
                PreviousShows = playlistItems.Items.Select(item => new Show
                {
                    Provider = "YouTube",
                    ProviderId = item.Snippet.ResourceId.VideoId,
                    Title = GetUsefulBitsFromTitle(item.Snippet.Title),
                    Description = item.Snippet.Description,
                    ThumbnailUrl = item.Snippet.Thumbnails.High.Url,
                    Url = GetVideoUrl(item.Snippet.ResourceId.VideoId, item.Snippet.PlaylistId, item.Snippet.Position ?? 0)
                }).ToList()
            };

            foreach (var show in result.PreviousShows)
            {
                show.ShowDate = await GetVideoPublishDate(client, show.ProviderId);
                show.LiveBroadcastContent = await GetVideoLiveBroadcastContent(client, show.ProviderId);
            }

            if (!string.IsNullOrEmpty(playlistItems.NextPageToken))
            {
                result.MoreShowsUrl = GetPlaylistUrl(playlist);
            }

            return result;
        }

        private async Task<DateTimeOffset> GetVideoPublishDate(YouTubeService client, string videoId)
        {
            var videoRequest = client.Videos.List("snippet");
            videoRequest.Id = videoId;
            videoRequest.MaxResults = 1;

            var video = await videoRequest.ExecuteAsync();
            var rawDate = video.Items[0].Snippet.PublishedAtRaw;

            return DateTimeOffset.Parse(rawDate, null, DateTimeStyles.RoundtripKind);
        }

        private async Task<string> GetVideoLiveBroadcastContent(YouTubeService client, string videoId)
        {
            var videoRequest = client.Videos.List("snippet");
            videoRequest.Id = videoId;
            videoRequest.MaxResults = 1;

            var video = await videoRequest.ExecuteAsync();
            var liveBroadcastContent = video.Items[0].Snippet.LiveBroadcastContent;

            return liveBroadcastContent;
        }

        private static string GetUsefulBitsFromTitle(string title)
        {
            if (string.IsNullOrEmpty(title)) return string.Empty;

            if (title.Count(c => c == '-') < 1)
            {
                return title;
            }

            var lastHyphen = title.IndexOf('-');
            if (lastHyphen >= 0)
            {
                var result = title.Substring(lastHyphen + 1).Trim();
                return result;
            }

            return string.Empty;
        }

        private static string GetVideoUrl(string id, string playlistId, long itemIndex)
        {
            var encodedId = UrlEncoder.Default.Encode(id);
            var encodedPlaylistId = UrlEncoder.Default.Encode(playlistId);
            var encodedItemIndex = UrlEncoder.Default.Encode(itemIndex.ToString());

            return $"https://www.youtube.com/watch?v={encodedId}&list={encodedPlaylistId}&index={encodedItemIndex}";
        }

        private static string GetPlaylistUrl(string playlistId)
        {
            var encodedPlaylistId = UrlEncoder.Default.Encode(playlistId);

            return $"https://www.youtube.com/playlist?list={encodedPlaylistId}";
        }

        private static class DesignData
        {
            private static readonly TimeSpan _pdtOffset = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time").BaseUtcOffset;

            public static readonly string LiveShow = null;

            public static readonly IList<Show> Shows = new List<Show>
            {
                new Show
                {
                    ShowDate = new DateTimeOffset(2015, 7, 21, 9, 30, 0, _pdtOffset),
                    Title = "SDN Cast - July 21st 2015",
                    Provider = "YouTube",
                    ProviderId = "7O81CAjmOXk",
                    ThumbnailUrl = "http://img.youtube.com/vi/7O81CAjmOXk/mqdefault.jpg",
                    Url = "https://www.youtube.com/watch?v=7O81CAjmOXk&index=1&list=PL0M0zPgJ3HSftTAAHttA3JQU4vOjXFquF"
                },
                new Show
                {
                    ShowDate = new DateTimeOffset(2015, 7, 14, 15, 30, 0, _pdtOffset),
                    Title = "SDN Cast - July 14th 2015",
                    Provider = "YouTube",
                    ProviderId = "bFXseBPGAyQ",
                    ThumbnailUrl = "http://img.youtube.com/vi/bFXseBPGAyQ/mqdefault.jpg",
                    Url = "https://www.youtube.com/watch?v=bFXseBPGAyQ&index=2&list=PL0M0zPgJ3HSftTAAHttA3JQU4vOjXFquF"
                },

                new Show
                {
                    ShowDate = new DateTimeOffset(2015, 7, 7, 15, 30, 0, _pdtOffset),
                    Title = "SDN Cast - July 7th 2015",
                    Provider = "YouTube",
                    ProviderId = "APagQ1CIVGA",
                    ThumbnailUrl = "http://img.youtube.com/vi/APagQ1CIVGA/mqdefault.jpg",
                    Url = "https://www.youtube.com/watch?v=APagQ1CIVGA&index=3&list=PL0M0zPgJ3HSftTAAHttA3JQU4vOjXFquF"
                },
                new Show
                {
                    ShowDate = DateTimeOffset.Now.AddDays(-28),
                    Title = "SDN Cast - July 21st 2015",
                    Provider = "YouTube",
                    ProviderId = "7O81CAjmOXk",
                    ThumbnailUrl = "http://img.youtube.com/vi/7O81CAjmOXk/mqdefault.jpg",
                    Url = "https://www.youtube.com/watch?v=7O81CAjmOXk&index=1&list=PL0M0zPgJ3HSftTAAHttA3JQU4vOjXFquF"
                },
            };
        }
    }
}
