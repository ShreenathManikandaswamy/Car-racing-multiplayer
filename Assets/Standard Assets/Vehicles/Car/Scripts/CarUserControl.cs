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
	        m_Car = GetComponent<CarController>();
	    }

	    private void FixedUpdate()
	    {
	        float m_h = CrossPlatformInputManager.GetAxis("Horizontal");
	        float m_v = CrossPlatformInputManager.GetAxis("Vertical");
			#if !MOBILE_INPUT
	        float m_handbrake = CrossPlatformInputManager.GetAxis("Jump");
			m_Car.Move(m_h, m_v, m_v, m_handbrake);
			#else
			m_Car.Move(m_h, m_v, m_v, 0f);
			#endif

		}

	}

}

