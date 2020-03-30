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
        public GameObject GameObject { get; private set; }

        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MemberExpression _elementMemberExpression; 

        public ElementBinding<TViewModel>[] Elements { get; }

        public string Path { get; }

        private MemberUpdater _memberUpdater;
        private ViewModelListener<TViewModel> _viewModelListener;

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
            
            _memberUpdater = new MemberUpdater(GameObject, _elementMemberExpression);
            _viewModelListener = new ViewModelListener<TViewModel>(view, _viewModelPropertyInfo, _memberUpdater, BindingMode.OneWay);
        }

        protected override void StartBinding()
        {
        }

        protected override void StopBinding()
        {
        }
    }
}