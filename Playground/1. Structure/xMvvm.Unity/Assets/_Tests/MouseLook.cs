namespace EtAlii.xMvvm
{
    using UnityEngine;

    /// MouseLook rotates the transform based on the mouse delta.
    /// Minimum and Maximum values can be used to constrain the possible rotation


    /// To make an FPS style character:
    /// - Create a capsule.
    /// - Add a rigid body to the capsule
    /// - Add the MouseLook script to the capsule.
    ///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
    /// - Add FPSWalker script to the capsule


    /// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
    /// - Add a MouseLook script to the camera.
    ///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
    [AddComponentMenu("Camera-Control/Mouse Look")]
    public class MouseLook : MonoBehaviour
    {


        public enum RotationAxes
        {
            MouseXAndY = 0,
            MouseX = 1,
            MouseY = 2
        }

        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;
        public float minimumX = -360F;
        public float maximumX = 360F;
        public float minimumY = -60F;
        public float maximumY = 60F;
        float _rotationX;
        float _rotationY;
        Quaternion _originalRotation;

#if UNITY_EDITOR
        private Vector2 _lastAxis;
#endif
        void FixedUpdate()
        {
#if UNITY_EDITOR
                Vector3 axis = new Vector3(-(_lastAxis.x - Input.mousePosition.x) * 0.1f, -(_lastAxis.y - Input.mousePosition.y) * 0.1f, Input.GetAxis("Mouse ScrollWheel"));
                _lastAxis = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                _rotationX += axis.x * sensitivityX;
                _rotationY += axis.y * sensitivityY;
#else             
                // Read the mouse input axis
                _rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                _rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
#endif           
                _rotationX = ClampAngle(_rotationX, minimumX, maximumX);
                _rotationY = ClampAngle(_rotationY, minimumY, maximumY);
                Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(_rotationY, -Vector3.right);
                transform.localRotation = _originalRotation * xQuaternion * yQuaternion;
        }

        void Start()
        {
            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
            _originalRotation = transform.localRotation;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}