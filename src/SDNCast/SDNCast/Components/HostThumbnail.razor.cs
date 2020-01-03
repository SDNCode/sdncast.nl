using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public class HostThumbnailBase : ComponentBase
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
    }
}
