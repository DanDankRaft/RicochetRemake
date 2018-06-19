using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class border : NetworkBehaviour {

	void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;		
	}


	void OnCollisionEnter(Collision other)
	{
		print("Hit " + other.gameObject.name);
		if(other.gameObject.GetComponent<Frisbee>() != null)
			Destroy(other.gameObject);		
	}

}
