namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;
    using System.Reflection;

    public class ViewModelListener<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MemberValueHelper _memberValueHelper;
        private readonly BindingMode _bindingMode;
        private readonly View<TViewModel> _view;
        private readonly PropertyInfo _viewModelPropertyInfo;

        public ViewModelListener(
            View<TViewModel> view,
            PropertyInfo viewModelPropertyInfo,
            MemberValueHelper memberValueHelper, 
            BindingMode bindingMode)
        {
            _viewModelPropertyInfo = viewModelPropertyInfo;
            _memberValueHelper = memberValueHelper;
            _bindingMode = bindingMode;
            _view = view;
        }

        public void StartListening()
        {
            _view.ViewModel.PropertyChanged += OnViewModelPropertyChanged;

        }
        
        public void StopListening()
        {
            _view.ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _viewModelPropertyInfo.Name) return;

            if (_bindingMode == BindingMode.OneWayToSource) return;
            
            var value = _viewModelPropertyInfo.GetValue(_view.ViewModel);
            _memberValueHelper.SetValue(value);
        }
    }
}