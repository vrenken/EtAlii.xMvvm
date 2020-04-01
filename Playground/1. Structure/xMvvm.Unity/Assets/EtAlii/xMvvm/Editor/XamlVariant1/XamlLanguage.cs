[assembly: EtAlii.xMvvm.XmlnsDefinition("EtAlii.xMvvm", "EtAlii.xMvvm")]
namespace EtAlii.xMvvm
{
    using System;

    public class ContentAttribute : Attribute
    {
        
    }

    public class XmlnsDefinitionAttribute : Attribute
    {
        public XmlnsDefinitionAttribute(
            // ReSharper disable once UnusedParameter.Local
            string xmlNamespace, 
            // ReSharper disable once UnusedParameter.Local
            string clrNamespace)
        {
            
        }
    }

    public class UsableDuringInitializationAttribute : Attribute
    {
        // ReSharper disable once UnusedParameter.Local
        public UsableDuringInitializationAttribute(bool usable)
        {
            
        }
    }

    public class DeferredContentAttribute : Attribute
    {
        
    }
    //
    public interface IViewRootObjectProvider
    {
        object RootObject { get; }
    }
    //
    public interface IViewProvideValueTarget
    {
         object TargetObject { get; }
         object TargetProperty { get; }
     }
    
    public interface IViewUriContext
    {
        Uri BaseUri { get; set; }
    }
}