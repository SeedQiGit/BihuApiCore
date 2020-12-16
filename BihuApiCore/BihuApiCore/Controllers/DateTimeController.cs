using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class DateTimeController : BaseController
    {
        private readonly ILogger<DateTimeController> _logger;

        public DateTimeController(ILogger<DateTimeController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType(typeof(BaseResponse), 1)]
        [HttpGet("SpringFestival")]
        public async Task<BaseResponse> SpringFestival([FromQuery]int year)
        {    
            DateTime myDT = new DateTime(year, 1, 1, new System.Globalization.ChineseLunisolarCalendar());
            return Ok(myDT);
        }

    }
}
