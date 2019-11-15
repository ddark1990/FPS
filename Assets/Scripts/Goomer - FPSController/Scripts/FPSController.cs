using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoomerFPSController
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class FPSController : MonoBehaviour
    {
        [Header("SpeedConfig")]
        [Range(1f, 25)] public float WalkForwardSpeed = 4;
        [Range(1f, 25)] public float StrafeSpeed = 3;
        [Range(1f, 25)] public float WalkBackwardSpeed = 3;
        [Range(1f,5)] public float SprintMultiplier = 1.4f;
        [Header("JumpConfig")]
        [Range(1,25)] public float maxJumpHeight = 7f;
        [Range(1,10)] public float FallMultiplier = 2.5f;
        [Range(1,10)] public float LowJumpMultiplier = 2f;
        [Header("MouseConfig")]
        [Range(1, 200)] public float MouseSensetivity = 40f;
        [Header("MaskConfig")]
        public LayerMask GroundMask;
        [Space]
        [Header("Private var")]
        [SerializeField] private bool IsGrounded;
        [SerializeField] private bool IsWalking;
        [SerializeField] private bool IsSprinting;
        [SerializeField] private bool MidAir;
        public bool cursorIsLocked;
        public bool cameraLocked;
        [SerializeField] private float moveSpeedOutput;
        [SerializeField] public Vector2 axisInput;
        [SerializeField] private Vector2 mouseInput;
        [SerializeField] private float sphereSize;
        [Space]
        [Header("Debug/Cache")]
        public bool DrawDebugRays;
        public PhysicMaterial ZeroFrictionMaterial;
        public Transform LookAtTransform;
        public Transform CameraPivot;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));

        private Animator animator;
        private CapsuleCollider capsuleCollider;
        private Rigidbody rb;
        private Vector3 colliderBottom;
        private Vector3 targetVelocity;
        private Vector3 groundNormal;
        private Camera cam;
        private float desiredPitch;
        private float desiredYaw;
        private float altYaw;

        private void Init()
        {
            GetComponents();

        }
        private void GetComponents()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            cam = GetComponentInChildren<Camera>();
            animator = GetComponent<Animator>();
        }
        private void OnEnable()
        {
            Init();
        }

        private void Update()
        {
            axisInput = ControllerInput.GetAxisInputs();
            mouseInput = ControllerInput.GetMouseInputs();
            IsGrounded = ControllerInput.IsGrounded(capsuleCollider, GroundMask, animator, out MidAir);

            InternalLockUpdate();
            UpdateDesiredTargetSpeed(axisInput);
            UpdateControllerInputs();
            GetGroundNormal();

        }

        private void FixedUpdate()
        {
            RotatePlayer();
            AltLook();
        }

        private void LateUpdate()
        {
            RotateCamera();
        }

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero) return;

            moveSpeedOutput = Mathf.Clamp(moveSpeedOutput, 0, 10);

            if (input.x > 0 || input.x < 0)
            {
                //strafe
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, StrafeSpeed, 1);
            }

            if (input.y < 0)
            {
                //backwards
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, WalkBackwardSpeed, 1);
            }

            if (input.y > 0 && input.x == 0)
            {
                //forwards
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, WalkForwardSpeed, 1);
            }
            else
            {
                moveSpeedOutput = Mathf.Lerp(moveSpeedOutput, 0, Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeedOutput *= SprintMultiplier;
                IsSprinting = true;
            }
            else
            {
                IsSprinting = false;
            }
        }

        private void UpdateControllerInputs() 
        {
            IsWalking = ControllerInput.WalkOutput();
            if (IsSprinting) IsWalking = false;

            targetVelocity = new Vector3(axisInput.x * moveSpeedOutput, rb.velocity.y, axisInput.y * moveSpeedOutput);
            targetVelocity = transform.TransformDirection(targetVelocity);

            rb.velocity = targetVelocity;

            if(animator) //move
            {
                animator.SetFloat("Horizontal", axisInput.x * moveSpeedOutput);
                animator.SetFloat("Vertical", axisInput.y * moveSpeedOutput);

                animator.GetBoneTransform(HumanBodyBones.Head).LookAt(LookAtTransform); //controlls the headlook

                //animator.speed = moveSpeedOutput;
            }

            Jump(maxJumpHeight);
        }

        private void AltLook()
        {
            var origYRot = 0f;

            if (Input.GetKey(KeyCode.LeftAlt) && !cameraLocked)
            {
                altYaw += mouseInput.x * MouseSensetivity * Time.deltaTime;
                altYaw = Mathf.Clamp(altYaw, -100, 100);
                CameraPivot.localRotation = Quaternion.Euler(new Vector3(0, altYaw, 0));
            }
            else
            {
                CameraPivot.localRotation = Quaternion.Euler(new Vector3(0, origYRot, 0));
                //StartCoroutine(LerpFloat(altYaw, origYRot, 1));
                //altYaw = Mathf.Lerp(altYaw, origYRot, 1 * Time.deltaTime);
                altYaw = origYRot; //find a way to lerp 
            }
        }

        private void RotatePlayer()
        {
            if (Input.GetKey(KeyCode.LeftAlt) || cameraLocked) return;

            //var yRot = mouseInput.x * MouseSensetivity;

            desiredYaw += mouseInput.x * MouseSensetivity * Time.deltaTime;

            transform.rotation = Quaternion.Euler(new Vector3(0, desiredYaw, 0));
        }

        private void RotateCamera()
        {
            if (cameraLocked) return;

            //var xRot = mouseInput.y * MouseSensetivity;

            desiredPitch -= mouseInput.y * MouseSensetivity * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, -90, 90);

            cam.transform.localRotation = Quaternion.Euler(new Vector3(desiredPitch, 0, 0));
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
                if(animator )//move
                    animator.SetTrigger("Jump");

                var jumpVelocity = new Vector3(0, jumpHeight, 0);
                jumpVelocity.y = Mathf.Clamp(jumpVelocity.y, -jumpHeight, jumpHeight);
                //jumpVelocity = transform.TransformDirection(jumpVelocity);

                rb.velocity = jumpVelocity;
            }

            CheckWall();
        }

        private void CheckWall()
        {
            Vector3 horizontalMove = rb.velocity;
            horizontalMove.y = 0;
            float distance = horizontalMove.magnitude;
            horizontalMove.Normalize();

            if (rb.SweepTest(horizontalMove, out var hit, distance))
            {
                //rb.velocity = new Vector3(0, rb.velocity.y, 0);
                Debug.DrawLine(transform.position, hit.point, Color.yellow);
            }
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(groundNormal, Vector3.up);
            return SlopeCurveModifier.Evaluate(angle);
        }

        void GetGroundNormal()
        {
            Debug.DrawRay(colliderBottom + new Vector3(0, 0.1f, 0), Vector3.down, Color.red);

            if (Physics.Raycast(colliderBottom + new Vector3(0,0.1f,0), Vector3.down, out var hitInfo, 0.2f, GroundMask))
            {
                groundNormal = hitInfo.normal;
            }
            else
            {
                groundNormal = Vector3.up;
            }
        }

        #region Debug

        private void OnDrawGizmos()
        {
            if (!DrawDebugRays || !capsuleCollider) return;

            var groundCheckRayDir = new Vector3(0, -0.05f, 0);
            colliderBottom = capsuleCollider.bounds.center - new Vector3(0, 0.999f, 0);

            //axis input debug ray at the bottom of collider
            Debug.DrawRay(colliderBottom, targetVelocity, Color.blue);

            //ground check ray at the bottom of collider
            if (IsGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(colliderBottom, 0.2f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(colliderBottom, 0.2f);
            }
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                cursorIsLocked = false;
                cameraLocked = true;
            }
            else if (Input.GetMouseButtonUp(0) && !PlayerSelection.IsPointerOverUiObject())
            {
                cursorIsLocked = true;
                cameraLocked = false;
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

        //helper functions

        private IEnumerator LerpFloat(float fromValue, float toValue, float lerpSpeed)
        {
            Mathf.Lerp(fromValue, toValue, lerpSpeed * Time.deltaTime);

            yield return new WaitForSeconds(lerpSpeed);
        }
    }
}