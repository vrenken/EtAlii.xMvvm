namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;

    public abstract class Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly View<TViewModel> _view;
        protected INotifyPropertyChanged ViewModel { get; set; }
        protected MonoBehaviour Component { get; }
        protected MemberExpression ComponentMemberExpression { get; }

        protected Binding(
            View<TViewModel> view,
            string path, 
            Expression<Func<TComponent, object>> componentPropertyLambda)
        {
            _view = view;
            _view.PropertyChanged += OnViewPropertyChanged;

            if (componentPropertyLambda == null)
            {
                throw new ArgumentNullException(nameof(componentPropertyLambda));
            }

            var child = _view.GameObject.transform.Find(path);
            Component = child != null ? child.GetComponent<TComponent>() : null;
            if (Component == null)
            {
                throw new InvalidOperationException($"Unable to find component {typeof(TComponent)} on path {path}");
            }

            ComponentMemberExpression  = componentPropertyLambda.Body as MemberExpression;
            if (ComponentMemberExpression  == null)
            {
                throw new InvalidOperationException("Unable to access component member from expression: " + componentPropertyLambda);
            }
        }

        protected abstract void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e);
        protected abstract void SetComponentPropertyValue();

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_view.ViewModel)) return;
            
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            ViewModel = _view.ViewModel;

            if (ViewModel != null)
            {
                SetComponentPropertyValue();
                ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }
    }
}