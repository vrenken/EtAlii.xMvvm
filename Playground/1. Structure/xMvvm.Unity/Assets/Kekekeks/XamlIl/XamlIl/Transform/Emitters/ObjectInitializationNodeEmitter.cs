using System.Collections.Generic;
using System.Reflection.Emit;
using XamlIl.Ast;
using XamlIl.TypeSystem;

namespace XamlIl.Transform.Emitters
{
#if !XAMLIL_INTERNAL
    public
#endif
    class ObjectInitializationNodeEmitter : IXamlIlAstNodeEmitter
    {
        public XamlIlNodeEmitResult Emit(IXamlIlAstNode node, XamlIlEmitContext context, IXamlIlEmitter codeGen)
        {
            if (!(node is XamlIlObjectInitializationNode init))
                return null;
            var supportInitType = context.Configuration.TypeMappings.SupportInitialize;
            var supportsInitialize = supportInitType != null
                                     && context.Configuration.TypeMappings.SupportInitialize
                                         .IsAssignableFrom(init.Type);

            if (supportsInitialize)
            {

                codeGen
                    // We need a copy for/EndInit
                    .Emit(OpCodes.Dup);
                if (!init.SkipBeginInit)
                    codeGen
                        .Emit(OpCodes.Dup)
                        .EmitCall(supportInitType.FindMethod(m => m.Name == "BeginInit"));
            }
            
            
            var addToParentStack = context.RuntimeContext.ParentListField != null
                                   && !init.Type.IsValueType
                                   && context.GetOrCreateItem<XamlIlNeedsParentStackCache>().NeedsParentStack(node);
            if(addToParentStack)
            {
                using (var local = context.GetLocal(init.Type))
                    codeGen
                        .Stloc(local.Local)
                        .Ldloc(context.ContextLocal)
                        .Ldloc(local.Local)
                        .EmitCall(context.RuntimeContext.PushParentMethod)
                        .Ldloc(local.Local);
            }

            context.Emit(init.Manipulation, codeGen, null);

            if (addToParentStack)
            {
                codeGen
                    .Ldloc(context.ContextLocal)
                    .EmitCall(context.RuntimeContext.PopParentMethod, true);
            }
            
            if (supportsInitialize)
                codeGen
                    .EmitCall(supportInitType.FindMethod(m => m.Name == "EndInit"));
            
            
            return XamlIlNodeEmitResult.Void(1);
        }
    }
    
    class XamlIlNeedsParentStackCache : Dictionary<IXamlIlAstNode, bool>
    {
        public static void Verify(XamlIlContextBase ctx, IXamlIlAstNode node)
        {
            var cache = ctx.GetItem<XamlIlNeedsParentStackCache>();
            // There is no parent stack
            if (cache == null)
                return;
            if (!cache.ContainsKey(node))
                throw new XamlIlLoadException("Node needs parent stack, but one doesn't seem to be provided", node);
        }
        class ParentStackVisitor : IXamlIlAstVisitor
        {
            private readonly XamlIlNeedsParentStackCache _cache;

            public ParentStackVisitor(XamlIlNeedsParentStackCache cache)
            {
                _cache = cache;
            }
            Stack<IXamlIlAstNode> _parents = new Stack<IXamlIlAstNode>();
            public IXamlIlAstNode Visit(IXamlIlAstNode node)
            {
                if (_cache.ContainsKey(node))
                    return node;
                if (node is IXamlIlAstNodeNeedsParentStack nps && nps.NeedsParentStack)
                {
                    _cache[node] = true;
                    foreach (var parent in _parents)
                        _cache[parent] = true;
                }
                else
                    _cache[node] = false;

                return node;
            }

            public void Push(IXamlIlAstNode node) => _parents.Push(node);

            public void Pop() => _parents.Pop();
        }

        public bool NeedsParentStack(IXamlIlAstNode node)
        {
            if (TryGetValue(node, out var rv))
                return rv;
            node.Visit(new ParentStackVisitor(this));
            return this[node];
        }
    }
}
