using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
using Photon;

// ammo effect - when a car hits this collectable its ammo is refilled
public class CollectableEffectAmmo : CollectableEffect {

	// if weapon is null then all weapon will be affected
	public Weapon weapon;

	// if ammount is negative then ammo will be equals to max ammo
	public int ammount = 1;

	public override CollectableType GetCollectableType () {
		return CollectableType.Ammo;
	}

	// call refill from car's weapon manager
	public override void Hit(GameObject car) {
		car.GetComponent<WeaponManager> ().Refill (weapon != null ? weapon.name : null, ammount);
		PreFinish ();
	}

}