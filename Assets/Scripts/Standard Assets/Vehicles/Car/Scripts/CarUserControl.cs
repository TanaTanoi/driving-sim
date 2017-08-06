using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public bool keyboardOverride = false;

        public bool ready = false;
        public bool canMove = false;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            ready = false;
        }


        private void FixedUpdate() {
            if (canMove) {
                if (keyboardOverride) {
                    KeyboardMove();
                } else {
                    ThrustmasterMove();
                }
            }
            if (Input.GetButtonDown("ThrustmasterLeftTrigger")) {
                gameObject.GetComponentInChildren<VRCameraPod>().CalibrateHeadset();
            }
            if (!ready) {
                if (Input.GetButtonDown("ThrustmasterRightTrigger")) {
                    ready = true;
                }
            }
        }

        private void ThrustmasterMove() {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("ThrustmasterWheel");
            float v = CrossPlatformInputManager.GetAxis("ThrustmasterAccelerate") + 1; // 0 to 2
            float footbreak = (CrossPlatformInputManager.GetAxis("ThrustmasterClutch") + 1); // -2 to 0
            float handbrake = CrossPlatformInputManager.GetAxis("ThrustmasterBreak") + 1; // 0 to 2

            footbreak = (v - footbreak) / -2f; // ensures that footbreak can't be 1 if gas is applied
            v /= 2f;
            handbrake /= 2f;

            //Debug.Log("Wheel " + h + " Accelerate " + v + " Footbreak  " + footbreak + " handbreak " + handbrake);
            m_Car.Move(h, v, footbreak, handbrake);
        }

        private void KeyboardMove() {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("KB_Horizontal");
            float v = CrossPlatformInputManager.GetAxis("KB_Vertical");
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            //Debug.Log("Wheel " + h + " Accelerate " + v + " Footbreak  " + footbreak + " handbreak " + handbrake);
            m_Car.Move(h, v, v, handbrake);
        }
    }
}
