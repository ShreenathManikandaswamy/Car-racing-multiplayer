using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

// weapon manager attached to a car to controls which weapon is selected and its informations on screen
public class WeaponManager : PunBehaviour {

	// list of weapons that can be used
	[HideInInspector]
	public Weapon[] weapons;

	// image on screen showing which weapon is selected
	[HideInInspector]
	public Image weaponImageGUI;

	// text on screen showing how much ammo the selected weapon has
	[HideInInspector]
	public Text weaponTextGUI;

	private int weaponSelectedIndex = 0;
	private Weapon weaponSelected;

	// Init weapons ammo and pre-select the first
	public void Start() {
		weapons = GetComponentsInChildren<Weapon> ();

		for (int index = 0; index < weapons.Length; index++) {
			weapons [index].currentAmmo = 10;

			// holds index of the weapon with order = 1
			if (weapons [index].order == 1) {
				weaponSelectedIndex = index;
			}
		}

		// selects weapon with order=1
		SelectWeapon ();
		weaponImageGUI.enabled = true;
		weaponTextGUI.enabled = true;
	}

	void Update() {
		// do not process if not on local player's computer
		if (photonView.owner != PhotonNetwork.player)
			return;

		// checks if user has pressed a weapon button
		for (int index = 0; index < weapons.Length; index++) {
			if (CrossPlatformInputManager.GetButtonDown ("Weapon"+weapons[index].order)) {
				weaponSelectedIndex = index;
				SelectWeapon ();
			}
		}

		if (CrossPlatformInputManager.GetButtonDown("WeaponChange")) {
			weaponSelectedIndex++;
			if (weaponSelectedIndex > weapons.Length - 1) {
				weaponSelectedIndex = 0;
			}

			SelectWeapon ();

			CrossPlatformInputManager.SetButtonUp ("WeaponChange");
		}
	}

	// refill the ammo of all weapons
	public void Refill(string weaponName, int ammount){
		for (int index = 0; index < weapons.Length; index++) {
			if (weaponName != null) {
				if (weapons [index].name == weaponName) {
					RefillCheck (weapons [index], ammount);
				}
			} else {
				RefillCheck (weapons [index], ammount);
			}
		}

		// updates weapon's remaining ammo
		weaponTextGUI.text = weaponSelected.currentAmmo + "";
	}

	// if ammount is negative then currentAmmo will be equals maxAmmo, if not checks if the new ammo is greater than maxAmmo
	private void RefillCheck(Weapon weapon, int ammount) {
		if (ammount < 0) {
			weapon.currentAmmo = weapon.maxAmmo;
		} else {
			weapon.currentAmmo += ammount;

			if (weapon.currentAmmo > weapon.maxAmmo) {
				weapon.currentAmmo = weapon.maxAmmo;
			}
		}
	}

	private void SelectWeapon() {
		// Unselect any previous weapon
		if (weaponSelected) {
			weaponSelected.isSelected = false;
		}

		weaponSelected = weapons[weaponSelectedIndex];
		weaponSelected.isSelected = true;

		weaponImageGUI.sprite = weaponSelected.sprite;

		UpdateAmmo ();
	}

	public void UpdateAmmo() {
		// updates weapon's remaining ammo
		weaponTextGUI.text = weaponSelected.currentAmmo + "";
	}

}