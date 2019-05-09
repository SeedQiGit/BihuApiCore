using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BihuApiCore.Repository.Repositories
{
    public class EfRepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        
        public DbContext Context { get; set; }

        public EfRepositoryBase(DbContext context)
        {
            Context = context;
        }
        public DbContext GetDbContext()
        {
            return Context;
        }

        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Any(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {

            return await GetAll().AnyAsync(predicate);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).CountAsync();
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return GetAllIncluding();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = Table.AsQueryable();

            if (propertySelectors != null && propertySelectors.Count() >= 0)
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }

            return query;
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public void Insert(TEntity entity)
        {
            Table.Add(entity);
        }

        public void Insert(List<TEntity> listEntity)
        {
            Table.AddRange(listEntity);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await Task.FromResult(Table.Add(entity));
        }

        public async Task InsertAsync(List<TEntity> listEntity)
        {
            await Table.AddRangeAsync(listEntity);
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).LongCountAsync();
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public void Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Update(List<TEntity> listEntity)
        {
            foreach (var item in listEntity)
            {
                Update(item);
            }
        }

        public void SetFieldValue(TEntity entity, Expression<Func<TEntity, object>> field)
        {
            var property = Context.Entry(entity).Property(field);
            //字段复制
            property.IsModified = true;
            //property.CurrentValue = value;
        }

        public virtual void AttachIfNot(TEntity entity)
        {
            var entry = Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }

        #region 同事的

          /// <summary>
        ///     实体指定字段赋值，用以触发部分字段更新
        ///     add by 李永光 2019-05-07
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="entity">待赋值实体</param>
        /// <param name="data">源数据实体，根据属性名匹配（不区分大小写）</param>
        /// <param name="fields">字段名列表，根据属性名匹配（不区分大小写）。若值为null，匹配字段赋值</param>
        /// <returns></returns>
        public void SetFieldValue<T>(TEntity entity, T data, IEnumerable<string> fields = null)
        {
            var cEntity = Context.Entry(entity);
            Type etype = entity.GetType();
            PropertyInfo[] eprops = etype.GetProperties();

            Type dtype = data.GetType();
            PropertyInfo[] dprops = dtype.GetProperties();
            //遍历实体属性进行赋值
            foreach (PropertyInfo pi in eprops)
            {
                var fieldName = pi.Name;
                //不在更新列表
                if (fields != null && !fields.Contains(fieldName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }
                //不在源数据实体中
                var dpi = FindPropertyInfo(dprops, fieldName);
                if (dpi == null)
                {
                    continue;
                }
                //忽略主键
                var fieldProp = cEntity.Property(fieldName);
                if (fieldProp != null && fieldProp.Metadata.IsPrimaryKey())
                {
                    continue;
                }
                
                //属性修改
                object value = dpi.GetValue(data, null);
                SetFieldValue(entity, fieldName, value);
            }
        }

        /// <summary>
        ///     实体属性查找
        ///     add by 李永光 2019-05-07
        /// </summary>
        /// <param name="props">属性列表</param>
        /// <param name="field">属性名</param>
        /// <returns></returns>
        private PropertyInfo FindPropertyInfo(PropertyInfo[] props, string field)
        {
            if (string.IsNullOrEmpty(field)) return null;
            foreach (PropertyInfo pi in props)
            {
                if (string.Equals(pi.Name, field, StringComparison.OrdinalIgnoreCase))
                    return pi;
            }

            return null;
        }

        /// <summary>
        ///     实体指定字段赋值用以触发部分字段更新
        ///     add by 李永光 2019-05-07
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="field">需要赋值的字段</param>
        /// <param name="value">新字段值</param>
        public void SetFieldValue(TEntity entity, Expression<Func<TEntity, object>> field, object value)
        {
            var property = Context.Entry(entity).Property(field);
            //字段复制
            property.IsModified = true;
            property.CurrentValue = value;
        }

        /// <summary>
        ///     实体指定字段赋值用以触发部分字段更新
        ///     add by 李永光 2019-05-07
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="fieldName">需要赋值的字段</param>
        /// <param name="value">新字段值</param>
        public void SetFieldValue(TEntity entity, string fieldName, object value)
        {
            var property = Context.Entry(entity).Property(fieldName);
            if (property == null) throw new Exception($"{nameof(entity)}实体不存在属性名{fieldName}");
            //字段复制
            property.IsModified = true;
            property.CurrentValue = value;
        }

        /// <summary>
        ///     修改部分字段
        ///     add by 李永光 2019-05-07
        ///     使用方法：1先使用SetValue方法给指定的字段赋值，不要使用实体直接赋值，否则不触发更新,2.调用Modify方法添加到更新队列，3调用SaveChanges提交
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Modify(TEntity entity)
        {
            AttachIfNot(entity);
        }

        /// <summary>
        ///     批量修改部分字段
        ///     add by 李永光 2019-05-07
        ///     使用方法：1先使用SetValue方法给指定的字段赋值，不要使用实体直接赋值，否则不触发更新,2.调用Modify方法添加到更新队列，3调用SaveChanges提交
        /// </summary>
        /// <param name="listEntity"></param>
        public void Modify(List<TEntity> listEntity)
        {
            foreach (var item in listEntity)
            {
                Modify(item);
            }
        }

        #endregion
    }
}
