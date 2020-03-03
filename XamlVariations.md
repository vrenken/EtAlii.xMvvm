# XAML Schema Variations

## XAML Variant 1 - Simple prefab wiring (Unity)
As many developers use Unity3D to develop 3D applications with this will be one of the first targets to develop a XAML/MVVM library and toolchain for. 
Within Unity there are three distinct concepts that play an important role to decide how to move forward:

- *Nodes hierarchies*

  Both 3D and 2D scenes are build of hierarchies of nodes that live in the same 2D/3D space. Each of them has a transformation (translation/rotation/scale) that takes into account the parent transformation as well.
- *Components*

  In Unity 3D (and most other ECS powered 3D engine) components are the decorating building blocks with which nodes can be given life. A component (MonoBehavior) can introduce both visible and non-visible expressions that cover a very, very broad range. 3D models, collissions, animations, texturing, effects and behavioral logic, you name it and it can be composed using nodes and one or more components. For simplicity reasons we don't dive into these matters deeper as they often are also a broad range of class instances working together, but to reason further towards 3d architectures we just classify components as the parts that make up a view or views from a 3D application.
- *Prefabs*

  As with any other application development reuse is an important aspect, and in Unity this is where the prefab mechanisms comes into play. It allows for sub-hierarchies of nodes and components to be re-used and applied multiple times.  

The idea of this XAML variation is to delegate the creation of the view onto prefabs, and allow a XAML file to create a compile-time map between the view and a viewmodel.

For this the following principles need to be implemented:

- Simple (i.e. value based) bindings
- List bindings
- View nesting



```XAML
<!-- Current setup -->
<View
    xmlns="xMvvm"    
    ViewModelType="LoginViewModel"        
    Prefab="LoginView.prefab">

    <Binding 
      Name="Password" 
      Component="PasswordTextBox" 
      ComponentProperty="Text" 
      ViewModelProperty="Password" 
      Mode="TwoWay" />       
    <Binding 
      Name="UserName" 
      Component="UserNameTextBox" 
      ComponentProperty="Text" 
      ViewModelProperty="UserName" 
      Mode="TwoWay" /> 
</View>   
```

```XAML
<!-- Aimed setup -->
<View
    xmlns="xMvvm"    
    ViewModelType="LoginViewModel"        
    Prefab="LoginView.prefab">

    <Binding 
      Name="Password" 
        Source="{x:Property Password}" 
        Target="{x:Component Path=Canvas/Panel/PasswordTextBox, Type=TMP_InputField, Member=Text}" 
        Mode="TwoWay">
    <Binding 
      Name="UserName" 
      Source="{x:Property UserName}" 
      Target="{x:Component Path=Canvas/Panel/UserNameTextBox, Type=TMP_InputField, Member=Text}" 
      Mode="TwoWay">
    <Binding 
      Name="Login" 
      Source="{x:Component Path=Canvas/Panel/LoginButton, Type=Button, Event=onClick}" 
      Target="{x:Method LoginClicked}">
</View>   
```

```XAML
<!-- Experiment 1 -->
<View
    xmlns="xMvvm"    
    ViewModelType="LoginViewModel"        
    Prefab="LoginView.prefab">

    <Property 
      Path="Panel/PasswordTextBox" 
      ComponentType="Label" 
      Property="Text" Value="{x:Bind Password}">
    <Event 
      Path="Panel/LoginButton" 
      ComponentType="Button" 
      Event="Clicked" 
      Handler="{x:Bind LoginClicked}">
</View>   
```

```XAML
<!-- Experiment 2 (Too much XAML needed) -->
<View
    xmlns="xMvvm"    
    ViewModelType="LoginViewModel"        
    Prefab="LoginView.prefab">

    <PropertyBinding Name="Password" Mode="TwoWay">
        <ComponentProperty Path="Canvas/Panel/PasswordTextBox" Type="TMP_InputField" Property="text" />
        <ViewModelProperty Property="UserName" />
    </PropertyBinding>
    <EventBinding Name="Login">
        <ComponentEvent Path="Panel/UserNameTextBox" Type="Button" Event="onClick" />
        <ViewModelMethod Property="Login" />
    </PropertyBinding>
</View>    
```
