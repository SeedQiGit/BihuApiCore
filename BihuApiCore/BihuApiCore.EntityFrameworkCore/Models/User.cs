﻿using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string UserPassWord { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string CertificateNo { get; set; }
        public long Mobile { get; set; }
        public IsVerifyEnum IsVerify { get; set; }

        public long ParentId { get; set; }
        public int LevelNum { get; set; }
        public string LevelCode { get; set; }
    }
}
