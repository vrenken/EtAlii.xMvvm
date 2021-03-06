# What is xMvvm?
xMvvm is a cross-UI paradigm 3D application architecture framework based on XAML, XamlIl, MVVM and Rx.NET/ReactiveUI.

# Why xMvvm?
Most traditional 3D development approaches don't make a distinct separation between View, Data and functional logic (i.e. ViewModels/Controller/Presenter etc.). It's often just a bunch of components thrown together which makes development more complex, especially when introducing new features or refactoring them towards a better quality. Surely there are eventing/messaging concepts that introduce some kind of separation of concerns, but these come with their own set of challenges.

In many (if not all) .NET oriented ecosystems the XAML/MVVM combination has become the de-facto standard for separation of control between the more visual or functional oriented parts of a code base. And with a broad support and awareness of user interface controls and components development of bigger (i.e. enterprise-grade applications) has become easier than ever.

The aim of this project is to support 3D application development with well-known application architectural patterns based on MVVM and XAML. Currently most 3D applications are developed using one of the available 3D engines, but as these are based on high-performance Entity-Component-Systems paradigm it isn't easy to build professional, layered applications.

# Approach
Developing an approach that allows XAML and MVVM to be used for 3D applications isn't that easy. Because of this we're very happy to all proposals, ideas and bugfixes in any form whasoever. Feel free to create issues and/or provide us with push requests so that we can make this a reality.

The rough approach will happen in three phases:

1. Playground / Experimenting
   We need to find out what XAML/MVVM principles work and which don't. For this we'll build a few experimental setups in Unity 3D.

   Binding tests: 
   - To have a few basic View/ViewModel setups with traditional INotifyPropertyChanged binding.
   - To have a few basic View/ViewModel setups with traditional Rx.NET / ReactiveUI binding.
   - To have a few basic View/ViewModel setups with ECS oriented bindings.

   XAML structure:
   - To have a few setups with a XAML definition that leans more towards ECS.
   - To have a few setups with a XAML definition that leans more towards traditional MVVM Views.
   - To have a few setups with a XAML definition that leans more towards Unity prefabs.
   - To have a few setups with a pure XAML definition (i.e. entities+components instantiated based on the XAML).

   Technological aspects:
   - During edit time we want to have Unity 3D generate C# code when the XAML code is altered.
   - If possible we want to have compile time bindings. Not the old, traditional runtime-MVVM ones as available in WPF.
   - Performance is key. We want to find the right balance and move as much business logic into the VM layer, but need to make sure the ECS powered View part stays performant. 

2. Decide on the best fitting ingredients for a 3D XAML/MVVM architecture.

3. Define a 3D XAML standard.
   - That can work across different 3D engines. 
   - If possible: A setup in which ViewModels can be reused between 2D and 3D applications. 

# Additional documents

1. [XAML schema variations](XamlVariations.md)
2. [Thoughts on ECS to MVVM mapping](EcsMapping.md)
3. ...

# Used open source projects

- Snippets from ReactiveUI (MIT)

  https://github.com/reactiveui/ReactiveUI
