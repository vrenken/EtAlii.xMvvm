namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;
    using System.Reflection;
    
    public class ViewModelUpdater<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MemberValueHelper _sourceMemberValueHelper;
        private readonly View<TViewModel> _view;

        public ViewModelUpdater(
            View<TViewModel> view,
            PropertyInfo viewModelPropertyInfo,
            MemberValueHelper sourceMemberValueHelper)
        {
            _viewModelPropertyInfo = viewModelPropertyInfo;
            _sourceMemberValueHelper = sourceMemberValueHelper;
            _view = view;
        }

        public void Update()
        {
            var value = _sourceMemberValueHelper.GetValue();
            _viewModelPropertyInfo.SetValue(_view.ViewModel, value);
        }
    }
}