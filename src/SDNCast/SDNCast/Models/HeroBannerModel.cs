using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SDNCast.Models
{
    public class HeroBannerModel
    {
        private static readonly string _dateTimeFormat = "yyyyMMddTHHmmssZ";
        private static readonly string _googleCalendarText = WebUtility.UrlEncode("SDN Cast");
        private static readonly string _googleCalendarLocation = WebUtility.UrlEncode("https://sdncast.nl/");

        public bool IsOnAir => !HasAdminMessage && (IsLiveShowEmbedded || !string.IsNullOrEmpty(LiveShowHtml));

        public string LiveShowEmbedUrl { get; set; }

        public string LiveShowHtml { get; set; }

        public bool IsLiveShowEmbedded => !string.IsNullOrEmpty(LiveShowEmbedUrl);

        public DateTime? NextShowDateUtc { get; set; }

        public bool NextShowScheduled => NextShowDateUtc.HasValue;

        public string AdminMessage { get; set; }

        public bool HasAdminMessage => !string.IsNullOrEmpty(AdminMessage);

        //public IList<Show> PreviousShows { get; set; }

        //public bool ShowPreviousShows => PreviousShows.Count > 0;

        public string MoreShowsUrl { get; set; }

        public bool ShowMoreShowsUrl => !string.IsNullOrEmpty(MoreShowsUrl);

        public string AddToGoogleUrl
        {
            get
            {
                // reference: http://stackoverflow.com/a/21653600/22941
                var from = WebUtility.UrlEncode(NextShowDateUtc?.ToString(_dateTimeFormat));
                var to = WebUtility.UrlEncode(NextShowDateUtc?.AddMinutes(30).ToString(_dateTimeFormat));

                return $"https://www.google.com/calendar/render?action=TEMPLATE&text={_googleCalendarText}&dates={from}/{to}&details={_googleCalendarLocation}&location={_googleCalendarLocation}&sf=true&output=xml";
            }
        }
    }
}
