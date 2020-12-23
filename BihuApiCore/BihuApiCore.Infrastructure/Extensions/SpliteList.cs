using System.Collections.Generic;
using System.Linq;

namespace BihuApiCore.Infrastructure.Extensions
{
    public class SpliteList
    {
        /// <summary>
        /// 将集合若干组
        /// </summary>
        /// <param name="source">数据集</param>
        /// <param name="pageSiez">每一组大小</param>
        private static List<List<int>> SpliteSourceBySize(List<int> source, int pageSiez)
        {
            int listCount = (source.Count() - 1) / pageSiez + 1;

            // 计算组数 
            List<List<int>> pages = new List<List<int>>();
            for (int pageIndex = 1; pageIndex <= listCount; pageIndex++)
            {
                var page = source.Skip((pageIndex - 1) * pageSiez).Take(pageSiez).ToList(); 
                pages.Add(page);
            }
            return pages;
        }

        /// <summary>
        ///  将集合若干组
        /// </summary>
        /// <param name="source">数据集<</param>
        /// <param name="count">组数</param>
        /// <returns></returns>
        private static List<List<int>> SpliteSourceByCount(List<int> source, int count)
        {
            int pageSiez = source.Count() / count;//取每一页大小 
            int remainder = source.Count() % count;//取余数 
            List<List<int>> pages = new List<List<int>>();
            for (int pageIndex = 1; pageIndex <= count; pageIndex++)
            {
                if (pageIndex != count)
                {
                    var page = source.Skip((pageIndex - 1) * pageSiez).Take(pageSiez).ToList();
                    pages.Add(page);
                }
                else
                {
                    var page = source.Skip((pageIndex - 1) * pageSiez).Take(pageSiez + remainder).ToList();
                    pages.Add(page);
                }
            }
            return pages;
        }
    }
}
