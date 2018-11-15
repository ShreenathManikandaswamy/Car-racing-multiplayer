using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float timeout = 3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeout -= Time.deltaTime;
		if (timeout <= 0)
			Destroy (gameObject);
	}
}
