namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public class ListElementBinding<TViewModel> : ElementBinding<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        public ListElementBinding(string path) : base(path)
        {
        }

        public ListElementBinding(View<TViewModel> view, string path) : base(view, path)
        {
        }

        public ListElementBinding(View<TViewModel> view, string path, ElementBinding<TViewModel>[] elements) : base(view, path, elements)
        {
        }

        public ListElementBinding(string path, ElementBinding<TViewModel>[] elements) : base(path, elements)
        {
        }

        public ListElementBinding(View<TViewModel> view, string path, Expression<Func<TViewModel, object>> vm) : base(view, path, null, vm)
        {
        }

        public ListElementBinding(View<TViewModel> view, string path, Expression<Func<TViewModel, object>> vm, ElementBinding<TViewModel>[] elements) : base(view, path, null, vm, elements)
        {
        }
        
        public void StartListening(IEnumerable enumerable)
        {
            if (enumerable is INotifyCollectionChanged observableCollection)
            {
                observableCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        public void StopListening(IEnumerable enumerable)
        {
            if (enumerable is INotifyCollectionChanged observableCollection)
            {
                observableCollection.CollectionChanged -= OnCollectionChanged;
            }
        }
        
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}