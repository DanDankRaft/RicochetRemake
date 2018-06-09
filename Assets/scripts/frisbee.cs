using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Frisbee : NetworkBehaviour {

	public static float speed = 20;
	public LayerMask hitMask;
	[SyncVar] public Player.PlayerInfo senderPlayer;
	[SyncVar] public Vector3 frisbeeVelocity;


	[ClientRpc]
	public void RpcSetColor()
	{
		GetComponent<MeshRenderer>().materials[1].SetColor("_EmissionColor", senderPlayer.color);
		GetComponent<MeshRenderer>().materials[1].SetColor("_Color", senderPlayer.color);
	}

	void Update()
	{
		if(!isServer)
			return;
		
		GetComponent<Rigidbody>().velocity = frisbeeVelocity * speed;	
		
		//colliisons don't seem to work very well with native OnCollisionEnter so we're using raycasting for this one.

		Ray hitRay = new Ray(transform.position, frisbeeVelocity.normalized);
		RaycastHit hit;
		float wiggleRoom = 0.5f;
		if(Physics.Raycast(hitRay, out hit, GetComponent<Collider>().bounds.extents.x + wiggleRoom, hitMask))
		{
			if(hit.collider.gameObject.GetComponent<Player>() != null)
			{
				Debug.Log(hit.collider.gameObject.name + " Died!");
			}
			frisbeeVelocity = Vector3.Reflect(frisbeeVelocity, hit.normal);
		}
	}
}
