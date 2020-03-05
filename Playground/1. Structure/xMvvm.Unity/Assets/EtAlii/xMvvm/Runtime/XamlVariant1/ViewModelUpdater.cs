namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;
    using System.Reflection;
    
    public class ViewModelUpdater<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MonoBehaviour _component;
        private readonly MemberExpression _componentMemberExpression;
        private readonly View<TViewModel> _view;

        public ViewModelUpdater(
            View<TViewModel> view,
            PropertyInfo viewModelPropertyInfo,
            MonoBehaviour component, 
            MemberExpression componentMemberExpression)
        {
            _viewModelPropertyInfo = viewModelPropertyInfo;
            _component = component;
            _componentMemberExpression = componentMemberExpression;
            _view = view;
        }

        public void UpdateFromComponent()
        {
            object value = null;
                
            switch (_componentMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo: 
                    value = componentPropertyInfo.GetValue(_component, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    value = componentFieldInfo.GetValue(_component);
                    break;
            }
            _viewModelPropertyInfo.SetValue(_view.ViewModel, value);
        }
    }
}