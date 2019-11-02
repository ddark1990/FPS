using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoomerFPSController
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class FPSController : MonoBehaviour
    {
        [Header("Config")]
        [Range(1, 10)] public float AccelerationSpeed = 2;
        [Range(1, 10)] public float WalkForwardSpeed = 4;
        [Range(1, 10)] public float StrafeSpeed = 3;
        [Range(1, 10)] public float WalkBackwardSpeed = 2;
        [Range(1,5)] public float SprintMultiplier = 1.5f;
        [Range(1,25)] public float JumpHeight = 2f;
        public LayerMask GroundMask;
        [Space]
        [Header("Private var")]
        [SerializeField] private bool IsGrounded;
        [SerializeField] private bool IsWalking;
        [SerializeField] private bool IsSprinting;
        [SerializeField] private float accelerationOutput;
        [SerializeField] private float walkSpeedOutput;
        [SerializeField] private Vector2 axisInput;
        [Space]
        public bool DrawDebugRays;


        private CapsuleCollider capsuleCollider;
        private Rigidbody rb;
        private Vector3 colliderBottom;

        private void Init()
        {
            GetComponents();

        }
        private void GetComponents()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
        }
        private void OnEnable()
        {
            Init();
        }

        private void FixedUpdate()
        {
            IsGrounded = ControllerInput.IsGrounded(capsuleCollider, GroundMask);
            MovePlayer(axisInput, walkSpeedOutput);
        }

        private void Update()
        {
            Debug();

            axisInput = ControllerInput.GetAxisOuput(axisInput.x, axisInput.y); //grabs the axis from unity's Input in project settings
            accelerationOutput = ControllerInput.AccelerationOutput(AccelerationSpeed);
            walkSpeedOutput = ControllerInput.MovementSpeedOutput(axisInput, WalkForwardSpeed, WalkBackwardSpeed, StrafeSpeed, accelerationOutput, SprintMultiplier); //AWSD keys
        }

        private void MovePlayer(Vector2 inputAxis, float _walkSpeedOutput)
        {
            IsWalking = ControllerInput.WalkOutput();
            if (IsSprinting) IsWalking = false;
            IsSprinting = ControllerInput.SprintOutput();

            rb.MovePosition(transform.position + new Vector3(inputAxis.x, 0, inputAxis.y) * Time.deltaTime * _walkSpeedOutput);

            if(IsGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector3(0, JumpHeight, 0), ForceMode.VelocityChange);
            }
        }

        private void Debug()
        {
            if (!DrawDebugRays) return;

            var groundCheckRayDir = new Vector3(0, -0.05f, 0);
            colliderBottom = capsuleCollider.bounds.center - new Vector3(0, 0.999f, 0);

            //axis input debug ray at the bottom of collider
            UnityEngine.Debug.DrawRay(colliderBottom, new Vector3(axisInput.x , 0, axisInput.y), Color.blue);

            //ground check ray at the bottom of collider
            if (IsGrounded)
            {
                UnityEngine.Debug.DrawRay(colliderBottom, groundCheckRayDir, Color.green);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0.5f, 0, 0), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(-0.5f, 0, 0), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0, 0, 0.5f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0, 0, -0.5f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0.35f, 0, 0.35f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(-0.35f, 0, 0.35f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(-0.35f, 0, -0.35f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0.35f, 0, -0.35f), groundCheckRayDir, Color.grey);

            }
            else
            {
                UnityEngine.Debug.DrawRay(colliderBottom, groundCheckRayDir, Color.red);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0.5f, 0, 0), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(-0.5f, 0, 0), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0, 0, 0.5f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0, 0, -0.5f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0.35f, 0, 0.35f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(-0.35f, 0, 0.35f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(-0.35f, 0, -0.35f), groundCheckRayDir, Color.grey);
                UnityEngine.Debug.DrawRay(colliderBottom + new Vector3(0.35f, 0, -0.35f), groundCheckRayDir, Color.grey);

            }
        }
    }
}