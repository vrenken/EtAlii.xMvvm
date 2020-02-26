using System.Reflection.Emit;
using XamlIl.Ast;
using XamlIl.TypeSystem;

namespace XamlIl.Transform.Emitters
{
#if !XAMLIL_INTERNAL
    public
#endif
    class ValueWithManipulationsEmitter : IXamlIlAstNodeEmitter
    {
        public XamlIlNodeEmitResult Emit(IXamlIlAstNode node, XamlIlEmitContext context, IXamlIlEmitter codeGen)
        {
            if (!(node is XamlIlValueWithManipulationNode vwm))
                return null;
            var created = context.Emit(vwm.Value, codeGen, vwm.Type.GetClrType());

            if (vwm.Manipulation != null &&
                !(vwm.Manipulation is XamlIlManipulationGroupNode grp && grp.Children.Count == 0))
            {
                codeGen.Emit(OpCodes.Dup);
                context.Emit(vwm.Manipulation, codeGen, null);
            }
            return XamlIlNodeEmitResult.Type(0, created.ReturnType);
        }
    }
}
