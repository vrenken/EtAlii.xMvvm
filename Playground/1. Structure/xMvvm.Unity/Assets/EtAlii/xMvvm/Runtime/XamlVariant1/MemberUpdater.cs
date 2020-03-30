namespace EtAlii.xMvvm.XamlVariant1
{
    using System.Linq.Expressions;
    using System.Reflection;
    
    public class MemberUpdater
    {
        private readonly object _target;
        private readonly MemberExpression _memberExpression;

        public MemberUpdater(object target, MemberExpression memberExpression)
        {
            _target = target;
            _memberExpression = memberExpression;
        }

        public void Update(object value)
        {
            switch (_memberExpression.Member)
            {
                case PropertyInfo propertyInfo: 
                    propertyInfo.SetValue(_target, value, null);
                    break;
                case FieldInfo fieldInfo: 
                    fieldInfo.SetValue(_target, value);
                    break;
            }
        }
    }
}