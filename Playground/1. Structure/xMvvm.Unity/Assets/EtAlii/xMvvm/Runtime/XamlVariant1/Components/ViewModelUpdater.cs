namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;

    public class ViewModelUpdater<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MemberValueHelper _targetMemberValueHelper;
        private readonly MemberValueHelper _sourceMemberValueHelper;
        private readonly View<TViewModel> _view;
        private readonly object _target;

        public ViewModelUpdater(
            View<TViewModel> view,
            object target,
            MemberValueHelper targetMemberValueHelper,
            MemberValueHelper sourceMemberValueHelper)
        {
            _targetMemberValueHelper = targetMemberValueHelper;
            _sourceMemberValueHelper = sourceMemberValueHelper;
            _view = view;
            _target = target;
        }

        public void Update()
        {
            var value = _targetMemberValueHelper.GetValue(_target);
            _sourceMemberValueHelper.SetValue(_view.ViewModel, value);
        }
    }
}