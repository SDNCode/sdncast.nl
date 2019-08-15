using System;

namespace SDNCast.Models
{
    public class Show
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public string Title { get; set; }
        public bool HasTitle => !string.IsNullOrEmpty(Title);
        public string Description { get; set; }
        public DateTimeOffset ShowDate { get; set; }
        public bool IsNew => !IsInFuture && (DateTimeOffset.Now - ShowDate).TotalDays <= 7;
        public bool IsInFuture => ShowDate > DateTimeOffset.Now;
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string LiveBroadcastContent { get; set; }
    }
}
