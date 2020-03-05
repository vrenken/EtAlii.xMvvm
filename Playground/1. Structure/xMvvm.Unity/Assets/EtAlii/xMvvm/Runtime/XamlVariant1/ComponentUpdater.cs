namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;
    using System.Reflection;
    
    public class ComponentUpdater<TViewModel>
    where TViewModel : INotifyPropertyChanged
    {
        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MonoBehaviour _component;
        private readonly MemberExpression _componentMemberExpression;
        private readonly View<TViewModel> _view;

        public ComponentUpdater(
            View<TViewModel> view,
            PropertyInfo viewModelPropertyInfo,
            MonoBehaviour component, 
            MemberExpression componentMemberExpression)
        {
            _component = component;
            _componentMemberExpression = componentMemberExpression;
            _view = view;
            _viewModelPropertyInfo = viewModelPropertyInfo;
        }

        public void UpdateFromViewModel()
        {
            var value = _viewModelPropertyInfo.GetValue(_view.ViewModel);
                
            switch (_componentMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo: 
                    componentPropertyInfo.SetValue(_component, value, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    componentFieldInfo.SetValue(_component, value);
                    break;
            }
        }

    }
}