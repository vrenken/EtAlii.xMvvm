namespace EtAlii.xMvvm.XamlVariant1
{
    using UnityEngine;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ElementListener<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MemberInfo _onValueChangedMember;
        private readonly GameObject _element;
        private readonly MemberExpression _elementMemberExpression;
        private readonly ViewModelUpdater<TViewModel> _viewModelUpdater;

        public ElementListener(
            MemberInfo onValueChangedMember, 
            GameObject element, 
            MemberExpression elementMemberExpression,
            ViewModelUpdater<TViewModel> viewModelUpdater)
        {
            _element = element;
            _onValueChangedMember = onValueChangedMember;
            _elementMemberExpression = elementMemberExpression;
            _viewModelUpdater = viewModelUpdater;
        }

        public void StartListening()
        {
            // switch (_onValueChangedMember)
            // {
            //     case PropertyInfo propertyInfo:
            //         var unityPropertyEvent = (UnityEventBase)propertyInfo.GetValue(_component);
            //         AddListener(unityPropertyEvent, _viewModelUpdater.UpdateFromComponent);
            //         break;
            //     case FieldInfo fieldInfo: 
            //         var unityFieldEvent = (UnityEventBase)fieldInfo.GetValue(_component);
            //         AddListener(unityFieldEvent, _viewModelUpdater.UpdateFromComponent);
            //         break;
            // }
        }
        
        public void StopListening()
        {
            // switch (_componentMemberExpression.Member)
            // {
            //     case PropertyInfo propertyInfo:
            //         var unityPropertyEvent = (UnityEventBase) propertyInfo.GetValue(_component);
            //         RemoveListener(unityPropertyEvent, _viewModelUpdater.UpdateFromComponent);
            //         break;
            //     case FieldInfo fieldInfo:
            //         var unityFieldEvent = (UnityEventBase) fieldInfo.GetValue(_component);
            //         RemoveListener(unityFieldEvent, _viewModelUpdater.UpdateFromComponent);
            //         break;
            // }
        }
        //
        // private void AddListener(UnityEventBase unityEvent, UnityAction action)
        // {
        //     switch (unityEvent)
        //     {
        //         case UnityEvent actionEvent:
        //             actionEvent.AddListener(action);
        //             break;
        //         case UnityEvent<bool> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<string> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<float> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<double> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<int> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<DateTime> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<Byte> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<Char> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<uint> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         case UnityEvent<ulong> actionEvent:
        //             actionEvent.AddListener(o => action());
        //             break;                    
        //         default:
        //             throw new NotSupportedException("UnityEvent not supported: " + unityEvent);
        //     }
        // }
        // private void RemoveListener(UnityEventBase unityEvent, UnityAction action)
        // {
        //     switch (unityEvent)
        //     {
        //         case UnityEvent actionEvent:
        //             actionEvent.RemoveListener(action);
        //             break;
        //         case UnityEvent<bool> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;
        //         case UnityEvent<string> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<float> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<int> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<DateTime> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<Byte> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<Char> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<uint> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         case UnityEvent<ulong> actionEvent:
        //             actionEvent.RemoveListener(o => action());
        //             break;                    
        //         default:
        //             throw new NotSupportedException("UnityEvent not supported: " + unityEvent);
        //     }
        // }
    }
}