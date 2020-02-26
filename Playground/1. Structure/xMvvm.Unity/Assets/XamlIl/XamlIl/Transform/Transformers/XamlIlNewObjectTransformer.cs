using System.Collections.Generic;
using System.Linq;
using XamlIl.Ast;
using XamlIl.Transform.Emitters;
using XamlIl.TypeSystem;

namespace XamlIl.Transform.Transformers
{
#if !XAMLIL_INTERNAL
    public
#endif
    class XamlIlNewObjectTransformer : IXamlIlAstTransformer
    {
        IXamlIlConstructor TransformArgumentsAndGetConstructor(XamlIlAstTransformationContext context,
            XamlIlAstObjectNode n)
        {
            var type = n.Type.GetClrType();
               
            var argTypes = n.Arguments.Select(a => a.Type.GetClrType()).ToList();
            var ctor = type.FindConstructor(argTypes);
            if (ctor == null)
            {
                if (argTypes.Count != 0)
                {
                    ctor = type.Constructors.FirstOrDefault(x =>
                        !x.IsStatic && x.IsPublic && x.Parameters.Count == argTypes.Count);
                    
                }

                if (ctor == null)
                    throw new XamlIlLoadException(
                        $"Unable to find public constructor for type {type.GetFqn()}({string.Join(", ", argTypes.Select(at => at.GetFqn()))})",
                        n);
            }

            for (var c = 0; c < n.Arguments.Count; c++)
            {
                if (!XamlIlTransformHelpers.TryGetCorrectlyTypedValue(context, n.Arguments[c], ctor.Parameters[c], out var arg))
                    throw new XamlIlLoadException(
                        $"Unable to convert {n.Arguments[c].Type.GetClrType().GetFqn()} to {ctor.Parameters[c].GetFqn()} for constructor of {n.Type.GetClrType().GetFqn()}",
                        n.Arguments[c]);
                n.Arguments[c] = arg;
            }

            return ctor;
        }
        
        public IXamlIlAstNode Transform(XamlIlAstTransformationContext context, IXamlIlAstNode node)
        {
            if (node is XamlIlAstObjectNode ni)
            {
                var t = ni.Type.GetClrType();
                if (t.IsValueType)
                    throw new XamlIlLoadException(
                        "Value types can only be loaded via converters. We don't want to mess with ldloca.s, ldflda and other weird stuff",
                        node);

                var ctor = TransformArgumentsAndGetConstructor(context, ni);                
                return new XamlIlValueWithManipulationNode(ni,
                    new XamlIlAstNewClrObjectNode(ni, ni.Type.GetClrTypeReference(), ctor, ni.Arguments),
                    new XamlIlObjectInitializationNode(ni,
                        new XamlIlManipulationGroupNode(ni)
                        {
                            Children = ni.Children.Cast<IXamlIlAstManipulationNode>().ToList()
                        }, ni.Type.GetClrType()));
            }

            return node;
        }
    }
}
