using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using Photon;

/// <summary>
/// Synchronizes car position/rotation between peers and performs dead
/// reckoning.
/// 
/// If car is controlled by local player, reads position/rotation/input
/// data from Transform and CarInput and sends to peers.
/// 
/// If it's a remote car, receives data from network to cache correct
/// position/rotation, which are smoothly interpolated, and set input
/// values on CarInput, which performs dead reckoning between synchonization
/// frames.
/// </summary>
public class NetworkCar : PunBehaviour
{
	// the CarInput to read/write input data from/to
    private CarInput m_CarInput;
	private Rigidbody rb;

	// cached values for correct position/rotation (which are then interpolated)
	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Vector3 currentVelocity;
	private float updateTime = 0;

    private void Awake()
    {
		m_CarInput = GetComponent<CarInput>();
		rb = GetComponent<Rigidbody>();
    }

	/// <summary>
	/// If it is a remote car, interpolates position and rotation
    /// received from network. 
	/// </summary>
    public void FixedUpdate()
    {
		if (!photonView.isMine) {
			Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);
			transform.position = Vector3.Lerp (transform.position, projectedPosition, Time.deltaTime * 4);
			transform.rotation = Quaternion.Lerp (transform.rotation, this.correctPlayerRot, Time.deltaTime * 4);
		}
	}

	/// <summary>
	/// At each synchronization frame, sends/receives player input, position
	/// and rotation data to/from peers/owner.
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			//We own this car: send the others our input and transform data
			stream.SendNext((float)m_CarInput.Steer);
			stream.SendNext((float)m_CarInput.Accell);
			stream.SendNext((float)m_CarInput.Handbrake);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(rb.velocity);
		}
		else {
			//Remote car, receive data
			m_CarInput.Steer = (float)stream.ReceiveNext();
			m_CarInput.Accell = (float)stream.ReceiveNext();
			m_CarInput.Handbrake = (float)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			currentVelocity = (Vector3)stream.ReceiveNext();
			updateTime = Time.time;
		} 
	}
}

