﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace sdncast.nl
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
    }
}
