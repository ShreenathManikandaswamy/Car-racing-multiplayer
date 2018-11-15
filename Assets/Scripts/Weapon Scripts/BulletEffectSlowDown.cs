using UnityEngine;
using Photon;
using System.Collections;

// slown down effect - slows down a target applying a backward force
public class BulletEffectSlowDown : BulletEffect {

	// amount of force applied
	public float impact = 5000;

	public override void Hit(GameObject car) {
		car.GetComponent<Rigidbody> ().AddForce (-car.transform.forward * impact, ForceMode.Impulse);
		PreFinish ();
	}

	// loads an explosion and destroys current game object
	[PunRPC]
	public override void Finish() {
		Instantiate (Resources.Load ("Explosion"), transform.position, transform.rotation);
		Destroy (gameObject);
	}

}