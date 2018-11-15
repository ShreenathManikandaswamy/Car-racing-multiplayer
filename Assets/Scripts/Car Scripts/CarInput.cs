using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Replaces standard Unity's CarInput. Caches player input values,
/// so they can be synchronized over the network for dead reckoning 
/// physics of remote cars.
/// 
/// Relies on Unity's standard CarController for basic car physics.
/// 
/// Used by NetworkClass when sending/receiving input to/from remote cars.
/// </summary>
public class CarInput : MonoBehaviour
{
	// The car controller we want to use
	private CarController car;

	// Player input properties
	public float Steer { get; set; }
	public float Accell { get; set; }
	public float Handbrake { get; set; }

	// When true, car will be controlled locally (remotely otherwise)
	public bool controlable = false;

	void Start()
	{
		car = GetComponent<CarController>();
	}
	
	void FixedUpdate()
	{
		// If car is locally controllable, read and cache player input
		if (controlable) {
			GetInput();
		}
		// Allways move the car, independently of wether the input values
		// came from (local player or remote).
		ApplyInput ();
	}

	// good to override on custom version for different car physics packs
	protected virtual void GetInput() {
		Steer = CrossPlatformInputManager.GetAxis("Horizontal");
		Accell = CrossPlatformInputManager.GetAxis("Vertical");
		
		if (Flipped () || CrossPlatformInputManager.GetButton("Fire3")) {
			Unflip();
		}
		#if !MOBILE_INPUT
		Handbrake = CrossPlatformInputManager.GetAxis("Jump");
		#endif
	}

	// good to override on custom version for different car physics packs
	protected virtual void ApplyInput() {
		car.Move(Steer, Accell, Accell, Handbrake);
	}

	private bool Flipped() {
		// Car is upside-down if transform.up vector is poiting downward (even slightly).
		// If standing, and upside-down, then it is flipped and should be unflipped
		if (GetComponent<Rigidbody> ().velocity.sqrMagnitude < 0.01 && Vector3.Dot(transform.up, Vector3.down) > 0) {
			return true;
		}
		return false;
	}

	private void Unflip() {
		Vector3 angles = transform.eulerAngles;
		angles.z = 0;
		transform.eulerAngles = angles;
	}
	
}

