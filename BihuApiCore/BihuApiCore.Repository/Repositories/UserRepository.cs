using BihuApiCore.EntityFrameworkCore;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace BihuApiCore.Repository.Repositories
{
    public class UserRepository: EfRepositoryBase<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public async Task ContextTest()
        {
            User user=new User(); 
            Context.Set<User>().Add(user);
            Companies companies=new Companies();
            Context.Set<Companies>().Add(companies);
            await Context.SaveChangesAsync();

        }

        public void CommandTest()
        {
            var sql = $@" delete from user where user.Id=100 ";
            Context.Database.ExecuteSqlCommand(sql);
        }

        #region 列表方法

        public async Task<PageData<User>> GetUserList(PageRequest request,string levelCode)
        {
            List<DbParameter> parms = new List<DbParameter>();

            parms.Add(  new MySqlParameter
            {
                MySqlDbType = MySqlDbType.VarChar,
                ParameterName = "levelCode",
                Value = levelCode
            });

            var parmArr = parms.ToArray();

            #region 查数据

            string select = $@"
                SELECT * FROM user where LevelCode like  concat(?levelCode,'%')  ";
            string limit = request.LimitSql();
            string sql = string.Concat(select, limit);
           
            var data =await Context.SqlQueryAsync<User>(sql,parmArr);

            #endregion

            #region 查总数

            string sqlcount = @"
                        SELECT
	                        count(1)  as Count
                        FROM  user where LevelCode like  concat(?levelCode,'%') ";

            //var count = await Context.SqlFuncAsync(sqlcount,c => (long)c["Count"],parmArr);
            var count = await Context.SqlQueryFirstAsync<long>(sqlcount,parmArr);
            #endregion

      
            var result = new PageData<User>
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = count,
                DataList = data
            };

            return result;
        }

        #endregion

        #region 测试sql方法

        public async Task<List<IsVerifyEnum>> TestSql()
        {
            #region 查总数

            string sqlcount = @"
                        SELECT
	                        IsVerify
                        FROM  user ";

            var count = await Context.SqlQueryAsync<IsVerifyEnum>(sqlcount);

            #endregion

            return count;
        }

        #endregion
    }
}
