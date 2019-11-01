using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoomerFPSController
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        private CharacterController characterController;
        private Rigidbody rb;

        public float MoveSpeed = 2;

        [Header("Debug")]
        public bool ControllerDebug;
        public bool InputActive;
        public float inputFloat;

        private void Init()
        {
            GetComponents();
        }

        private void GetComponents()
        {
            characterController = GetComponent<CharacterController>();
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Init();
        }

        private void Update()
        {
            Debugs();


            inputFloat = Mathf.Clamp01(inputFloat);

            if (Input.GetKey(KeyCode.W))
            {
                inputFloat += Time.deltaTime * MoveSpeed;

                if(inputFloat >= 1)
                {
                    inputFloat = 1;
                    InputActive = true;
                }
            }
            else
            {
                inputFloat -= Time.deltaTime * MoveSpeed;

                if (inputFloat <= 0)
                {
                    inputFloat = 0;
                    InputActive = false;
                }
            }
        }


        private void Debugs()
        {
            if (!ControllerDebug) return;

            var origin = transform.position + characterController.bounds.center;
            Debug.DrawRay(characterController.bounds.center - new Vector3(0,1f,0), new Vector3(0,0,inputFloat), Color.red);
        }
    }
}