using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine.UI;

/// <summary>
/// Connects to Photon Cloud, then enables GUI to create/join game rooms,
/// which loads the actual Race scene.
/// Inherits from PUNBehavior, overriding standard Photon's Event methods.
/// </summary>
public class PUNMenu : PunBehaviour {

	// References to GUI game objects, so they can be enabled/disabled and
	// used to show useful messages to player.
	public Transform btStart;
	public Transform carPanel;
	public Transform nickPanel;
	public Transform racesPanel;

	public Sprite[] carTextures;
	public Sprite noCar;
	private List<RoomJoiner> rooms = new List<RoomJoiner>();
	public Transform[] playerMenus;
	public InputField edtNickname;
	public Text messages;
	public Sprite[] trackTextures;
	public Transform[] trackButtons;
	public Image trackSprite;

	private int carIndex = 0;
	private int trackIndex = 0;
	// Connects to photon
	void Start () {
		// in case weŕe getting back from a race, already connected to lobby
		if (PhotonNetwork.connected) {
			racesPanel.gameObject.SetActive(true);
			return;
		}
		messages.text = "";
		nickPanel.gameObject.SetActive(true);
	}

	// For each valid game room, creates a join button.
	public override void OnReceivedRoomListUpdate () {
		foreach (RoomJoiner roomButton in rooms) {
			Destroy(roomButton.gameObject);
		}
		rooms.Clear ();
		int i = 0;
		foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
			if (!room.open)
				continue;
			GameObject buttonPrefab = Resources.Load<GameObject>("GUI/RoomGUI");
			GameObject roomButton = Instantiate<GameObject>(buttonPrefab);
			roomButton.GetComponent<RoomJoiner>().RoomName = room.name;
			string info = room.name.Trim() + " (" + room.playerCount + "/" + room.maxPlayers + ")";
			roomButton.GetComponentInChildren<Text>().text = info;
			rooms.Add(roomButton.GetComponent<RoomJoiner>());
			roomButton.transform.SetParent(racesPanel, false);
			roomButton.transform.position.Set(0, i * 120, 0);
		}
	}

	// Called when finished editing nickname (which will also serve as 
	// room name - if player creates one)
	public void EnteredNickname() {
		PhotonNetwork.player.name = edtNickname.text;
		PhotonNetwork.ConnectUsingSettings("v1.0");
		messages.text = "Connecting...";
	}

    // When connected to Photon, enable nickname editing (too)
    public override void OnConnectedToMaster()
    {
		PhotonNetwork.JoinLobby ();
		messages.text = "Entering lobby...";
        
    }

	// When connected to Photon Lobby, disable nickname editing and messages text, enables room list
	public override void OnJoinedLobby () {
		nickPanel.gameObject.SetActive(false);
		messages.gameObject.SetActive(false);
		racesPanel.gameObject.SetActive(true);
	}

	// Called from UI
	public void CreateGame () {
		RoomOptions options = new RoomOptions();
		options.maxPlayers = 4;
		PhotonNetwork.CreateRoom(PhotonNetwork.player.name, options, TypedLobby.Default);
		foreach (Transform tb in trackButtons) {
			tb.gameObject.SetActive(true);
		}
	}

	public override void OnPhotonCreateRoomFailed(object[] codeMessage) {
		if ((short) codeMessage [0] == ErrorCode.GameIdAlreadyExists) {
			PhotonNetwork.playerName += "-2";
			CreateGame ();
		}
	}

	private bool checkSameNameOnPlayersList(string name) {
		foreach (PhotonPlayer pp in PhotonNetwork.otherPlayers) {
			if (pp.name.Equals(name)) {
				return true;
			}
		}

		return false;
	}

	// if we join (or create) a room, no need for the create button anymore;
	public override void OnJoinedRoom () {

		if (checkSameNameOnPlayersList (PhotonNetwork.playerName)) {
			string newName = PhotonNetwork.playerName;
			do {
				newName += "-2";
			} while (checkSameNameOnPlayersList (newName));

			PhotonNetwork.playerName = newName;
		}

		racesPanel.gameObject.SetActive(false);
		carPanel.gameObject.SetActive(true);
		SetCustomProperties(PhotonNetwork.player, 0, PhotonNetwork.playerList.Length - 1);
	}

	// (masterClient only) enables start race button
	public override void OnCreatedRoom () {
		btStart.gameObject.SetActive(true);
		SetCustomProperties(PhotonNetwork.player, 0, PhotonNetwork.playerList.Length - 1);
	}

	// If master client, for every newly connected player, sets the custom properties for him
	// car = 0, position = last (size of player list)
	public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer) {
		if (PhotonNetwork.isMasterClient) {
			SetCustomProperties (newPlayer, 0, PhotonNetwork.playerList.Length - 1);
			photonView.RPC("UpdateTrack", PhotonTargets.All, trackIndex);
		}
	}

	// when a player disconnects from the room, update the spawn/position order for all
	public override void OnPhotonPlayerDisconnected(PhotonPlayer disconnetedPlayer) {
		if (PhotonNetwork.isMasterClient) {
			int playerIndex = 0;
			foreach (PhotonPlayer p in PhotonNetwork.playerList) {
				SetCustomProperties(p, (int) p.customProperties["car"], playerIndex++);
			}
		}
	}

	public override void OnPhotonPlayerPropertiesChanged (object[] playerAndUpdatedProps) {
		UpdatePlayerList ();
	}

	// self-explainable
	public void UpdatePlayerList() {
		Debug.Log ("updating");
		ClearPlayersGUI ();
		int playerIndex = 0;
		foreach (PhotonPlayer p in PhotonNetwork.playerList) {
			Transform playerMenu = playerMenus[playerIndex++];
			// enable car choose option for local player
			if (p == PhotonNetwork.player) {
				playerMenu.Find("arrow-left").gameObject.SetActive(true);
				playerMenu.Find("arrow-right").gameObject.SetActive(true);
			}
			// updates icon based on car index (protected for early calls before a new player has own properties set)
			if (p.customProperties.ContainsKey("car")) {
				playerMenu.Find("Image").GetComponent<Image>().sprite = carTextures[(int) p.customProperties["car"]];
				playerMenu.Find("Text").GetComponent<Text>().text = p.name.Trim();
			}
		}
	}

	private void ClearPlayersGUI() {
		foreach (Transform t in playerMenus) {
			t.Find("Image").GetComponent<Image>().sprite = noCar;
			t.Find("Text").GetComponent<Text>().text = "";
			t.Find("arrow-left").gameObject.SetActive(false);
			t.Find("arrow-right").gameObject.SetActive(false);
		}
	}

	public void NextCar() {
		carIndex = (carIndex + 1) % carTextures.Length;
		SetCustomProperties (PhotonNetwork.player, carIndex, (int) PhotonNetwork.player.customProperties ["spawn"]);
	}

	public void PreviousCar() {
		carIndex--;
		if (carIndex < 0)
			carIndex = carTextures.Length - 1;
		SetCustomProperties (PhotonNetwork.player, carIndex, (int) PhotonNetwork.player.customProperties ["spawn"]);
	}

	public void NextTrack() {
		trackIndex = (trackIndex + 1) % trackTextures.Length;
		photonView.RPC("UpdateTrack", PhotonTargets.All, trackIndex);
	}
	
	public void PreviousTrack() {
		trackIndex--;
		if (trackIndex < 0)
			trackIndex = trackTextures.Length - 1;
		photonView.RPC("UpdateTrack", PhotonTargets.All, trackIndex);
	}

	[PunRPC]
	public void UpdateTrack(int index) {
		trackIndex = index;
		trackSprite.sprite = trackTextures [trackIndex];
	}

	// masterClient only. Calls an RPC to start the race on all clients. Called from GUI
	public void CallLoadRace() {
		PhotonNetwork.room.open = false;
		photonView.RPC("LoadRace", PhotonTargets.All);
	}

	// Loads race level (called once from masterClient)
	// Use LoadLevel from Photon, otherwise it messes up the GOs created in
	// between level changes
	// The level loaded is related to the track chosen by the Master Client (updated via RPC).
	[PunRPC]
	public void LoadRace () {
		PhotonNetwork.LoadLevel("Race" + trackIndex);
	}

	// sets and syncs custom properties on a network player (including masterClient)
	private void SetCustomProperties(PhotonPlayer player, int car, int position) {
		ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "spawn", position }, {"car", car} };
		player.SetCustomProperties(customProperties);
	}

	// Use this to go back to the menu, without leaving the lobby
	public void ResetToMenu () {
		PhotonNetwork.LeaveRoom ();
		PhotonNetwork.LoadLevel ("Menu");
	}

}
