namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public class ElementBinding<TViewModel> : ViewBinding<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly BindingMode _bindingMode = BindingMode.OneWay;
        public GameObject GameObject { get; private set; }

        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MemberExpression _elementMemberExpression; 

        public ElementBinding<TViewModel>[] Elements { get; }

        public string Path { get; }

        private MemberValueHelper _elementMemberValueHelper;
        private ViewModelListener<TViewModel> _viewModelListener;
        private ElementListener<TViewModel> _elementListener;
        private ViewModelUpdater<TViewModel> _viewModelUpdater;

        public ElementBinding(string path)
        {
            Path = path;
            Elements = Array.Empty<ElementBinding<TViewModel>>();
        }
        

        public ElementBinding(
            View<TViewModel> view,
            string path)
            : this(view, path, Array.Empty<ElementBinding<TViewModel>>())
        {
        }

        public ElementBinding(
            View<TViewModel> view,
            string path,
            ElementBinding<TViewModel>[] elements)
            : base(view)
        {
            Path = path;
            Elements = elements;
        }
        
        public ElementBinding(
            string path,
            ElementBinding<TViewModel>[] elements)
        {
            Path = path;
            Elements = elements;
        }

        public ElementBinding(
            View<TViewModel> view, 
            string path, 
            Expression<Func<GameObject, object>> element, 
            Expression<Func<TViewModel, object>> vm)
            : this(view, path, element, vm, Array.Empty<ElementBinding<TViewModel>>())
        {
        }

        public ElementBinding(
            View<TViewModel> view, 
            string path, 
            Expression<Func<GameObject, object>> element, 
            Expression<Func<TViewModel, object>> vm, 
            ElementBinding<TViewModel>[] elements)
            : base(view)
        {
            Path = path;
            Elements = elements;
            
            MemberHelper.GetProperty(vm, out _viewModelPropertyInfo);
            MemberHelper.GetMember(element, out _elementMemberExpression);

            
            var parentGameObject = view.GameObject;
            DelayedInitialize(view, parentGameObject);
        }

        private void DelayedInitialize(View<TViewModel> view, GameObject parentGameObject)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }
            View = view;
            if (parentGameObject == null)
            {
                throw new ArgumentNullException(nameof(parentGameObject));
            }
            var gameObject = parentGameObject.transform.Find(Path)?.gameObject; 
            if (gameObject == null)
            {
                throw new InvalidOperationException($"Unable to find gameobject for view/path: {View.GetType().Name}{Path}");
            }

            GameObject = gameObject;
            
            foreach (var element in Elements)
            {
                element.DelayedInitialize(view, GameObject);                
            }
            
            _elementMemberValueHelper = new MemberValueHelper(GameObject, _elementMemberExpression);
            _viewModelUpdater = new ViewModelUpdater<TViewModel>(view, _viewModelPropertyInfo, _elementMemberValueHelper);
            _viewModelListener = new ViewModelListener<TViewModel>(view, _viewModelPropertyInfo, _elementMemberValueHelper, BindingMode.OneWay);
            
            // We only want a element listener when the binding is two-way.
            if (_bindingMode != BindingMode.TwoWay && _bindingMode != BindingMode.OneWayToSource) return;
            _elementListener = new ElementListener<TViewModel>(GameObject, _viewModelUpdater);
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
                _elementMemberValueHelper.SetValue(value);
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                _viewModelListener.StartListening();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                _elementListener.StartListening();
            }
        }

        protected override void StopBinding()
        {
            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                _elementListener.StopListening();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                _viewModelListener.StopListening();
            }
        }
    }
}