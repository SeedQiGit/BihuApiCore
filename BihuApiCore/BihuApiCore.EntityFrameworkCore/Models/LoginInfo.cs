using System;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public class LoginInfo
    {
        public string Id { get; set; }
        public string OsName { get; set; }
        public string OsVerson { get; set; }
        public string BrowerName { get; set; }
        public string BrowerVersion { get; set; }
        public string BrowerMajor { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}
