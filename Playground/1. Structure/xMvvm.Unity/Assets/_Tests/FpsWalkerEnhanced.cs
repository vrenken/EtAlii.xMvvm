namespace EtAlii.xMvvm
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(CharacterController))]
    public class FpsWalkerEnhanced : MonoBehaviour
    {
        [Tooltip("How fast the player moves when walking (default move speed).")] [SerializeField]
        private float walkSpeed = 6.0f;

        [Tooltip("How fast the player moves when running.")] [SerializeField]
        private float runSpeed = 11.0f;

        [Tooltip(
            "If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster.")]
        [SerializeField]
        public bool limitDiagonalSpeed = true;

        [Tooltip(
            "If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down.")]
        [SerializeField]
        private bool toggleRun;

        [Tooltip("How high the player jumps when hitting the jump button.")] [SerializeField]
        private float jumpSpeed = 8.0f;

        [Tooltip("How fast the player falls when not standing on anything.")] [SerializeField]
        private float gravity = 20.0f;

        [Tooltip(
            "Units that player can fall before a falling function is run. To disable, type \"infinity\" in the inspector.")]
        [SerializeField]
        private float fallingThreshold = 10.0f;

        [Tooltip(
            "If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down.")]
        [SerializeField]
        private bool slideWhenOverSlopeLimit;

        [Tooltip(
            "If checked and the player is on an object tagged \"Slide\", he will slide down it regardless of the slope limit.")]
        [SerializeField]
        private bool slideOnTaggedObjects;

        [Tooltip("How fast the player slides when on slopes as defined above.")] [SerializeField]
        private float slideSpeed = 12.0f;

        [Tooltip("If checked, then the player can change direction while in the air.")] [SerializeField]
        private bool airControl;

        [Tooltip(
            "Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast.")]
        [SerializeField]
        private float antiBumpFactor = .75f;

        [Tooltip(
            "Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping.")]
        [SerializeField]
        private int antiBunnyHopFactor = 1;

        private Vector3 _moveDirection = Vector3.zero;
        private bool _grounded;
        private CharacterController _controller;
        private Transform _transform;
        private float _speed;
        private RaycastHit _hit;
        private float _fallStartLevel;
        private bool _falling;
        private float _slideLimit;
        private float _rayDistance;
        private Vector3 _contactPoint;
        private bool _playerControl;
        private int _jumpTimer;


        private void Start()
        {
            // Saving component references to improve performance.
            _transform = GetComponent<Transform>();
            _controller = GetComponent<CharacterController>();

            // Setting initial values.
            _speed = walkSpeed;
            _rayDistance = _controller.height * .5f + _controller.radius;
            _slideLimit = _controller.slopeLimit - .1f;
            _jumpTimer = antiBunnyHopFactor;
        }


        private void Update()
        {
            // If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
            // FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)
            if (toggleRun && _grounded && Input.GetButtonDown($"Run"))
            {
                _speed = (Math.Abs(_speed - walkSpeed) < 0.01f ? runSpeed : walkSpeed);
            }
        }


        private void FixedUpdate()
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
            float inputModifyFactor = (Math.Abs(inputX) > 0.01f && Math.Abs(inputY) > 0.01f && limitDiagonalSpeed) ? .7071f : 1.0f;

            if (_grounded)
            {
                bool sliding = false;
                // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
                // because that interferes with step climbing amongst other annoyances
                if (Physics.Raycast(_transform.position, -Vector3.up, out _hit, _rayDistance))
                {
                    if (Vector3.Angle(_hit.normal, Vector3.up) > _slideLimit)
                    {
                        sliding = true;
                    }
                }
                // However, just raycasting straight down from the center can fail when on steep slopes
                // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
                else
                {
                    Physics.Raycast(_contactPoint + Vector3.up, -Vector3.up, out _hit);
                    if (Vector3.Angle(_hit.normal, Vector3.up) > _slideLimit)
                    {
                        sliding = true;
                    }
                }

                // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                if (_falling)
                {
                    _falling = false;
                    if (_transform.position.y < _fallStartLevel - fallingThreshold)
                    {
                        OnFell(_fallStartLevel - _transform.position.y);
                    }
                }

                // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
                if (!toggleRun)
                {
                    _speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
                }

                // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
                if ((sliding && slideWhenOverSlopeLimit) || slideOnTaggedObjects && _hit.collider.CompareTag($"Slide"))
                {
                    Vector3 hitNormal = _hit.normal;
                    _moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                    Vector3.OrthoNormalize(ref hitNormal, ref _moveDirection);
                    _moveDirection *= slideSpeed;
                    _playerControl = false;
                }
                // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
                else
                {
                    _moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor,
                        inputY * inputModifyFactor);
                    _moveDirection = _transform.TransformDirection(_moveDirection) * _speed;
                    _playerControl = true;
                }

                // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
                if (!Input.GetButton("Jump"))
                {
                    _jumpTimer++;
                }
                else if (_jumpTimer >= antiBunnyHopFactor)
                {
                    _moveDirection.y = jumpSpeed;
                    _jumpTimer = 0;
                }
            }
            else
            {
                // If we stepped over a cliff or something, set the height at which we started falling
                if (!_falling)
                {
                    _falling = true;
                    _fallStartLevel = _transform.position.y;
                }

                // If air control is allowed, check movement but don't touch the y component
                if (airControl && _playerControl)
                {
                    _moveDirection.x = inputX * _speed * inputModifyFactor;
                    _moveDirection.z = inputY * _speed * inputModifyFactor;
                    _moveDirection = _transform.TransformDirection(_moveDirection);
                }
            }

            // Apply gravity
            _moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            _grounded = (_controller.Move(_moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }


        // Store point that we're in contact with for use in FixedUpdate if needed
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _contactPoint = hit.point;
        }


        // This is the place to apply things like fall damage. You can give the player hitpoints and remove some
        // of them based on the distance fallen, play sound effects, etc.
        private void OnFell(float fallDistance)
        {
            print("Ouch! Fell " + fallDistance + " units!");
        }
    }
}