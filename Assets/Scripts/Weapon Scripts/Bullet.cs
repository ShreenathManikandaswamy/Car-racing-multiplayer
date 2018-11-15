using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

// represents a bullet, controls its movement and lifetime
public class Bullet : PunBehaviour {

	// type of a bullet, ranged type is used for missiles and fixed is used for bullet put on the ground like a grenade
	public enum BulletType { Ranged, Fixed };

	public BulletType type;

	// speed of the projectile iff it is a ranged bullet
	public float projectileSpeed = 100;

	// how much time to be destroyed if it doesn't hit a car
	public float lifetime = 3;

	[HideInInspector]
	public bool local;

	private bool wasHitCalled = false;

	private BulletEffect bulletEffect;

	public void Start() {
		this.bulletEffect = GetComponent<BulletEffect> ();
	}

	void Update () {
		if (type == BulletType.Ranged) {
			// do this even for not local projectiles (dead simple dead reckoning)
			transform.Translate (Vector3.forward * projectileSpeed * Time.deltaTime, Space.Self);
		} 

		// the rest is done only in the original copy of the weapon
		if (!local)
			return;
		
		if (lifetime <= 0 && !wasHitCalled) {
			EventWasHit();
			wasHitCalled = true;
		}

		lifetime -= Time.deltaTime;
	}

	// calls the effect attached to this bullet
	public void EventHit(GameObject car) {
		bulletEffect.Hit(car);
	}

	// destroys the bullet and its copies over network
	public void EventWasHit() {
		bulletEffect.PreFinish ();
	}

}