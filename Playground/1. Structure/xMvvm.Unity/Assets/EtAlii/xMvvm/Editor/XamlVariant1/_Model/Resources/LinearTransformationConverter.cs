namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType(new[]
    {
        nameof(Key),
        nameof(Type),
        
        nameof(Direction),
        nameof(Scale)
    })]
    public class LinearTransformationConverter : Resource
    {
        public Direction Direction { get; set; } = Direction.X;
        public float Scale { get; set; } = 1f;
        
        public LinearTransformationConverter()
            : base(nameof(LinearTransformationConverter))
        {
        }
    }
}