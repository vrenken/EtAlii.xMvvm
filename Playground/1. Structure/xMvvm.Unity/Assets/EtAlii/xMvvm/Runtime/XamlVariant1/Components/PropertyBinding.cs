namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public class PropertyBinding<TComponent, TViewModel> : ComponentBinding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly BindingMode _bindingMode;

        private readonly ComponentListener<TViewModel> _componentListener;
        private readonly MemberUpdater _memberUpdater;
        private readonly ViewModelUpdater<TViewModel> _viewModelUpdater;
        private readonly ViewModelListener<TViewModel> _viewModelListener;

        private readonly PropertyInfo _viewModelPropertyInfo;

        public PropertyBinding(
            View<TViewModel> view, 
            string path, 
            Expression<Func<TComponent, object>> component, 
            Expression<Func<TViewModel, object>> vm, 
            BindingMode bindingMode) 
            : base(view, path, component)
        {

            _bindingMode = bindingMode;

            MemberHelper.GetProperty(vm, out _viewModelPropertyInfo);

            _viewModelUpdater = new ViewModelUpdater<TViewModel>(view, _viewModelPropertyInfo, Component, ComponentMemberExpression);
            _memberUpdater = new MemberUpdater(Component, ComponentMemberExpression);
            _viewModelListener = new ViewModelListener<TViewModel>(view, _viewModelPropertyInfo, _memberUpdater, bindingMode);

            if (_bindingMode != BindingMode.TwoWay && _bindingMode != BindingMode.OneWayToSource) return;
            
            _componentListener = new ComponentListener<TViewModel>(Component, _viewModelUpdater);
        }

        protected override void StartBinding()
        {
            if (_bindingMode == BindingMode.OneWayToSource)
            {
                _viewModelUpdater.UpdateFromComponent();
            }
            else
            {
                var value = _viewModelPropertyInfo.GetValue(View.ViewModel);
                _memberUpdater.Update(value);
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                _viewModelListener.StartListening();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                _componentListener.StartListening();
            }
        }
      
        
        protected override void StopBinding()
        {
            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                _componentListener.StopListening();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                _viewModelListener.StopListening();
            }
        }
    }
}