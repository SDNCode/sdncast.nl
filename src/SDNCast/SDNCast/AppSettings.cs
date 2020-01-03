namespace SDNCast
{
    public class AppSettings
    {
        public string YouTubeApplicationName { get; set; }
        public string YouTubeApiKey { get; set; }
        public string YouTubeCastPlaylistId { get; set; }
        public string YouTubeLiveEventsPlaylistId { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string AzureStorageBlobName { get; set; }
        public string AzureStorageContainerName { get; set; }
        public string[] PlaylistItems { get; set; }
    }
}
