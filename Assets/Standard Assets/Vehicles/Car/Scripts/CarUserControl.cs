using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("ThrustmasterWheel");
            float v = CrossPlatformInputManager.GetAxis("ThrustmasterAccelerate") + 1; // 0 to 2
            float footbreak = (CrossPlatformInputManager.GetAxis("ThrustmasterClutch") + 1) * -1; // -2 to 0
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("ThrustmasterBreak") + 1; // 0 to 2

            Debug.Log("Wheel " + h + " Accelerate " + v + " Footbreak  " + footbreak + " handbreak " + handbrake);
            m_Car.Move(h, v, footbreak, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
