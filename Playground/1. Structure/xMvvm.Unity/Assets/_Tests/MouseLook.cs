namespace EtAlii.xMvvm.Tests
{
    using UnityEngine;
    using Cursor = UnityEngine.Cursor;

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
        public float sensitivity = 15f;
        private Vector2 _mouseLook;
        private GameObject _character;

#if UNITY_EDITOR
        private Vector2 _lastAxis;
#endif
        void Update()
        {
            if (Input.GetMouseButton(1)) // = right button down.
            {
                Cursor.lockState = CursorLockMode.Locked;
                
#if FALSE //UNITY_EDITOR
                var look = new Vector2(-(_lastAxis.x - Input.mousePosition.x) * 0.1f, -(_lastAxis.y - Input.mousePosition.y) * 0.1f);
                _lastAxis = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else             
                // Read the mouse input axis
                var horizontal = Input.GetAxis("Mouse X");
                var vertical = Input.GetAxis("Mouse Y");
                var look = new Vector2(horizontal, vertical);
#endif
                _mouseLook += look * sensitivity;
                _mouseLook.y = Mathf.Clamp(_mouseLook.y, -80f, +80f);
            }
            else
            {
                // We want to be able to interact with UI elements.
                Cursor.lockState = CursorLockMode.None;
            }

            transform.localRotation = Quaternion.AngleAxis(-_mouseLook.y, Vector3.right);
            _character.transform.localRotation = Quaternion.AngleAxis(_mouseLook.x, _character.transform.up);
        }
        void Start()
        {
            _character = transform.parent.gameObject;
        }
    }
}