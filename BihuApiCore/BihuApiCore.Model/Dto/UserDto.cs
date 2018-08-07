using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Model.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string UserPassWord { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string CertificateNo { get; set; }
        public long Mobile { get; set; }
        public int IsVerify { get; set; }
    }
}
