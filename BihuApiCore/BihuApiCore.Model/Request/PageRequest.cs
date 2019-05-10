using BihuApiCore.Infrastructure.Configuration;

namespace BihuApiCore.Model.Request
{
    public class PageRequest:BaseRequest
    {
        private int _pageIndex;
        private int _pageSize;

        /// <summary>
        ///     页码
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex <= 0 ? 1 : _pageIndex;
            set => _pageIndex = value <= 0 ? 1 : value;
        }

        /// <summary>
        ///页大小
        /// </summary>
        public int PageSize
        {
            get => _pageSize <= 0 ? ConfigurationManager.GetValue<int>("PageSize") : _pageSize;
            set => _pageSize = value <= 0 ? ConfigurationManager.GetValue<int>("PageSize") : value;
        }

        public string LimitSql()
        {
            string limit = $"LIMIT {(PageIndex - 1) * PageSize},{PageSize}";
            return limit;
        }
    }
}
