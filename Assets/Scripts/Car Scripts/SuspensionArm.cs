using UnityEngine;
using System.Collections;

/// <summary>
/// Used on suspension arm Meshes to move them (rotation over pivot point) coherently with correspondent wheel collider.
/// </summary>
public class SuspensionArm : MonoBehaviour {

	// The wheel collider for this wheel's suspension
	public WheelCollider wheel;

	// Original values for suspension when resting
	private Vector3 originalRotation;
	private float originalTravelPosition = 0;

	// calibration values for angles
	public float offsetAngle = 30;
	public float angleFactor = 150;
	
	void Start () {
		originalRotation = transform.localEulerAngles;
		originalTravelPosition = ComputeSuspensionTravel ();
	}
	
	// Compute suspension travel based on wheel collider movement, and update this arm
	void FixedUpdate () {
		float suspensionCourse = ComputeSuspensionTravel () - originalTravelPosition;
		transform.localEulerAngles = originalRotation;
		transform.Rotate (Vector3.up, suspensionCourse * angleFactor - offsetAngle, Space.Self);
	}

	// Calculates local Y-axis movement of wheel collider (suspension travel)
	private float ComputeSuspensionTravel() {
		Quaternion quat;
		Vector3 position;
		wheel.GetWorldPose(out position, out quat);
		Vector3 local = wheel.transform.InverseTransformPoint (position);
		return local.y;
	}
}
