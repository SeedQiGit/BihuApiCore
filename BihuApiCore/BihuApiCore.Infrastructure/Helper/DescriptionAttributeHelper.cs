using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BihuApiCore.Infrastructure.Helper
{
    public static class DescriptionAttributeHelper
    {
        
        /// <summary>
        /// 获取某一个属性的描述DescriptionAttribute
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this PropertyInfo property)
        {
            object[] objs =property.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length == 0) return "";
            DescriptionAttribute da = (DescriptionAttribute)objs[0];
            return da.Description;
        }
    }
}
