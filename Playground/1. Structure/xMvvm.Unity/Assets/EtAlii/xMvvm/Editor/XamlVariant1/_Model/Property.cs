namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Value),
        nameof(Path),
        nameof(Member),
        nameof(Type),
    })]
    public class Property : Element
    {
        [Content]
        public string Name { get; set; }
        
        public string Value { get; set; }
        public string Path { get; set; }
        public string Member { get; set; }
        
        public string Type { get; set; }
    }
}