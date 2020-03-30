namespace EtAlii.xMvvm.XamlVariant1
{
    using System.Reflection;
    
    public partial class MemberValueHelper
    {
        private readonly MemberInfo _memberInfo;

        public string MemberName => _memberInfo.Name;
        
        public MemberValueHelper(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }
        
        public void SetValue(object instance, object value)
        {
            // First try to set the value explicit. There are some mappings that cannot be done automagically.
            if (SetValueExplicit(instance, value)) return;
            
            switch (_memberInfo)
            {
                case PropertyInfo propertyInfo: 
                    propertyInfo.SetValue(instance, value, null);
                    break;
                case FieldInfo fieldInfo: 
                    fieldInfo.SetValue(instance, value);
                    break;
            }
        }

        public object GetValue(object instance)
        {
            object value = null;
            switch (_memberInfo)
            {
                case PropertyInfo componentPropertyInfo: 
                    value = componentPropertyInfo.GetValue(instance, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    value = componentFieldInfo.GetValue(instance);
                    break;
            }
            return value;
        }
    }
}