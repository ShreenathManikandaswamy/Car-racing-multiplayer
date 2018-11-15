using UnityEngine;
using System.Collections;

/// <summary>
/// Attached to room-buttons to control joining of respective room.
/// Hopes to replace this with better solution soon.
/// </summary>
public class RoomJoiner : MonoBehaviour {

	public string RoomName { get; set; }
	
	// Called when this room-button is clicked
	public void Join () {
		PhotonNetwork.JoinRoom(RoomName);
	}
}
