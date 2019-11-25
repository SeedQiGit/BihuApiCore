using System;

namespace BihuApiCore.Infrastructure.Helper
{
    public static class TimeHelper
    {
        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static string GetTimeStamp() 
        { 
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0); 
            return Convert.ToInt64(ts.TotalSeconds).ToString(); 
        } 

        /// <summary> 
        /// 根据utc时间获取本机时间 
        /// </summary> 
        /// <returns></returns> 
        public static DateTime UtcToCurrent(this DateTime utc) 
        { 
            TimeSpan ts = DateTime.UtcNow -  DateTime.Now; 
            return utc-ts; 
        } 


        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string timeStamp)
        {
            timeStamp = timeStamp.Substring(0, 10);
            double timestamp = Convert.ToInt64(timeStamp);
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();       
            return dateTime;
        }

        /// <summary>
        /// 获取当前时间的字符串标准格式  yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string NowTimeStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
