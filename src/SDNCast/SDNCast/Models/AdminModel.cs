using System;
using System.ComponentModel.DataAnnotations;

namespace SDNCast.Models
{
    public class AdminModel
    {
        [Display(Name = "Live Show Embed URL", Description = "URL for embedding the live show")]
        [DataType(DataType.Url)]
        public string LiveShowEmbedUrl { get; set; }

        [Display(Name = "Live Show HTML", Description = "HTML content for the live show")]
        [DataType(DataType.MultilineText)]
        public string LiveShowHtml { get; set; }

        [Display(Name = "Next Show Date/time", Description = "Exact date and time of the next live show in Pacific Time")]
        [DateAfterNow(TimeZoneId = "Central Europe Standard Time")]
        public DateTime? NextShowDatePst { get; set; }

        public string NextShowDatePstString
        {
            get { return NextShowDatePst.Value.ToString("MM/dd/yyyy HH:mm"); }
            set
            {
                NextShowDatePst = DateTime.Parse(value);
            }
        }

        [Display(Name = "Standby Message", Description = "Message to show on home page during show standby")]
        public string AdminMessage { get; set; }

        public string NextShowDateSuggestionCetPM { get; set; }

        public AppSettings AppSettings { get; set; }

        public string EnvironmentName { get; set; }
    }
}
