namespace EtAlii.xMvvm
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using Mono.Cecil;
    using XamlIl.Ast;
    using XamlIl.Parsers;
    using XamlIl.Runtime;
    using XamlIl.Transform;
    using XamlIl.TypeSystem;
    using Object = System.Object;
    using TypeAttributes = Mono.Cecil.TypeAttributes;

    public class XamlViewCompiler 
    {
        private const string GeneratedNameSpace = "EtAlii.xMvvm.Generated";
        private const string GeneratedAssemblyName = "EtAlii.xMvvm.Generated";
        private const string GeneratedViewClass = "GeneratedView";
        
        private XamlIlDocument Compile(IXamlIlTypeBuilder builder, XamlIlTransformerConfiguration configuration, IXamlIlType context, string xaml)
        {
            var parsed = XDocumentXamlIlParser.Parse(xaml);
            var compiler = new XamlIlCompiler(configuration, true) { EnableIlVerification = true };
            compiler.Transform(parsed);
            compiler.Compile(parsed, builder, context, "Populate", "Build", "XamlIlNamespaceInfo", "http://example.com/", null);
            return parsed;
        }
        
        
        public (Func<IServiceProvider, object> create, Action<IServiceProvider, object> populate) Compile(string xaml)
        {
            // Let's build ourselves a typesystem and compiler configuration.
            // We're going to do this for each compile action as we don't want to have anything 
            // cached in the Unity subsystems.
            var references = new[] 
            {
                typeof(Object).Assembly.Location,
                typeof(XamlViewCompiler).Assembly.Location,
                typeof(IServiceProvider).Assembly.Location,
                typeof(ITypeDescriptorContext).Assembly.Location,
                typeof(IXamlIlParentStackProviderV1).Assembly.Location,
            };

            var typeSystem = new CecilTypeSystem(references);
            
            var defaultAssembly = typeSystem.FindAssembly(typeof(XamlViewCompiler).Assembly.FullName);
            
            var languageTypeMappings = new XamlIlLanguageTypeMappings(typeSystem)
            {
                XmlnsAttributes = {typeSystem.GetType(typeof(XmlnsDefinitionAttribute).FullName)},
                ContentAttributes = {typeSystem.GetType(typeof(ContentAttribute).FullName)},
                UsableDuringInitializationAttributes = {typeSystem.GetType(typeof(UsableDuringInitializationAttribute).FullName)},
                DeferredContentPropertyAttributes = { typeSystem.GetType(typeof(DeferredContentAttribute).FullName) },
                
                RootObjectProvider = typeSystem.GetType(typeof(IViewRootObjectProvider).FullName),
                UriContextProvider = typeSystem.GetType(typeof(IViewUriContext).FullName),
                ProvideValueTarget = typeSystem.GetType(typeof(IViewProvideValueTarget).FullName),
                
                ParentStackProvider = typeSystem.GetType(typeof(IXamlIlParentStackProviderV1).FullName),
                XmlNamespaceInfoProvider = typeSystem.GetType(typeof(IXamlIlXmlNamespaceInfoProviderV1).FullName)
            };
            var configuration = new XamlIlTransformerConfiguration(typeSystem, defaultAssembly, languageTypeMappings);

            var asm = typeSystem.CreateAndRegisterAssembly(GeneratedAssemblyName, new Version(1, 0),
                ModuleKind.Dll);

            var def = new TypeDefinition(GeneratedNameSpace, GeneratedViewClass,
                TypeAttributes.Class | TypeAttributes.Public, asm.MainModule.TypeSystem.Object);

            var ct = new TypeDefinition(GeneratedNameSpace, "XamlContext", TypeAttributes.Class,
                asm.MainModule.TypeSystem.Object);
            asm.MainModule.Types.Add(ct);
            var ctb = typeSystem.CreateTypeBuilder(ct);
            var contextTypeDef = XamlIlContextDefinition.GenerateContextClass(ctb, typeSystem, configuration.TypeMappings);
            
            asm.MainModule.Types.Add(def);


            var tb = typeSystem.CreateTypeBuilder(def);
            var _ = Compile(tb, configuration, contextTypeDef, xaml);

            using (var ms = new MemoryStream())
            {
                asm.Write(ms);
                var data = ms.ToArray();
            
                var loaded = Assembly.Load(data);
                var t = loaded.GetType($"{GeneratedNameSpace}.{GeneratedViewClass}");

                return GetCallbacks(t);
            }
        }

        private (Func<IServiceProvider, object> create, Action<IServiceProvider, object> populate) GetCallbacks(Type created)
        {
            var isp = Expression.Parameter(typeof(IServiceProvider));
            var createCb = Expression.Lambda<Func<IServiceProvider, object>>(
                    // ReSharper disable once AssignNullToNotNullAttribute
                Expression.Convert(Expression.Call(created.GetMethod("Build"), isp), typeof(object)), isp).Compile();
            
            var epar = Expression.Parameter(typeof(object));
            var populate = created.GetMethod("Populate");
            isp = Expression.Parameter(typeof(IServiceProvider));
            var populateCb = Expression.Lambda<Action<IServiceProvider, object>>(
                Expression.Call(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    populate, isp, 
                    Expression.Convert(
                        epar, 
                        // ReSharper disable once PossibleNullReferenceException
                        populate.GetParameters()[1].ParameterType)),
                isp, epar).Compile();
            
            return (createCb, populateCb);
        }
    }
}
