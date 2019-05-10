using System.Collections.Generic;

namespace BihuApiCore.Model.Models
{
    /// <summary>
    /// 分页数据集合
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PageData<TEntity>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 当前页数据集合
        /// </summary>
        public List<TEntity> DataList { get; set; }
    }
}
