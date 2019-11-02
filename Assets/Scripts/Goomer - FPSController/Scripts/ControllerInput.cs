using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ControllerInput
{
    public static float accelerationFloat;
    public static bool walkInput;

    public static bool IsWalking()
    {
        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 ||
            Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
        {
            walkInput = true;
        }
        else walkInput = false;

        return walkInput;
    }
    public static float GetAcceleration(float accelerationSpeed)
    {
        accelerationFloat = Mathf.Clamp01(accelerationFloat);

        if (IsWalking())
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
    public static float GetMovementSpeed(Vector2 axisInput, float walkForwardSpeed, float walkBackwardSpeed, float strafeSpeed, float accelerationSpeed)
    {
        if (axisInput.y > 0 && axisInput.x == 0) //forward
        {
            var walkForwardFloat = walkForwardSpeed * accelerationSpeed;
            return walkForwardFloat;
        }
        else if(axisInput.y < 0) //backward
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

        return 0;
    }

    public static Vector2 GetAxisInput(float x, float y)
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        return new Vector2(x,y);
    }
}
