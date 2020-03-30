namespace EtAlii.xMvvm.XamlVariant1
{
    using UnityEngine;

    public partial class MemberValueHelper
    {
        private bool SetValueExplicit(object instance, object value)
        {
            if (instance is GameObject gameObject && 
                value is ViewModelTransform viewModelTransform &&
                MemberName == "transform")
            {
                gameObject.transform.localPosition = viewModelTransform.LocalPosition.ToUnity();
                gameObject.transform.localScale = viewModelTransform.LocalScale.ToUnity();
                gameObject.transform.localRotation = viewModelTransform.LocalRotation.ToUnity();
                return true;
            }

            return false;
        }
    }
}