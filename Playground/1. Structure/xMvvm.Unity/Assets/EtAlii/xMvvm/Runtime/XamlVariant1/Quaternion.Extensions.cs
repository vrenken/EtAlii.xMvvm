namespace EtAlii.xMvvm.XamlVariant1
{
    using UnityEngine;

    public static class QuaternionExtensions
    {
        public static Quaternion ToUnity(this System.Numerics.Quaternion quaternion) => new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        public static System.Numerics.Quaternion ToNumerics(this Quaternion quaternion) => new System.Numerics.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}