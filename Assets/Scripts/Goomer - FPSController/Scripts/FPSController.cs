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
        public float AccelerationSpeed = 2;
        public float WalkForwardSpeed = 4;
        public float StrafeSpeed = 3;
        public float WalkBackwardSpeed = 2;
        public LayerMask LayerMask;
        [Space]
        public bool ControllerDebug;
        [Header("Debug")]
        public bool IsGrounded;
        public bool IsWalking;
        public float accelerationOutput;
        public float walkSpeedOutput;
        public Vector2 axisInput;

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

        private void Update()
        {
            Debug();

            axisInput = ControllerInput.GetAxisInput(axisInput.x, axisInput.y);
            accelerationOutput = ControllerInput.GetAcceleration(AccelerationSpeed);
            walkSpeedOutput = ControllerInput.GetMovementSpeed(axisInput, WalkForwardSpeed, WalkBackwardSpeed, StrafeSpeed, accelerationOutput); //AWSD keys
            IsWalking = ControllerInput.IsWalking();

            rb.MovePosition(transform.position + new Vector3(axisInput.x, 0, axisInput.y) * Time.deltaTime * walkSpeedOutput);
        }

        public bool isGrounded()
        {
            colliderBottom = capsuleCollider.bounds.center - new Vector3(0, 0.999f, 0);

            var rayLength = 0.2f;
            var rayDir = new Vector3(0, -rayLength, 0);

            return Physics.Raycast(colliderBottom, rayDir, rayLength, LayerMask);
        }

        private void Debug()
        {
            if (!ControllerDebug) return;

            var groundCheckRayDir = new Vector3(0, -0.2f, 0);

            //axis input debug ray at the bottom of collider
            UnityEngine.Debug.DrawRay(colliderBottom, new Vector3(axisInput.x , 0, axisInput.y), Color.blue);

            //ground check ray at the bottom of collider
            IsGrounded = isGrounded();
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