namespace EtAlii.xMvvm.XamlVariant1
{
    using System.Numerics;

    public struct ViewModelTransform
    {
        public Vector3 LocalPosition { get; }
        public Quaternion LocalRotation { get; }
        public Vector3 LocalScale { get; }
        
        public static ViewModelTransform Empty { get; } = new ViewModelTransform(Vector3.Zero, Quaternion.Identity, Vector3.One);
        private ViewModelTransform(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
            LocalScale = localScale;
        }

        public ViewModelTransform SetRotation(float localX, float localY, float localZ, float localW)
        {
            return new ViewModelTransform(LocalPosition, new Quaternion(localX, localY, localZ, localW), LocalScale);
        }

        public ViewModelTransform Move(Vector3 localMovement)
        {
            return new ViewModelTransform(LocalPosition + localMovement, LocalRotation, LocalScale);
        }

        public ViewModelTransform SetPosition(Vector3 location)
        {
            return new ViewModelTransform(location, LocalRotation, LocalScale);
        }

        public ViewModelTransform SetPosition(float localX, float localY, float localZ)
        {
            return new ViewModelTransform(new Vector3(localX, localY, localZ), LocalRotation, LocalScale);
        }

        public ViewModelTransform Move(float localX, float localY, float localZ)
        {
            return new ViewModelTransform(LocalPosition + new Vector3(localX, localY, localZ), LocalRotation, LocalScale);
        }
        
        public ViewModelTransform Scale(Vector3 localScale)
        {
            return new ViewModelTransform(LocalPosition, LocalRotation, LocalScale + localScale);
        }

        public ViewModelTransform SetScale(float scaleX, float scaleY, float scaleZ)
        {
            return new ViewModelTransform(LocalPosition, LocalRotation, new Vector3(scaleX, scaleY, scaleZ));
        }

        public ViewModelTransform Scale(float scaleX, float scaleY, float scaleZ)
        {
            return new ViewModelTransform(LocalPosition, LocalRotation, LocalScale + new Vector3(scaleX, scaleY, scaleZ));
        }
    }
}