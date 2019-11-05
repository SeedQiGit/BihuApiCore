using System;

namespace BihuApiCore.Infrastructure.Helper
{
    public class IdCardHelper
    {
        public static BirthdayAgeSex GetBirthdayAgeSex(string identityCard)
        {
            if (string.IsNullOrEmpty(identityCard))
            {
                return null;
            }
            else
            {
                if (identityCard.Length != 15 && identityCard.Length != 18)//身份证号码只能为15位或18位其它不合法
                {
                    return null;
                }
            }

            BirthdayAgeSex entity = new BirthdayAgeSex();
            string strSex = string.Empty;
            if (identityCard.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                entity.Birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" + identityCard.Substring(12, 2);
                strSex = identityCard.Substring(14, 3);
            }
            if (identityCard.Length == 15)
            {
                entity.Birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" + identityCard.Substring(10, 2);
                strSex = identityCard.Substring(12, 3);
            }
            
            entity.Age = CalculateAge(entity.Birthday);//根据生日计算年龄
            if (int.Parse(strSex) % 2 == 0)//性别代码为偶数是女性奇数为男性
            {
                entity.Sex = "女";
            }
            else
            {
                entity.Sex = "男";
            }
            return entity;
        }

        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDate">生日</param>
        /// <returns></returns>
        public static int CalculateAge(string birthDay)
        {
            DateTime birthDate=DateTime.Parse(birthDay);
            DateTime nowDateTime=DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            { 
                age--; 
            }
            return age;
        }

        /// <summary>
        /// 定义 生日年龄性别 实体
        /// </summary>
        public class BirthdayAgeSex
        {
            public string Birthday { get; set; }
            public int Age { get; set; }
            public string Sex { get; set; }
        }
    
    }
}
