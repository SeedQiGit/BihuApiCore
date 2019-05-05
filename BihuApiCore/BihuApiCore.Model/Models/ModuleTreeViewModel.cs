using BihuApiCore.Model.Enums;
using System.Collections.Generic;

namespace BihuApiCore.Model.Models
{
    public class ModuleTreeViewModel
    {
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public string ParentCode { get; set; }
        public string Status { get; set; }
        public string ActionUrl { get; set; }
        public decimal OrderBy { get; set; }
        /// <summary>
        /// 1菜单，2按钮，3方法
        /// </summary>
        public EnumModuleType ModuleType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EnumPlatformType Plat { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<ModuleTreeViewModel> Nodes { get; set; }
    }
}
