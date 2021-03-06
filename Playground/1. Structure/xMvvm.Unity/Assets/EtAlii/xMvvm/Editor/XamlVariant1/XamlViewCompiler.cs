﻿namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using Mono.Cecil;
    using UnityEngine;
    using XamlIl.Ast;
    using XamlIl.Parsers;
    using XamlIl.Runtime;
    using XamlIl.Transform;
    using XamlIl.TypeSystem;
    using Assembly = System.Reflection.Assembly;
    using Object = System.Object;
    using TypeAttributes = Mono.Cecil.TypeAttributes;

    public class XamlViewCompiler 
    {
        private const string GeneratedNameSpace = "EtAlii.xMvvm.Generated";
        private const string GeneratedAssemblyNamePrefix = "EtAlii.xMvvm.Generated";
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
            try
            {
                return CompileInternal(xaml);
            }
            catch (Exception e)
            {
                xaml = DebugHelper.AddLineNumbers(xaml);
                
                // And throw both errors.
                Debug.LogError(e.GetType().Name + ": " + e.Message + Environment.NewLine + Environment.NewLine + xaml);
                return default;
            }
        }

        private (Func<IServiceProvider, object> create, Action<IServiceProvider, object> populate) CompileInternal(string xaml)
        {
            // Let's build ourselves a typesystem and compiler configuration.
            // We're going to do this for each compile action as we don't want to have anything 
            // cached in the Unity subsystems.
            var references = new List<string>(new [] 
            {
                typeof(Object).Assembly.Location,
                typeof(XamlViewCompiler).Assembly.Location,
                typeof(IServiceProvider).Assembly.Location,
                typeof(ITypeDescriptorContext).Assembly.Location,
                typeof(IXamlIlParentStackProviderV1).Assembly.Location,
            });
            
            // We need to have the compiled code from the Unity program.
            // This might load a lot of assemblies but it is needed to make sure we create the correct source code.
            var assembly = Assembly.LoadFrom("Library/ScriptAssemblies/Assembly-CSharp.dll");
            references.Add(assembly.Location);
            references.AddRange(assembly.GetReferencedAssemblies().Select(reference => Assembly.Load(reference).Location));

            var typeSystem = new CecilTypeSystem(references);
            
            var defaultAssembly = typeSystem.FindAssembly(typeof(XamlViewCompiler).Assembly.FullName);
            
            var languageTypeMappings = new XamlIlLanguageTypeMappings(typeSystem)
            {
                XmlnsAttributes = {typeSystem.GetType(typeof(XmlnsDefinitionAttribute).FullName)},
                ContentAttributes = {typeSystem.GetType(typeof(ContentAttribute).FullName)},
                UsableDuringInitializationAttributes = {typeSystem.GetType(typeof(UsableDuringInitializationAttribute).FullName)},
                DeferredContentPropertyAttributes = { typeSystem.GetType(typeof(DeferredContentAttribute).FullName) },
                //DeferredContentExecutorCustomization = typeSystem.GetType(typeof(XamlViewCompiler).FullName).FindMethod(m => m.Name == "CustomizeDeferredContent"),
                
                RootObjectProvider = typeSystem.GetType(typeof(IViewRootObjectProvider).FullName),
                UriContextProvider = typeSystem.GetType(typeof(IViewUriContext).FullName),
                ProvideValueTarget = typeSystem.GetType(typeof(IViewProvideValueTarget).FullName),
                
                ParentStackProvider = typeSystem.GetType(typeof(IXamlIlParentStackProviderV1).FullName),
                XmlNamespaceInfoProvider = typeSystem.GetType(typeof(IXamlIlXmlNamespaceInfoProviderV1).FullName)
            };
            var configuration = new XamlIlTransformerConfiguration(typeSystem, defaultAssembly, languageTypeMappings);

            var generatedAssemblyName = $"{GeneratedAssemblyNamePrefix}_{Guid.NewGuid():n}";
            var assemblyDefinition = typeSystem.CreateAndRegisterAssembly(generatedAssemblyName, new Version(1, 0),
                ModuleKind.Dll);

            var classTypeDefinition = new TypeDefinition(GeneratedNameSpace, GeneratedViewClass, TypeAttributes.Class | TypeAttributes.Public, assemblyDefinition.MainModule.TypeSystem.Object);

            var mainModuleTypeDefinition = new TypeDefinition(GeneratedNameSpace, "XamlContext", TypeAttributes.Class, assemblyDefinition.MainModule.TypeSystem.Object);
            assemblyDefinition.MainModule.Types.Add(mainModuleTypeDefinition);
            var ctb = typeSystem.CreateTypeBuilder(mainModuleTypeDefinition);
            var contextTypeDef = XamlIlContextDefinition.GenerateContextClass(ctb, typeSystem, configuration.TypeMappings);
            
            assemblyDefinition.MainModule.Types.Add(classTypeDefinition);


            var tb = typeSystem.CreateTypeBuilder(classTypeDefinition);
            var _ = Compile(tb, configuration, contextTypeDef, xaml);

            using (var memoryStream = new MemoryStream())
            {
                assemblyDefinition.Write(memoryStream);
                var data = memoryStream.ToArray();
            
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
        
        // public static Func<IServiceProvider, object> CustomizeDeferredContent(
        //     Func<IServiceProvider, object> builder, 
        //     IServiceProvider parentServices)
        // {
        //     return builder;
        //     
        //     //var parentRoot = ((IViewRootObjectProvider)parentServices.GetService(typeof(IViewRootObjectProvider))).RootObject;
        //     //var cb = parentServices.GetService(typeof(CallbackExtensionCallback));
        //
        //     // return sp => builder(new DictionaryServiceProvider
        //     // {
        //     //     [typeof(ITestRootObjectProvider)] = new ConstantRootObjectProvider {RootObject = parentRoot},
        //     //     [typeof(CallbackExtensionCallback)] = cb,
        //     //     Parent = sp
        //     // });
        //     // return null;
        // }

    }
}
