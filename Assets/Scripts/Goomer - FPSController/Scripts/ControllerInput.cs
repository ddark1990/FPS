using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoomerFPSController
{
    public static class ControllerInput
    {
        public static float accelerationFloat;
        public static bool walkOutput, sprintOutput;

        public static Vector2 GetAxisInputs()
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        public static Vector2 GetMouseInputs()
        {
            return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        public static bool WalkOutput()
        {
            return walkOutput = Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 ||
                                Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0;
        }
        public static bool SprintOutput()
        {
            return sprintOutput = Input.GetKey(KeyCode.LeftShift);
        }
        public static bool IsGrounded(CapsuleCollider capsuleCollider, LayerMask groundMask)
        {
            var colliderBottom = capsuleCollider.bounds.center - new Vector3(0, 0.999f, 0);

            var rayLength = 0.05f;
            var rayDir = new Vector3(0, -rayLength, 0);

            return Physics.CheckSphere(colliderBottom, 0.25f, groundMask);
        }
        public static float AccelerationOutput(float accelerationSpeed)
        {
            accelerationFloat = Mathf.Clamp01(accelerationFloat);

            if (WalkOutput())
            {
                accelerationFloat += Time.deltaTime * accelerationSpeed;

                if (accelerationFloat >= 1)
                {
                    accelerationFloat = 1;
                    return accelerationFloat;
                }
            }
            else
            {
                accelerationFloat -= Time.deltaTime * accelerationSpeed;

                if (accelerationFloat <= 0)
                {
                    return accelerationFloat = 0;
                }
            }

            return accelerationFloat;
        }
        public static float MovementSpeedOutput(Vector2 axisInput, float walkForwardSpeed, float walkBackwardSpeed, float strafeSpeed, float accelerationSpeed, float sprintMultiplier)
        {
            if(SprintOutput())
            {
                if (axisInput.y > 0 && axisInput.x == 0) //forward
                {
                    var walkForwardFloat = (walkForwardSpeed * accelerationSpeed) * sprintMultiplier;
                    return walkForwardFloat;
                }
                else if (axisInput.y < 0) //backward
                {
                    var walkBackwardFloat = (walkBackwardSpeed * accelerationSpeed) * sprintMultiplier;
                    return walkBackwardFloat;
                }
                else if (axisInput.x < 0) //strafe left
                {
                    var strafeFloat = (strafeSpeed * accelerationSpeed) * sprintMultiplier;
                    return strafeFloat;
                }
                else if (axisInput.x > 0) //strafe right
                {
                    var strafeFloat = (strafeSpeed * accelerationSpeed) * sprintMultiplier;
                    return strafeFloat;
                }
            }
            else
            {
                if (axisInput.y > 0 && axisInput.x == 0) //forward
                {
                    var walkForwardFloat = walkForwardSpeed * accelerationSpeed;
                    return walkForwardFloat;
                }
                else if (axisInput.y < 0) //backward
                {
                    var walkBackwardFloat = (walkBackwardSpeed * accelerationSpeed);
                    return walkBackwardFloat;
                }
                else if (axisInput.x < 0) //strafe left
                {
                    var strafeFloat = (strafeSpeed * accelerationSpeed);
                    return strafeFloat;
                }
                else if (axisInput.x > 0) //strafe right
                {
                    var strafeFloat = (strafeSpeed * accelerationSpeed);
                    return strafeFloat;
                }
            }

            return 0;
        }
    }
}