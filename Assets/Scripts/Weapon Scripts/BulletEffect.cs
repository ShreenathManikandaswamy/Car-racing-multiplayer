using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

// Base class to create bullet effects
public class BulletEffect : PunBehaviour {

	// this method is called when a bullet hits a car
	public virtual void Hit(GameObject car) {
		
	}

	// destroy this bullet and other copies over network in case of hit the track or other gameobjects with tag "World"
	public void OnTriggerEnter(Collider other) {
		if (other.tag == "World") {
			PreFinish ();
		}
	}

	// this method takes care of call Finish on the current instance and on other instances over network
	public void PreFinish() {
		Finish();
		photonView.RPC ("Finish", PhotonTargets.Others);
	}

	// this method is called when a bullet has finished its effect or has reached the max lifetime
	[PunRPC]
	public virtual void Finish() {
		Destroy (gameObject);
	}

}