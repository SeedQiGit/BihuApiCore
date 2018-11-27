using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class DataExcel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string CallPassword { get; set; }
        public string CallNumber { get; set; }
        public string CallExtNumber { get; set; }
        public string DirectNumber { get; set; }
    }
}
