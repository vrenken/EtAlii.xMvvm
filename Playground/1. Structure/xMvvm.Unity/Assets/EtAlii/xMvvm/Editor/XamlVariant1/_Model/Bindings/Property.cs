namespace EtAlii.xMvvm
{
    using DotLiquid;

    /// <summary>
    /// A Binding from ViewModel properties to Component properties or fields. 
    /// </summary>
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