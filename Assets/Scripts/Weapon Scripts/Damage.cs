using UnityEngine;
using System.Collections;

public class Damage : Photon.MonoBehaviour {

	void Start() {
	}

	void Update() {
	}

	void OnTriggerEnter(Collider other) {
		// only process bullets in the car's original local computer (where physics are computed)
		if (photonView.owner != PhotonNetwork.player)
			return;

		if (other.tag == "Bullet") {
			Bullet bullet = other.GetComponent<Bullet> ();

			// do not hit the car that created the bullet
			if (photonView.owner != bullet.photonView.owner) {
				bullet.EventHit (gameObject);
			}
		} else if (other.tag == "Collectable") {
			CollectableEffect collectableEffect = other.GetComponent<CollectableEffect> ();
			collectableEffect.EventHit (gameObject);
		}
	}

}