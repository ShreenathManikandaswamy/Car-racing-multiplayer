using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

// responsible to randomly spawn the collectables passed as parameter
public class CollectableSpawn : PunBehaviour {

	// array of possible collectables to spawn (they don't need to be the same type)
	public CollectableEffect[] collectables;

	// cooldown to respawn if last collectable was a powerup
	public float cooldownPowerup = 5;

	// cooldown to respawn if last collectable was an ammo pickup
	public float cooldownAmmo = 3;

	[HideInInspector]
	public bool spawnEnabled = true;

	private float currentTime = 0;

	private CollectableEffect nextCollectable;

	public void Start() {
		// disables renderer because it serves only as placeholder to correctly position collectables on map
		this.GetComponent<Renderer> ().enabled = false;
	}

	public void Update() {
		// only runs on masterclient and if it has collectables
		if (!PhotonNetwork.isMasterClient || collectables.Length == 0) {
			return;
		}

		// if spawning is enabled and cooldown is ok then a next collectable is randomly picked and instantiated over network
		if (spawnEnabled) {
			currentTime -= Time.deltaTime;

			if (currentTime <= 0) {
				PickNextCollectable ();

				CollectableEffect collectableEffet = PhotonNetwork.Instantiate ("Collectable/" + nextCollectable.name, transform.position, transform.rotation, 0).GetComponent<CollectableEffect>();
				collectableEffet.spawnPoint = this;

				spawnEnabled = false;
			}
		}
	}

	// resets the spawning, so a new collectable will be instantiated after cooldown
	[PunRPC]
	public void Reset() {
		spawnEnabled = true;
	}

	// randomly picks a new a collectable from collectables and sets the correct cooldown
	public void PickNextCollectable() {
		nextCollectable = collectables[Random.Range (0, collectables.Length)];
		if (nextCollectable.GetCollectableType() == CollectableEffect.CollectableType.PowerUp) {
			currentTime = cooldownPowerup;
		} else if (nextCollectable.GetCollectableType() == CollectableEffect.CollectableType.Ammo) {
			currentTime = cooldownAmmo;
		}
	}

}