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
        private readonly MemberValueHelper _componentMemberValueHelper;
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

            _componentMemberValueHelper = new MemberValueHelper(Component, ComponentMemberExpression);
            _viewModelUpdater = new ViewModelUpdater<TViewModel>(view, _viewModelPropertyInfo, _componentMemberValueHelper);
            _viewModelListener = new ViewModelListener<TViewModel>(view, _viewModelPropertyInfo, _componentMemberValueHelper, bindingMode);

            // We only want a component listener when the binding is two-way.
            if (_bindingMode != BindingMode.TwoWay && _bindingMode != BindingMode.OneWayToSource) return;
            _componentListener = new ComponentListener<TViewModel>(Component, _viewModelUpdater);
        }

        protected override void StartBinding()
        {
            if (_bindingMode == BindingMode.OneWayToSource)
            {
                _viewModelUpdater.Update();
            }
            else
            {
                var value = _viewModelPropertyInfo.GetValue(View.ViewModel);
                _componentMemberValueHelper.SetValue(value);
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