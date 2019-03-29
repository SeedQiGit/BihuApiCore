using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class UserConfig
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int UserLevel { get; set; }
        public int UserGrade { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
