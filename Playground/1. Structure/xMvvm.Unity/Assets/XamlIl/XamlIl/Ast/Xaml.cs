using System.Collections.Generic;
using System.Linq;
using XamlIl.TypeSystem;
using Visitor = XamlIl.Ast.IXamlIlAstVisitor;
namespace XamlIl.Ast
{

#if !XAMLIL_INTERNAL
    public
#endif
    class XamlIlAstXmlDirective : XamlIlAstNode, IXamlIlAstManipulationNode
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<IXamlIlAstValueNode> Values { get; set; }

        public XamlIlAstXmlDirective(IXamlIlLineInfo lineInfo,
            string ns, string name, IEnumerable<IXamlIlAstValueNode> values) : base(lineInfo)
        {
            Namespace = ns;
            Name = name;
            Values = values.ToList();
        }

        public override void VisitChildren(Visitor visitor)
        {
            VisitList(Values, visitor);
        }
    }

#if !XAMLIL_INTERNAL
    public
#endif
    class XamlIlAstXamlPropertyValueNode : XamlIlAstNode, IXamlIlAstManipulationNode
    {
        public IXamlIlAstPropertyReference Property { get; set; }
        public List<IXamlIlAstValueNode> Values { get; set; }

        public XamlIlAstXamlPropertyValueNode(IXamlIlLineInfo lineInfo,
            IXamlIlAstPropertyReference property, IXamlIlAstValueNode value) : base(lineInfo)
        {
            Property = property;
            Values = new List<IXamlIlAstValueNode> {value};
        }
        
        public XamlIlAstXamlPropertyValueNode(IXamlIlLineInfo lineInfo,
            IXamlIlAstPropertyReference property, IEnumerable<IXamlIlAstValueNode> values) : base(lineInfo)
        {
            Property = property;
            Values = values.ToList();
        }

        public override void VisitChildren(Visitor visitor)
        {
            Property = (IXamlIlAstPropertyReference) Property.Visit(visitor);
            VisitList(Values, visitor);
        }
    }

#if !XAMLIL_INTERNAL
    public
#endif
    class XamlIlAstObjectNode : XamlIlAstNode, IXamlIlAstValueNode
    {
        public XamlIlAstObjectNode(IXamlIlLineInfo lineInfo, IXamlIlAstTypeReference type) : base(lineInfo)
        {
            Type = type;
        }

        public IXamlIlAstTypeReference Type { get; set; }
        public List<IXamlIlAstNode> Children { get; set; } = new List<IXamlIlAstNode>();
        public List<IXamlIlAstValueNode> Arguments { get; set; } = new List<IXamlIlAstValueNode>();

        public override void VisitChildren(Visitor visitor)
        {
            Type = (IXamlIlAstTypeReference) Type.Visit(visitor);
            VisitList(Arguments, visitor);
            VisitList(Children, visitor);
        }
    }
    
    

#if !XAMLIL_INTERNAL
    public
#endif
    class XamlIlAstTextNode : XamlIlAstNode, IXamlIlAstValueNode
    {
        public string Text { get; set; }

        public XamlIlAstTextNode(IXamlIlLineInfo lineInfo, string text, IXamlIlType type = null) : base(lineInfo)
        {
            Text = text;
            if (type != null)
                Type = new XamlIlAstClrTypeReference(lineInfo, type, false);
            else
                Type = new XamlIlAstXmlTypeReference(lineInfo, XamlNamespaces.Xaml2006, "String");
        }

        public override void VisitChildren(Visitor visitor)
        {
            Type = (IXamlIlAstTypeReference) Type.Visit(visitor);
        }

        public IXamlIlAstTypeReference Type { get; set; }
    }
    
#if !XAMLIL_INTERNAL
    public
#endif
    class XamlIlAstNamePropertyReference : XamlIlAstNode, IXamlIlAstPropertyReference
    {
        public IXamlIlAstTypeReference DeclaringType { get; set; }
        public string Name { get; set; }
        public IXamlIlAstTypeReference TargetType { get; set; }

        public XamlIlAstNamePropertyReference(IXamlIlLineInfo lineInfo,
            IXamlIlAstTypeReference declaringType, string name, IXamlIlAstTypeReference targetType) : base(lineInfo)
        {
            DeclaringType = declaringType;
            Name = name;
            TargetType = targetType;
        }

        public override void VisitChildren(Visitor visitor)
        {
            DeclaringType = (IXamlIlAstTypeReference) DeclaringType.Visit(visitor);
            TargetType = (IXamlIlAstTypeReference) TargetType.Visit(visitor);
        }
    }
}
