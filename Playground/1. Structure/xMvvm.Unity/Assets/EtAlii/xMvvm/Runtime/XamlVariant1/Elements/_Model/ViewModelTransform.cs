namespace EtAlii.xMvvm.XamlVariant1
{
    using System.Numerics;

    public struct ViewModelTransform
    {
        public Vector3 LocalLocation { get; }
        public Quaternion LocalRotation { get; }
        public Vector3 LocalScale { get; }

        public ViewModelTransform(Vector3 localLocation, Quaternion localRotation, Vector3 localScale)
        {
            LocalLocation = localLocation;
            LocalRotation = localRotation;
            LocalScale = localScale;
        }

        public ViewModelTransform SetRotation(float localX, float localY, float localZ, float localW)
        {
            return new ViewModelTransform(LocalLocation, new Quaternion(localX, localY, localZ, localW), LocalScale);
        }

        public ViewModelTransform Move(Vector3 localMovement)
        {
            return new ViewModelTransform(LocalLocation + localMovement, LocalRotation, LocalScale);
        }

        public ViewModelTransform SetLocation(Vector3 location)
        {
            return new ViewModelTransform(location, LocalRotation, LocalScale);
        }

        public ViewModelTransform SetLocation(float localX, float localY, float localZ)
        {
            return new ViewModelTransform(new Vector3(localX, localY, localZ), LocalRotation, LocalScale);
        }

        public ViewModelTransform Move(float localX, float localY, float localZ)
        {
            return new ViewModelTransform(LocalLocation + new Vector3(localX, localY, localZ), LocalRotation, LocalScale);
        }
        
        public ViewModelTransform Scale(Vector3 localScale)
        {
            return new ViewModelTransform(LocalLocation, LocalRotation, LocalScale + localScale);
        }

        public ViewModelTransform SetScale(float scaleX, float scaleY, float scaleZ)
        {
            return new ViewModelTransform(LocalLocation, LocalRotation, new Vector3(scaleX, scaleY, scaleZ));
        }

        public ViewModelTransform Scale(float scaleX, float scaleY, float scaleZ)
        {
            return new ViewModelTransform(LocalLocation, LocalRotation, LocalScale + new Vector3(scaleX, scaleY, scaleZ));
        }
    }
}