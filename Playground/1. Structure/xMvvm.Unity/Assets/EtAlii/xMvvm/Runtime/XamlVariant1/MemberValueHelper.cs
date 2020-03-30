namespace EtAlii.xMvvm.XamlVariant1
{
    using System.Linq.Expressions;
    using System.Reflection;
    
    public class MemberValueHelper
    {
        private readonly object _target;
        private readonly MemberExpression _memberExpression;

        public MemberValueHelper(object target, MemberExpression memberExpression)
        {
            _target = target;
            _memberExpression = memberExpression;
        }

        public void SetValue(object value)
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

        public object GetValue()
        {
            object value = null;
            switch (_memberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo: 
                    value = componentPropertyInfo.GetValue(_target, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    value = componentFieldInfo.GetValue(_target);
                    break;
            }
            return value;
        }
    }
}