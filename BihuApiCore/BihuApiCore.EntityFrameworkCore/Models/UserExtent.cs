using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class UserExtent
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserHobby { get; set; }
        public string UserOccupation { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
