using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// For the local car only. Updates GUI information based on computed position and lap
/// (from CarCareControl), speed (from rigidbody), etc
/// </summary>
public class CarGUI : Photon.MonoBehaviour {
	
	// GUI references to update on race
	public Text speedGUI;
	public Text positionGUI;
	public Text lapGUI;
	public Text messagesGUI;
	private Sprite carIcon;
	
	private Rigidbody m_rigidbody;
	private CarRaceControl carRaceControl;
	
	// register car with manager
	// get and manage waypoints
	void Start () {
		m_rigidbody = GetComponent<Rigidbody>();
		carRaceControl = GetComponent<CarRaceControl>();
	}

	public Sprite GetIcon() {
		if (carIcon == null) {
			int carIndex = (int)photonView.owner.customProperties ["car"];
			carIcon = Resources.Load<Sprite> ("icon" + carIndex);
		}
		return carIcon;
	}
	
	void Update () {
		speedGUI.text = ((int) (m_rigidbody.velocity.magnitude*2.23693629f)) + "";
		lapGUI.text = (carRaceControl.lapsCompleted + 1) + "/" + carRaceControl.totalLaps;
		positionGUI.text = "" + carRaceControl.currentPosition;
		// wait for first checkpoint, so the GO! message isn't hidden by this
		if (carRaceControl.waypointsPassed > 0 && carRaceControl.state != RaceState.FINISHED) {
			if (carRaceControl.WrongWay) {
				messagesGUI.text = "Wrong Way!";
			} else {
				messagesGUI.text = "";
			}
		}
	}

	// ***** Template code for car custom colors *****
	// see the end of CreateCar() on PunRaceManager as well!

	// sends a custom color for all copies of this car (plus player name for debugging)
	public void SendColor(Color color) {
		photonView.RPC ("ReceiveColor", PhotonTargets.All, color.r, color.g, color.b, PhotonNetwork.player.name);
	}

	// receives a custom color for this car from the original computer that Instantiated it
	[PunRPC]
	public void ReceiveColor(float r, float g, float b, string name) {
		Debug.Log ("Received color for: " + name);
		Debug.Log ("Color: " + new Color(r,g,b));
		// TODO: Apply color to the appropriate materials, etc...
	}
}
