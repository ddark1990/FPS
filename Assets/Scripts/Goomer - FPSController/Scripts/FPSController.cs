using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoomerFPSController
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class FPSController : MonoBehaviour
    {
        [Header("SpeedConfig")]
        [Range(1, 25)] public float AccelerationSpeed = 2;
        [Range(1, 25)] public float WalkForwardSpeed = 4;
        [Range(1, 25)] public float StrafeSpeed = 3;
        [Range(1, 25)] public float WalkBackwardSpeed = 2;
        [Range(1,5)] public float SprintMultiplier = 1.5f;
        [Header("JumpConfig")]
        [Range(1,25)] public float JumpHeight = 2f;
        [Range(1,10)] public float FallMultiplier = 2.5f;
        [Range(1,10)] public float LowJumpMultiplier = 2f;
        [Header("MouseConfig")]
        [Range(1, 10)] public float MouseSensetivity = 2f;
        public bool Smooth = true;
        [Range(1, 25)] public float SmoothFloat = 5;
        [Header("MaskConfig")]
        public LayerMask GroundMask;

        [Space]
        [Header("Private var")]
        [SerializeField] private bool IsGrounded;
        [SerializeField] private bool IsWalking;
        [SerializeField] private bool IsSprinting;
        [SerializeField] private bool IsJumping;
        [SerializeField] private bool cursorIsLocked;
        [SerializeField] private float accelerationOutput;
        [SerializeField] private float walkSpeedOutput;
        [SerializeField] private Vector2 axisInput;
        [SerializeField] private Vector2 mouseInput;
        [SerializeField] private float sphereSize;
        [Space]
        public bool DrawDebugRays;
        public PhysicMaterial ZeroFrictionMaterial;

        private CapsuleCollider capsuleCollider;
        private Rigidbody rb;
        private Vector3 colliderBottom;
        private Vector3 moveDirection;
        private Camera cam;

        private void Init()
        {
            GetComponents();

        }
        private void GetComponents()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            cam = GetComponentInChildren<Camera>();
        }
        private void OnEnable()
        {
            Init();
        }

        private void FixedUpdate()
        {
            IsGrounded = ControllerInput.IsGrounded(capsuleCollider, GroundMask);
            CheckWall();
            MovePlayer(axisInput, walkSpeedOutput);

            Jump(JumpHeight);
            RotatePlayer();
        }

        private void Update()
        {
            axisInput = ControllerInput.GetAxisInputs(); //grabs the axis from unity's Input in project settings
            mouseInput = ControllerInput.GetMouseInputs(); //grabs the axis from unity's Input in project settings
            accelerationOutput = ControllerInput.AccelerationOutput(AccelerationSpeed);
            walkSpeedOutput = ControllerInput.MovementSpeedOutput(axisInput, WalkForwardSpeed, WalkBackwardSpeed, StrafeSpeed, accelerationOutput, SprintMultiplier); //AWSD keys

            InternalLockUpdate();
        }

        private void LateUpdate()
        {
            RotateCamera();
        }

        private void MovePlayer(Vector2 inputAxis, float _walkSpeedOutput)
        {
            IsWalking = ControllerInput.WalkOutput();
            if (IsSprinting) IsWalking = false;
            IsSprinting = ControllerInput.SprintOutput();

            moveDirection = new Vector3(axisInput.x, 0, axisInput.y);
            moveDirection = transform.TransformDirection(moveDirection);

            rb.AddForce(transform.position + moveDirection * Time.deltaTime * _walkSpeedOutput);
        }

        private void RotatePlayer()
        {
            var yRot = mouseInput.x * MouseSensetivity;

            var playerTargetRot = transform.localRotation * Quaternion.Euler(0, yRot, 0);

            if(Smooth) transform.localRotation = Quaternion.Slerp(transform.localRotation, playerTargetRot, SmoothFloat * Time.deltaTime);
            else rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles + new Vector3(0f, yRot, 0f)));
        }

        private void RotateCamera()
        {
            var xRot = mouseInput.y * MouseSensetivity;

            var camTargetRot = cam.transform.localRotation * Quaternion.Euler(-xRot, 0, 0);

            if (Smooth) cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, camTargetRot, SmoothFloat * Time.deltaTime);
            else cam.transform.localRotation = camTargetRot;
        }

        private void Jump(float jumpHeight)
        {
            if (rb.velocity.y < 0)
            {
                capsuleCollider.material = null;
                rb.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetKeyDown(KeyCode.Space))
            {
                capsuleCollider.material = ZeroFrictionMaterial;
                rb.velocity += Vector3.up * Physics.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
            {
                rb.velocity = new Vector3(0, jumpHeight, 0);
            }
        }

        private void CheckWall()
        {
            Vector3 horizontalMove = rb.velocity;
            horizontalMove.y = 0;
            float distance = horizontalMove.magnitude * Time.fixedDeltaTime;
            horizontalMove.Normalize();

            if (rb.SweepTest(horizontalMove, out var hit, distance))
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }

        #region Debug

        private void OnDrawGizmos()
        {
            if (!DrawDebugRays || !capsuleCollider) return;

            var groundCheckRayDir = new Vector3(0, -0.05f, 0);
            colliderBottom = capsuleCollider.bounds.center - new Vector3(0, 0.999f, 0);

            //axis input debug ray at the bottom of collider
            Debug.DrawRay(colliderBottom, moveDirection, Color.blue);

            //ground check ray at the bottom of collider
            if (IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(colliderBottom, 0.25f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(colliderBottom, 0.25f);
            }
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                cursorIsLocked = true;
            }

            if (cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        #endregion
    }
}