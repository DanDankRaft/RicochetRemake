using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class frisbee : NetworkBehaviour {

	public static float speed = 20;
	public Player ownerPlayer;

	Vector3 frisbeeVelocity;
	void Start()
	{
		frisbeeVelocity = ownerPlayer.transform.TransformDirection(Vector3.forward * speed);
		GetComponent<MeshRenderer>().materials[1].SetColor("_EmissionColor", ownerPlayer.playerColor);
		GetComponent<MeshRenderer>().materials[1].SetColor("_Color", ownerPlayer.playerColor);
	}

	void Update()
	{
		GetComponent<Rigidbody>().velocity = frisbeeVelocity;
	}

	void OnCollisionEnter(Collision other)
	{
		Debug.Log("something got hit!");
		if(other.gameObject.GetComponent<Player>() != null)
		{
			Debug.Log("Player got hit boiii");
			Destroy(gameObject);
		}
		frisbeeVelocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, other.contacts[0].normal);
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Debug.Log("Player got hit!");
		Destroy(gameObject);
	}
}
