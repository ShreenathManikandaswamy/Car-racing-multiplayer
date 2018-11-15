using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
using Photon;

// max speed effect - when a car hits this collectable its max speed is modified over a specified amount of time
public class CollectableEffectMaxSpeed : CollectableEffect {

	// multiplies car's current max speed
	public float maxSpeedMultiplier = 1;

	// how long the car sould have its max speed modified
	public float lifetime = 5;

	private GameObject car;

	// temporally stores the max speed before apply this effect
	private float oldMaxSpeed;

	public override CollectableType GetCollectableType () {
		return CollectableType.PowerUp;
	}

	public override void Hit(GameObject car) {
		this.car = car;
		this.oldMaxSpeed = this.car.GetComponent<CarController> ().m_Topspeed;
		this.car.GetComponent<CarController> ().m_Topspeed = this.oldMaxSpeed * maxSpeedMultiplier;
	}

	// decreases lifetime until it reaches 0, so sets back the old max speed
	public void FixedUpdate() {
		if (car) {
			lifetime -= Time.fixedDeltaTime;
			if (lifetime <= 0) {
				this.car.GetComponent<CarController> ().m_Topspeed = oldMaxSpeed;
				PreFinish ();
			}
		}
	}

}