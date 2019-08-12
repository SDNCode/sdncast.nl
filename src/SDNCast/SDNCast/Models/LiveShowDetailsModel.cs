using System;

namespace SDNCast.Models
{
    public class LiveShowDetailsModel
    {
        public string LiveShowEmbedUrl { get; set; }

        public string LiveShowHtml { get; set; }

        public DateTime? NextShowDateUtc { get; set; }

        public string AdminMessage { get; set; }
    }
}
