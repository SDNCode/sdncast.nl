using System.Collections.Generic;

using Microsoft.AspNetCore.Components;

using SDNCast.Models;

namespace SDNCast.Components
{
    public partial class MemberList : ComponentBase
    {
        [Parameter]
        public Dictionary<string, CastMember> CastMemberDictionary { get; set; }
    }
}
