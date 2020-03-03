namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public abstract class Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly View<TViewModel> _view;
        private INotifyPropertyChanged _viewModel;
        private readonly MonoBehaviour _component;
        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MemberExpression _componentMemberExpression;

        protected Binding(
            View<TViewModel> view,
            string path, 
            Expression<Func<TComponent, object>> componentPropertyLambda, 
            Expression<Func<TViewModel, object>> viewModelPropertyLambda)
        {
            _view = view;
            _view.PropertyChanged += OnViewPropertyChanged;

            if (componentPropertyLambda == null)
            {
                throw new ArgumentNullException(nameof(componentPropertyLambda));
            }

            if (viewModelPropertyLambda == null)
            {
                throw new ArgumentNullException(nameof(viewModelPropertyLambda));
            }

            var child = _view.GameObject.transform.Find(path);
            _component = child != null ? child.GetComponent<TComponent>() : null;
            if (_component == null)
            {
                throw new InvalidOperationException($"Unable to find component {typeof(TComponent)} on path {path}");
            }

            _componentMemberExpression = componentPropertyLambda.Body as MemberExpression;
            if (_componentMemberExpression == null)
            {
                throw new InvalidOperationException("Unable to access component member from expression: " + componentPropertyLambda);
            }

            var viewModelMemberExpression = viewModelPropertyLambda.Body as MemberExpression;
            _viewModelPropertyInfo = viewModelMemberExpression?.Member as PropertyInfo;
            if (_viewModelPropertyInfo == null)
            {
                throw new InvalidOperationException("Unable to access viewModelProperty from expression: " + viewModelPropertyLambda);
            }
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_view.ViewModel)) return;
            
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _viewModel = _view.ViewModel;

            if (_viewModel != null)
            {
                SetComponentPropertyValue();
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }
        
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _viewModelPropertyInfo.Name) return;
            
            SetComponentPropertyValue();
        }
        
        private void SetComponentPropertyValue()
        {
            var value = _viewModelPropertyInfo.GetValue(_viewModel);
                
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