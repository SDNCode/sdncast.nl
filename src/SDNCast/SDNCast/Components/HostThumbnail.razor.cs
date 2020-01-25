using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public partial class HostThumbnail : ComponentBase
    {
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Avatar { get; set; }

        [Parameter]
        public string BioLink { get; set; }

        [Parameter]
        public string TwitterAccount { get; set; }

        [Parameter]
        public string TwitchAccount { get; set; }

        [Parameter]
        public string YouTubeAccount { get; set; }
    }
}
