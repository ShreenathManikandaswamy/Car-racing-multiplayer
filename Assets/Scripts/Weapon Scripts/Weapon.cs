using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

// represents a weapon and its features
public class Weapon : UnityEngine.MonoBehaviour {

	// it means which button will fire this weapon (Weapon1, Weapon2, Weapon3...)
	public int order = 1;

	public Bullet bullet;

	// cooldown to fire again
	public float cooldown;
	public int maxAmmo;

	// sprite to show on screen
	public Sprite sprite;

	[HideInInspector]
	public int currentAmmo = 0;

	[HideInInspector]
	public bool local = false;

	[HideInInspector]
	public bool isSelected = false;

	private WeaponManager weaponManager;

	private float currentTime = 0;

	public void Start() {
		weaponManager = GetComponentInParent<WeaponManager> ();
	}

	void Update () {
		// do not process if not on local player's computer
		if (weaponManager.GetComponent<PhotonView> ().owner != PhotonNetwork.player)
			return;

		// decreases current time (cooldown) of each weapon
		currentTime -= (Time.deltaTime);

		// if selected weapon has cooldown ok and ammo then fire
		if (isSelected && currentTime <= 0 && currentAmmo > 0 && (CrossPlatformInputManager.GetButtonDown ("Fire1") || CrossPlatformInputManager.GetAxis("Fire1") > 0)) {
			currentTime = cooldown;

			GameObject bulletGO = null;

			// if weapon is fixed then instantiate it in the same position of the weapon except for y axis and rotation
			if (bullet.type == Bullet.BulletType.Fixed) {
				bulletGO = PhotonNetwork.Instantiate ("Bullet/" + bullet.name, new Vector3 (transform.position.x, bullet.transform.position.y, transform.position.z), bullet.transform.rotation, 0);
			} else {
				Vector3 eulerAngles = transform.rotation.eulerAngles;
				eulerAngles.x = 0;

				Quaternion bulletRotation = transform.rotation;
				bulletRotation.eulerAngles = eulerAngles;

				bulletGO = PhotonNetwork.Instantiate ("Bullet/" + bullet.name, transform.position, bulletRotation, 0);
			}

			bulletGO.GetComponent<Bullet>().local = true;
			currentAmmo--;

			// updates weapon's remaining ammo
			weaponManager.UpdateAmmo ();
		}	
	}

}