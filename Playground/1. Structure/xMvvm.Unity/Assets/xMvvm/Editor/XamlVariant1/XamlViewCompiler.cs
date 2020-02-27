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
    using TypeAttributes = Mono.Cecil.TypeAttributes;

    public class XamlViewCompiler 
    {
        private readonly IXamlIlTypeSystem _typeSystem;
        public XamlIlTransformerConfiguration Configuration { get; }

        private static readonly object AsmLock = new object();

        private const string GeneratedNameSpace = "EtAlii.xMvvm.Generated";
        private const string GeneratedAssemblyName = "EtAlii.xMvvm.Generated";
        private const string GeneratedViewClass = "GeneratedView";
        
        public XamlViewCompiler()
        {
            var references = new[] 
            {
                typeof(Object).Assembly.Location,
                typeof(XamlViewCompiler).Assembly.Location,
                typeof(IServiceProvider).Assembly.Location,
                typeof(ITypeDescriptorContext).Assembly.Location,
                typeof(IXamlIlParentStackProviderV1).Assembly.Location,
            };

            //_typeSystem = new SreTypeSystem();
            _typeSystem = new CecilTypeSystem(references);
            
            var defaultAssembly = _typeSystem.FindAssembly(typeof(XamlViewCompiler).Assembly.FullName);
            
            var languageTypeMappings = new XamlIlLanguageTypeMappings(_typeSystem)
            {
                XmlnsAttributes = {_typeSystem.GetType(typeof(XmlnsDefinitionAttribute).FullName)},
                ContentAttributes = {_typeSystem.GetType(typeof(ContentAttribute).FullName)},
                UsableDuringInitializationAttributes = {_typeSystem.GetType(typeof(UsableDuringInitializationAttribute).FullName)},
                DeferredContentPropertyAttributes = { _typeSystem.GetType(typeof(DeferredContentAttribute).FullName) },
                
                RootObjectProvider = _typeSystem.GetType(typeof(IViewRootObjectProvider).FullName),
                UriContextProvider = _typeSystem.GetType(typeof(IViewUriContext).FullName),
                ProvideValueTarget = _typeSystem.GetType(typeof(IViewProvideValueTarget).FullName),
                
                ParentStackProvider = _typeSystem.GetType(typeof(IXamlIlParentStackProviderV1).FullName),
                XmlNamespaceInfoProvider = _typeSystem.GetType(typeof(IXamlIlXmlNamespaceInfoProviderV1).FullName)
            };
            Configuration = new XamlIlTransformerConfiguration(_typeSystem, defaultAssembly, languageTypeMappings);
        }
        
        private XamlIlDocument Compile(IXamlIlTypeBuilder builder, IXamlIlType context, string xaml)
        {
            var parsed = XDocumentXamlIlParser.Parse(xaml);
            var compiler = new XamlIlCompiler(Configuration, true) { EnableIlVerification = true };
            compiler.Transform(parsed);
            compiler.Compile(parsed, builder, context, "Populate", "Build", "XamlIlNamespaceInfo", "http://example.com/", null);
            return parsed;
        }
        
        
        public (Func<IServiceProvider, object> create, Action<IServiceProvider, object> populate) Compile(string xaml)
        {
            var ts = (CecilTypeSystem) (_typeSystem);
            var asm = ts.CreateAndRegisterAssembly(GeneratedAssemblyName, new Version(1, 0),
                ModuleKind.Dll);

            var def = new TypeDefinition(GeneratedNameSpace, GeneratedViewClass,
                TypeAttributes.Class | TypeAttributes.Public, asm.MainModule.TypeSystem.Object);

            var ct = new TypeDefinition(GeneratedNameSpace, "XamlContext", TypeAttributes.Class,
                asm.MainModule.TypeSystem.Object);
            asm.MainModule.Types.Add(ct);
            var ctb = ((CecilTypeSystem)_typeSystem).CreateTypeBuilder(ct);
            var contextTypeDef = XamlIlContextDefinition.GenerateContextClass(ctb, _typeSystem, Configuration.TypeMappings);
            
            asm.MainModule.Types.Add(def);


            var tb = ts.CreateTypeBuilder(def);
            Compile(tb, contextTypeDef, xaml);
            
            var ms = new MemoryStream();
            asm.Write(ms);
            var data = ms.ToArray();
            lock (AsmLock)
                File.WriteAllBytes(GeneratedAssemblyName+".dll", data);
            
            var loaded = Assembly.Load(data);
            var t = loaded.GetType($"{GeneratedNameSpace}.{GeneratedViewClass}");

            return GetCallbacks(t);
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
