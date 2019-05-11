using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Models;
using BihuApiCore.Model.Request;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BihuApiCore.Repository.Repositories
{
    public class UserRepository: EfRepositoryBase<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public void CommandTest()
        {
            var sql = $@" delete from user where user.Id=100 ";
            Context.ExecuteScalar(sql);
        }

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

            var count = await Context.SqlFuncAsync(sqlcount,c => (long)c["Count"],parmArr);

            #endregion

      
            var result = new PageData<User>
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = count.FirstOrDefault(),
                DataList = data
            };

            return result;
        }
    }
}
