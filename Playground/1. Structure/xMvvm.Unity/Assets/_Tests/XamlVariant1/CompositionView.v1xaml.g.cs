﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the xMvvm Unity 3D toolchain.
//     Runtime Version: TBD
//     Created: 03/18/2020 00:04:16
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace EtAlii.xMvvm
{
    using UnityEngine;
    using System;
    using EtAlii.xMvvm.XamlVariant1;

    public partial class CompositionView : EtAlii.xMvvm.XamlVariant1.View<CompositionViewModel>  
    {
        // Resources.
        private readonly LinearTransformationConverter _linearTransformationConverter;
        
        // Bindings.
        private readonly Binding<UnityEngine.UI.Slider, CompositionViewModel> _up;
        private readonly Binding<UnityEngine.UI.Slider, CompositionViewModel> _forward;
        private readonly Binding<UnityEngine.UI.Toggle, CompositionViewModel> _rotate;

        // Elements.
        private readonly EtAlii.xMvvm.XamlVariant1.Element _canvas;
        private readonly EtAlii.xMvvm.XamlVariant1.Element _subject;

        public CompositionView() : this(GameObject.Instantiate(Resources.Load("CompositionView.prefab", typeof(GameObject))) as GameObject)
        {
        }
        
        public CompositionView(GameObject gameObject) : base(gameObject)
        {
            // Setup the resources.
            _linearTransformationConverter = new LinearTransformationConverter();

            // Wire the bindings.
            _up = new PropertyBinding<UnityEngine.UI.Slider, CompositionViewModel>(this, "Canvas/Panel/UpSlider", component => component.value, vm => vm.Up, BindingMode.OneWayToSource);
            _forward = new PropertyBinding<UnityEngine.UI.Slider, CompositionViewModel>(this, "Canvas/Panel/ForwardSlider", component => component.value, vm => vm.Forward, BindingMode.OneWayToSource);
            _rotate = new PropertyBinding<UnityEngine.UI.Toggle, CompositionViewModel>(this, "Canvas/Panel/RotateToggle", component => component.isOn, vm => vm.Rotate, BindingMode.OneWayToSource);

            // Setup the child elements.
            _canvas = ﻿new EtAlii.xMvvm.XamlVariant1.Element(gameObject, this, "Canvas")  
            {
                Elements = new EtAlii.xMvvm.XamlVariant1.Element[]
                {
                }
            };
            _subject = ﻿new EtAlii.xMvvm.XamlVariant1.Element(gameObject, this, "Subject")  
            {
                Elements = new EtAlii.xMvvm.XamlVariant1.Element[]
                {
                    ﻿new EtAlii.xMvvm.XamlVariant1.Element(gameObject, this, "Child")  
                    {
                        Elements = new EtAlii.xMvvm.XamlVariant1.Element[]
                        {
                        }
                    },
                    ﻿new EtAlii.xMvvm.XamlVariant1.Element(gameObject, this, "Child2")  
                    {
                        Elements = new EtAlii.xMvvm.XamlVariant1.Element[]
                        {
                        }
                    }
                }
            };

            // And initialize.
            Initialize();   
        }

    }
}