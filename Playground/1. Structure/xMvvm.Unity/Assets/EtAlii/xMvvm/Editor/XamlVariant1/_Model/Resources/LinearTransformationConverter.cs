namespace EtAlii.xMvvm
{
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