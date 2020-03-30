namespace EtAlii.xMvvm.XamlVariant1
{
    using UnityEngine;

    public static class Vector3Extensions
    {
        public static Vector3 ToUnity(this System.Numerics.Vector3 vector3) => new Vector3(vector3.X, vector3.Y, vector3.Z);
        public static System.Numerics.Vector3 ToNumerics(this Vector3 vector3) => new System.Numerics.Vector3(vector3.x, vector3.y, vector3.z);
    }
}