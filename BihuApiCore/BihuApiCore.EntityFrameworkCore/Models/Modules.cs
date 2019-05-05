using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class Modules
    {
        public string ModuleCode { get; set; }
        public DateTime Createdtime { get; set; }
        public long CreatedEmp { get; set; }
        public DateTime Updatedtime { get; set; }
        public long UpdatedEmp { get; set; }
        public string ModuleName { get; set; }
        public string ParentCode { get; set; }
        public int ModuleLevel { get; set; }
        public string ActionUrl { get; set; }
        public int ModuleType { get; set; }
        public int IsUserd { get; set; }
        public string Icon { get; set; }
        public int OrderBy { get; set; }
        public int PlatformType { get; set; }
    }
}
