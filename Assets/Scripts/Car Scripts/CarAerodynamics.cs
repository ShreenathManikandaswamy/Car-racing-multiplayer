using UnityEngine;
using System.Collections;

public class CarAerodynamics : MonoBehaviour {

	public float coeficient = 0.3f;
	public float frontalArea = 2;
	private Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		float speed = rb.velocity.magnitude;

		float drag = speed * coeficient * frontalArea * 1.1f * 1.29f;

		rb.AddForce (-rb.velocity * (drag + 15));
		Debug.Log (speed);
	}
}
