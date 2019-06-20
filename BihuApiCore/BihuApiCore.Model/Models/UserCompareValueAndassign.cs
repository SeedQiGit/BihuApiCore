using System;
using System.Collections.Generic;
using System.Text;
using BihuApiCore.EntityFrameworkCore;

namespace BihuApiCore.Model.Models
{
    public class UserCompareValueAndassign
    {
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public IsVerifyEnum IsVerify { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
