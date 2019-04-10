// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using sdncast.nl.Models;
using sdncast.nl.Services;
using Microsoft.AspNetCore.Mvc;

namespace sdncast.nl.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ILiveShowDetailsService _liveShowDetails;

        public CalendarController(ILiveShowDetailsService liveShowDetails)
        {
            _liveShowDetails = liveShowDetails;
        }

        [HttpGet("/ical")]
        [Produces("text/calendar")]
        public async Task<LiveShowDetails> GetiCal()
        {
            var liveShowDetails = await _liveShowDetails.LoadAsync();

            return liveShowDetails;
        }
    }
}
