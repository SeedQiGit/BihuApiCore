using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class ZsPiccCall
    {
        public long Id { get; set; }
        public long UserAgentId { get; set; }
        public string UserName { get; set; }
        public int CallState { get; set; }
        public long CallId { get; set; }
        public string CallPassword { get; set; }
        public string CallNumber { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string CallExtNumber { get; set; }
        public string DirectNumber { get; set; }
    }
}
