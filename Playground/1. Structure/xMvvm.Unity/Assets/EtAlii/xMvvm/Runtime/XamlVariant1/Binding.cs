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
            Expression<Func<TComponent, object>> component)
        {
            _view = view;
            _view.PropertyChanged += OnViewPropertyChanged;

            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            var child = _view.GameObject.transform.Find(path);
            Component = child != null ? child.GetComponent<TComponent>() : null;
            if (Component == null)
            {
                throw new InvalidOperationException($"Unable to find component {typeof(TComponent)} on path {path}");
            }

            switch (component.Body)
            {
                case MemberExpression memberExpression:
                    ComponentMemberExpression  = memberExpression;
                    break;
                case UnaryExpression unaryExpression:
                    ComponentMemberExpression  = unaryExpression.Operand as MemberExpression;
                    break;
            }
            if (ComponentMemberExpression  == null)
            {
                throw new InvalidOperationException("Unable to access component member from expression: " + component);
            }
        }

        protected abstract void StartBinding();
        protected abstract void StopBinding();
        protected abstract void UpdateBinding();

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_view.ViewModel)) return;
            
            if (ViewModel != null)
            {
                StopBinding();
            }

            ViewModel = _view.ViewModel;

            if (ViewModel != null)
            {
                UpdateBinding();
                StartBinding();
            }
        }
    }
}