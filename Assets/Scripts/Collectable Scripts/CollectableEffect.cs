using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

// base class to create collectable effects
public abstract class CollectableEffect : PunBehaviour {

	// this could be used to set different actions per type, like a different cooldown if it is a powerup or an ammo
	public enum CollectableType { PowerUp, Ammo };

	// if target is 'player' the method Hit is executed only in the car that get a collectable (e.g max speed boost), if target is 'enemy' than Hit is executed in all other cars (e.g. max speed reduction)
	public enum ExecutionTarget	{ Player, Enemy };

	public ExecutionTarget executionTarget;

	// a bool to avoid multiple execution
	private bool executed = false;

	// collectable spawn point that generated this collectable, it used to retrigger collectable spawning when a car hits it
	[HideInInspector]
	public CollectableSpawn spawnPoint;

	// this method is called when a car hit a collectable
	public void EventHit(GameObject car) {
		if (!executed) {
			executed = true;
			// some effects don't destroy the collectable immediately, so the renderer is disabled
			gameObject.GetComponent<Renderer> ().enabled = false;

			// the spawning of collectables is resetted on master client
			photonView.RPC ("ResetSpawn", PhotonTargets.MasterClient);

			// if target is 'player' than call 'Finish' on others instances (destroy them) and execute local Hit method
			if (executionTarget == ExecutionTarget.Player) {
				photonView.RPC ("Finish", PhotonTargets.Others);
				Hit (car);
			
			// if target is not 'player' execut Hit on the enemies' instances and destroy this instance
			} else {
				photonView.RPC ("PreHitOnEnemies", PhotonTargets.Others);
				Finish ();
			}
		}
	}

	// reset collectables spawning
	[PunRPC]
	public void ResetSpawn() {
		if (spawnPoint != null) {
			spawnPoint.Reset ();
		}
	}

	// type of the collectable to give, for example, different cooldowns for powerups and ammo
	public abstract CollectableType GetCollectableType ();

	// the real effect of a collectable must be implemented here
	public abstract void Hit (GameObject car);

	// this method is executed when target is 'enemy', so on each enemy the local car instance is passed to Hit method
	[PunRPC]
	public void PreHitOnEnemies() {
		gameObject.GetComponent<Renderer> ().enabled = false;
		executed = true;

		Hit(PUNRaceManager.instance.localCar.gameObject);
	}

	// this method takes care of call Finish on the current instance and on other instances over network
	public void PreFinish() {
		Finish();
		photonView.RPC ("Finish", PhotonTargets.Others);
	}

	// this method is called when a collectable has finished its effect or has reached the max lifetime
	[PunRPC]
	public virtual void Finish() {
		Destroy (gameObject);
	}

}