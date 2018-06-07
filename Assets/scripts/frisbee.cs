using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class frisbee : NetworkBehaviour {

	public static float speed = 20;

	[SyncVar] public Vector3 frisbeeVelocity;

	void Update()
	{
		if(isServer)
			GetComponent<Rigidbody>().velocity = frisbeeVelocity * speed;		
	}
}
