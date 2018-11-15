using UnityEngine;
using Photon;
using System.Collections;

// spin effect - spins a target applying a force over a specified time
public class BulletEffectSpin : BulletEffect {

	// amount of force applied
	public float impact = 5000;

	// how long the car must rotate
	public float rotationTime = 0.5f;

	private GameObject car;

	public override void Hit(GameObject car) {
		this.car = car;
	}

	// when rotations is over then execute PreFinish
	public void FixedUpdate() {
		if (car) {
			if (rotationTime > 0) {
				car.GetComponent<Rigidbody> ().AddTorque (car.transform.up * impact, ForceMode.VelocityChange);
				this.rotationTime -= Time.fixedDeltaTime;
			} else {
				PreFinish();
			}
		}
	}

	// loads an explosion and destroys current game object
	[PunRPC]
	public override void Finish() {
		Instantiate (Resources.Load ("Explosion"), transform.position, transform.rotation);
		Destroy (gameObject);
	}

}