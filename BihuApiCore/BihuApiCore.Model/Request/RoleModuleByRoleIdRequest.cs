using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Model.Request
{
    public class RoleModuleByRoleIdRequest
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public long CompId { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public long RoleId { get; set; }
    }
}
