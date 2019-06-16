using System;
using System.Collections.Generic;
using System.Linq;

namespace BihuApiCore.Infrastructure.Helper
{
    public static class ComparerHelper
    {
        /// <summary>
        ///     自定义Distinct扩展方法
        /// </summary>
        /// <typeparam name="T">要去重的对象类</typeparam>
        /// <param name="source">要去重的对象</param>
        /// <param name="getfield">获取自定义去重字段的委托</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctEx<T>(this IEnumerable<T> source, params Func<T, object>[] getfield)
        {
            return source.Distinct(new CompareEntityFields<T>(getfield));
        }
    }
    public class CompareEntityFields<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object>[] _compareFields;

        /// <summary>
        ///     可以根据字段比对数据
        /// </summary>
        /// <param name="compareFields">比对字段引用</param>
        public CompareEntityFields(params Func<T, object>[] compareFields)
        {
            _compareFields = compareFields;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            if (_compareFields == null || _compareFields.Length <= 0)
            {
                return EqualityComparer<T>.Default.Equals(x, y);
            }

            bool result = true;
            foreach (var func in _compareFields)
            {
                var xv = func(x);
                var yv = func(y);
                result = xv == null && yv == null || Equals(xv, yv);
                if (!result) break;
            }

            return result;
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj">obj</paramref> is a reference type
        ///     and <paramref name="obj">obj</paramref> is null.
        /// </exception>
        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
