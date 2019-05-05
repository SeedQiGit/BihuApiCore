using System;
using System.Collections.Generic;

namespace BihuApiCore.EntityFrameworkCore.Models
{
    public partial class RoleModuleRelation
    {
        public long Id { get; set; }
        public DateTime Createdtime { get; set; }
        public long CreatedEmp { get; set; }
        public DateTime Updatedtime { get; set; }
        public long UpdatedEmp { get; set; }
        public long RoleId { get; set; }
        public string ModuleCode { get; set; }
    }
}
