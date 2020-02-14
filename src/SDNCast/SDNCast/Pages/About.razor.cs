
using System.Collections.Generic;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

using SDNCast.Models;

namespace SDNCast.Pages
{
    public partial class About : ComponentBase
    {
        [Inject]
        private IOptions<AppSettings> AppSettings { get; set; }

        protected override void OnInitialized()
        {
            Dictionary<string, CastMemberType> cm = AppSettings.Value.CastMemberTypesDictionary;
        }
    }
}
