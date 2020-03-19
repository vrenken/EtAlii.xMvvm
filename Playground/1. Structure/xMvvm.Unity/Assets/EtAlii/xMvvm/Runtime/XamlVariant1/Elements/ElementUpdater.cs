namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;
    using System.Reflection;
    
    public class ElementUpdater<TViewModel>
    where TViewModel : INotifyPropertyChanged
    {
        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly GameObject _element;
        private readonly MemberExpression _elementMemberExpression;
        private readonly View<TViewModel> _view;

        public ElementUpdater(
            View<TViewModel> view,
            PropertyInfo viewModelPropertyInfo,
            GameObject element, 
            MemberExpression componentMemberExpression)
        {
            _element = element;
            _elementMemberExpression = componentMemberExpression;
            _view = view;
            _viewModelPropertyInfo = viewModelPropertyInfo;
        }

        public void UpdateFromViewModel()
        {
            var value = _viewModelPropertyInfo.GetValue(_view.ViewModel);
                
            switch (_elementMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo: 
                    componentPropertyInfo.SetValue(_element, value, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    componentFieldInfo.SetValue(_element, value);
                    break;
            }
        }
    }
}