namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Path),
        nameof(Member),
        nameof(Type),
        nameof(ElementType),
        
        nameof(Value),
    })]
    public class Property : ComponentElement
    {
        public string Value { get; set; }
        public Property()
            : base(ElementType.Property)
        {
        }

    }
}