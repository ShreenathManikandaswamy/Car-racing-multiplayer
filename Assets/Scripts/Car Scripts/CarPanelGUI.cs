using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CarPanelGUI : MonoBehaviour {

	private CarRaceControl car;
	private CarGUI carGUI;
	public Image playerIcon;
	public Sprite noCar;
	public Text playerMessage;

	public bool showTime = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (car == null) {
			playerIcon.sprite = noCar;
			playerMessage.text = "";
			return;
		}
		playerIcon.sprite = carGUI.GetIcon();
		string text = car.photonView.owner.name;
		if (showTime) {
			// formating time (see FormatTime(float time) bellow)
			text += " - " + FormatTime(car.raceTime);
		}
		playerMessage.text = text;
	}

	public string FormatTime(float time) {
		string minutes = Mathf.Floor(time / 60).ToString("00");
		string seconds = Mathf.Floor(time % 60).ToString("00");
		string milis = Mathf.Floor((time*1000) % 1000).ToString("000");
		return minutes + ":" + seconds + ":" + milis; 
	}

	public void SetCar(CarRaceControl car) {
		this.car = car;
		carGUI = car.GetComponent<CarGUI> ();
	}
}
