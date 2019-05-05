using System.Collections.Generic;

namespace BihuApiCore.Model.Models
{
    public class ManagerModuleViewModel
    {
        public string module_code { get; set; }
        public string text { get; set; }
        public string pater_code { get; set; }
        public string status { get; set; }
        public string action_url { get; set; }
        public decimal orderby { get; set; }

        public List<ManagerModuleViewModel> attrs { get; set; }
        public List<ManagerModuleViewModel> nodes { get; set; }

    }
}
