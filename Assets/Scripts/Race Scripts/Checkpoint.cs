using UnityEngine;
using System.Collections;

/// <summary>
/// Static checkpoints for registering race progress (CarRaceControl).
/// Forms a linked list based on the "next" attribute.
/// </summary>
public class Checkpoint : MonoBehaviour {

	// must be be true on last (lap end) checkpoint only.
	public bool isStartFinish;

	public Checkpoint next;
	
}
