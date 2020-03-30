namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;

    public class ViewModelListener<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MemberValueHelper _targetMemberValueHelper;
        private readonly MemberValueHelper _sourceMemberValueHelper;
        private readonly BindingMode _bindingMode;
        private readonly View<TViewModel> _view;
        private readonly object _target;

        public ViewModelListener(
            View<TViewModel> view,
            object target,
            MemberValueHelper targetMemberValueHelper, 
            MemberValueHelper sourceMemberValueHelper,
            BindingMode bindingMode)
        {
            _targetMemberValueHelper = targetMemberValueHelper;
            _sourceMemberValueHelper = sourceMemberValueHelper;
            _bindingMode = bindingMode;
            _view = view;
            _target = target;
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
            if (e.PropertyName != _sourceMemberValueHelper.MemberName) return;

            if (_bindingMode == BindingMode.OneWayToSource) return;

            var value = _sourceMemberValueHelper.GetValue(_view.ViewModel);
            _targetMemberValueHelper.SetValue(_target, value);
        }
    }
}