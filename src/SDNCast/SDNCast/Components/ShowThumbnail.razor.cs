using Microsoft.AspNetCore.Components;

using SDNCast.Models;

namespace SDNCast.Components
{
    public partial class ShowThumbnail : ComponentBase
    {
        [Parameter]
        public Show Context { get; set; }
    }
}
