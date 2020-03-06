namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Path),
        nameof(Member),
        nameof(Type),
        nameof(BindingType),
        
        nameof(Value),
    })]
    public class Property : ComponentBinding
    {
        public Bind Value { get; set; }
        
        public Property()
            : base(BindingType.Property)
        {
        }

    }
}