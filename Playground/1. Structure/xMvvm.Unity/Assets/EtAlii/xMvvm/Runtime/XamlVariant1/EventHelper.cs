namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.Reflection;
    using UnityEngine.Events;

    public static class EventHelper
    {
        public static void AddListener(MemberInfo member, object instance, object argument, MethodInfo handler)
        {
            var listener = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), argument, handler);
            UnityEvent unityEvent = null;

            switch (member)
            {
                case PropertyInfo propertyInfo:
                    unityEvent = (UnityEvent)propertyInfo.GetValue(instance);
                    break;
                case FieldInfo fieldInfo: 
                    unityEvent = (UnityEvent)fieldInfo.GetValue(instance);
                    break;
            }

            if (unityEvent == null)
            {
                throw new InvalidOperationException($"Unable to find unity event for Member: {member?.Name ?? "None"}");
            }
            unityEvent.AddListener(listener);
        }

        public static void RemoveListener(MemberInfo member, object instance, object argument, MethodInfo handler)
        {
            var listener = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), argument, handler);
            UnityEvent unityEvent = null;

            switch (member)
            {
                case PropertyInfo componentPropertyInfo:
                    unityEvent = (UnityEvent)componentPropertyInfo.GetValue(instance);
                    break;
                case FieldInfo componentFieldInfo: 
                    unityEvent = (UnityEvent)componentFieldInfo.GetValue(instance);
                    break;
            }
            if (unityEvent == null)
            {
                throw new InvalidOperationException($"Unable to find unity event for Member: {member?.Name ?? "None"}");
            }
            unityEvent.RemoveListener(listener);
        }

        public static void AddListener(MemberInfo member, object instance, UnityAction handler)
        {
            UnityEventBase unityEvent = null;
            switch (member)
            {
                case PropertyInfo propertyInfo:
                    unityEvent = (UnityEventBase)propertyInfo.GetValue(instance);
                    break;
                case FieldInfo fieldInfo: 
                    unityEvent = (UnityEventBase)fieldInfo.GetValue(instance);
                    break;
            }
            AddListener(unityEvent, handler);
        }

        public static void RemoveListener(MemberInfo member, object instance, UnityAction handler)
        {
            UnityEventBase unityEvent = null;
            switch (member)
            {
                case PropertyInfo propertyInfo:
                    unityEvent = (UnityEventBase) propertyInfo.GetValue(instance);
                    break;
                case FieldInfo fieldInfo:
                    unityEvent = (UnityEventBase) fieldInfo.GetValue(instance);
                    break;
            }
            RemoveListener(unityEvent, handler);
        }

                
        private static void AddListener(UnityEventBase unityEvent, UnityAction action)
        {
            switch (unityEvent)
            {
                case UnityEvent actionEvent:
                    actionEvent.AddListener(action);
                    break;
                case UnityEvent<bool> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<string> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<float> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<double> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<int> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<DateTime> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<Byte> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<Char> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<uint> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<ulong> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                default:
                    throw new NotSupportedException("UnityEvent not supported: " + unityEvent);
            }
        }
        private static void RemoveListener(UnityEventBase unityEvent, UnityAction action)
        {
            switch (unityEvent)
            {
                case UnityEvent actionEvent:
                    actionEvent.RemoveListener(action);
                    break;
                case UnityEvent<bool> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;
                case UnityEvent<string> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<float> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<int> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<DateTime> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<Byte> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<Char> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<uint> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<ulong> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                default:
                    throw new NotSupportedException("UnityEvent not supported: " + unityEvent);
            }
        }

    }
}