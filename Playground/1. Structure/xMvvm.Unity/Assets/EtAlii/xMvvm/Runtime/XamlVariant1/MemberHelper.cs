namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class MemberHelper
    {
        
        public static void GetMember<TInstance>(Expression<Func<TInstance, object>> instance,
            out MemberExpression memberExpression)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            memberExpression = null;
            switch (instance.Body)
            {
                case MemberExpression me:
                    memberExpression = me;
                    break;
                case UnaryExpression unaryExpression:
                    memberExpression = unaryExpression.Operand as MemberExpression;
                    break;
            }
            if (memberExpression == null)
            {
                throw new InvalidOperationException("Unable to access member from expression: " + instance);
            }

        }

        public static void GetProperty<TInstance>(Expression<Func<TInstance, object>> instance, out PropertyInfo propertyInfo)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            propertyInfo = null;
            switch (instance.Body)
            {
                case MemberExpression memberExpression:
                    propertyInfo = memberExpression.Member as PropertyInfo;
                    break;
                case UnaryExpression unaryExpression:
                    propertyInfo = (unaryExpression.Operand as MemberExpression)?.Member as PropertyInfo;
                    break;
            }
            if (propertyInfo == null)
            {
                throw new InvalidOperationException("Unable to access property from expression: " + instance);
            }
        }
    }
}