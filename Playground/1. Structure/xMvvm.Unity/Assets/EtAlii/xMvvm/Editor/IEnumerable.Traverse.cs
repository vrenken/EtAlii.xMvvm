namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableTraverseExtensions
    {
        /// <summary>
        /// Return item and all children recursively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="childSelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> SelectAllDepthFirst<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector) => items.SelectMany(i => new T[] { i }.Concat(childSelector(i).SelectAllDepthFirst(childSelector)));
    }
}
