using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class Companies
    {
        public long CompId { get; set; }
        public DateTime Createdtime { get; set; }
        public long CreatedEmp { get; set; }
        public DateTime Updatedtime { get; set; }
        public long UpdatedEmp { get; set; }
        public string CompName { get; set; }
        public bool IsUsed { get; set; }
        public int CompanyType { get; set; }
        public int PayType { get; set; }
        public long TopAgentId { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public int PiccAccount { get; set; }
        public int ZsType { get; set; }
        public long ParentCompId { get; set; }
        public int LevelNum { get; set; }
        public string LevelCode { get; set; }
        public string ClientName { get; set; }
    }
}
