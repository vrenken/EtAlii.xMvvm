namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using UnityEngine;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public class ComponentListener<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MemberInfo _onValueChangedMember;
        private readonly MonoBehaviour _component;
        private readonly ViewModelUpdater<TViewModel> _viewModelUpdater;

        public ComponentListener(
            MonoBehaviour component, 
            ViewModelUpdater<TViewModel> viewModelUpdater)
        {
            _component = component;
            _viewModelUpdater = viewModelUpdater;
            
            _onValueChangedMember = _component.GetType().GetMember("onValueChanged").SingleOrDefault();
            if (_onValueChangedMember == null)
            {
                throw new InvalidOperationException("Unable to access onValueChanged from component: " + _component);
            }
        }

        public void StartListening() => EventHelper.AddListener(_onValueChangedMember, _component, _viewModelUpdater.Update);
        
        public void StopListening() => EventHelper.RemoveListener(_onValueChangedMember, _component, _viewModelUpdater.Update);
    }
}