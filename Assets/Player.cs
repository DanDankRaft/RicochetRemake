using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public override void OnStartLocalPlayer()
	{
		transform.GetComponentInChildren<Camera>().enabled = true;
		transform.GetComponentInChildren<AudioListener>().enabled = true;
		transform.Find("Direction Indicator").GetComponent<MeshRenderer>().enabled = false;
	}
}
