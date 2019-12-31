using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BihuApiCore.Controllers
{
    public class MvcController:Controller
    {


        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public BaseResponse AddCookies( )
        {
            TimeSpan timeSpan = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,3,0,0).AddDays(1) - DateTime.Now;
            
            //DateTimeOffset expires = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(15));
            //  DateTimeOffset expires = DateTimeOffset.UtcNow.Add(timeSpan);
            //原来是utc  改为正常时间
            DateTimeOffset expires = DateTimeOffset.Now.Add(timeSpan);
            DateTimeOffset expiresNormal= DateTimeOffset.Now.AddDays(1);
            DateTimeOffset expiresHour= DateTimeOffset.Now.AddHours(1);

            HttpContext.Response.Cookies.Append("loginStamp", "asdsad", new CookieOptions
            {
                //Expires =  DateTimeOffset.Now.AddDays(1),
                Expires =  expiresHour,
                HttpOnly = false,
                //Domain = ,
            });

            return BaseResponse.Ok();
        }


    }
}
