﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the xMvvm Unity 3D toolchain.
//     Runtime Version: 1.0.0-Preview
//     Created: 04/01/2020 22:27:48
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

    public partial class NestedView : EtAlii.xMvvm.XamlVariant1.View<NestedViewModel>  
    {
        // Resources.
        
        // Component bindings.

        // Element bindings.
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _canvas;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _subject;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _child;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _child3;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _child4;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _child2;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _photo;
        private readonly EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel> _child5;

        public NestedView() : this(GameObject.Instantiate(Resources.Load("NestedView.prefab", typeof(GameObject))) as GameObject)
        {
        }
        
        public NestedView(GameObject gameObject) : base(gameObject)
        {
            // Setup the resources.

            // Setup the component bindings.

            // Setup the element bindings.
            var element1 = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>("Child8");
            var element2 = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>("Child6");
            _child5 = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>(this, "Child5", new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>[] { element2, element1 });
            _photo = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>(this, "Photo");
            _child2 = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>("Child2");
            _child4 = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>("Child4");
            _child3 = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>("Child3");
            _child = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>("Child", new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>[] { _child3, _child4 });
            _subject = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>(this, "Subject", element => element.transform, vm => vm.SubjectTransformation, new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>[] { _child, _child2 });
            _canvas = ﻿new EtAlii.xMvvm.XamlVariant1.ElementBinding<NestedViewModel>(this, "Canvas2");
 
            // And initialize.
            Initialize();   
        }
    }
}
