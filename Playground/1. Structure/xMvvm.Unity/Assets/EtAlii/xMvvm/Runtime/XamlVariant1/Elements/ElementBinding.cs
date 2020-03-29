namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;

    public class ElementBinding<TViewModel> : ViewBinding<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        public GameObject GameObject { get; private set; }
        public ElementBinding<TViewModel>[] Elements { get; }

        public string Path { get; }

        public ElementBinding(string path)
        {
            Path = path;
            Elements = Array.Empty<ElementBinding<TViewModel>>();
        }
        

        public ElementBinding(
            View<TViewModel> view,
            string path)
            : this(view, path, null, Array.Empty<ElementBinding<TViewModel>>())
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
            Expression<Func<TViewModel, object>> vm)
            : this(view, path, vm, Array.Empty<ElementBinding<TViewModel>>())
        {
        }

        public ElementBinding(
            View<TViewModel> view, 
            string path, 
            Expression<Func<TViewModel, object>> vm, 
            ElementBinding<TViewModel>[] elements)
            : base(view)
        {
            Path = path;
            Elements = elements;

            var parentGameObject = view.GameObject;
            DelayedInitialize(view, parentGameObject);
        }

        private void DelayedInitialize(View<TViewModel> view, GameObject parentGameObject)
        {
            View = view;
            GameObject = parentGameObject.transform.Find(Path).gameObject;
            
            foreach (var element in Elements)
            {
                element.DelayedInitialize(view, GameObject);                
            }
        }

        protected override void StartBinding()
        {
        }

        protected override void StopBinding()
        {
        }
    }
}