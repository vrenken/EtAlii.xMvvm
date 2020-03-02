namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

   public static class PropertyChangedEventHandlerRaiseExtensions
    {
        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        public static void SetAndRaise<TSource, TProperty>(this PropertyChangedEventHandler eventHandler, TSource source, ref TProperty backingStore, TProperty value, Expression<Func<TSource, TProperty>> propertyNameExpression)
        {
            if (EqualityComparer<TProperty>.Default.Equals(backingStore, value))
                return;
            backingStore = value;

            eventHandler?.Invoke(source, new PropertyChangedEventArgs(((MemberExpression)propertyNameExpression.Body).Member.Name));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        public static void Raise<TSource, TProperty>(this PropertyChangedEventHandler eventHandler, TSource source, Expression<Func<TSource, TProperty>> propertyNameExpression)
        {
            eventHandler?.Invoke(source, new PropertyChangedEventArgs(((MemberExpression)propertyNameExpression.Body).Member.Name));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        public static void Raise<TSource>(this PropertyChangedEventHandler eventHandler, TSource source, [CallerMemberName] string propertyName = null)
        {
            eventHandler?.Invoke(source, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        public static void SetAndRaise<TSource, TProperty>(this PropertyChangedEventHandler eventHandler, TSource source, ref TProperty backingStore, TProperty value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TProperty>.Default.Equals(backingStore, value))
                return;
            backingStore = value;

            eventHandler?.Invoke(source, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event for all properties.
        /// </summary>
        public static void RaiseAll<TSource>(this PropertyChangedEventHandler eventHandler, TSource source)
        {
            eventHandler?.Invoke(source, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Raises the PropertyChanged event for all properties.
        /// </summary>
        public static void SetAndRaiseAll<TSource, TProperty>(this PropertyChangedEventHandler eventHandler, TSource source, ref TProperty backingStore, TProperty value)
        {
            if (EqualityComparer<TProperty>.Default.Equals(backingStore, value))
                return;
            backingStore = value;
            eventHandler?.Invoke(source, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
