using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public override void OnStartLocalPlayer()
	{
		transform.GetComponentInChildren<Camera>().enabled = true;
		transform.GetComponentInChildren<AudioListener>().enabled = true;
		transform.Find("Direction Indicator").GetComponent<MeshRenderer>().enabled = false; //well I only want to see other players direction indicators
		//proper FPS mouse control
		Cursor.lockState = CursorLockMode.Locked;
	}

	//the function that shoots the frisbee
	public GameObject frisbee;

	[Command] public void CmdFire()
	{
		Vector3 frisbeePosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 2f));
		Vector3 frisbeeRotation = Camera.main.transform.eulerAngles + frisbee.transform.eulerAngles; 
		GameObject instantiatedFrisbee = Instantiate(frisbee, frisbeePosition, Quaternion.Euler(frisbeeRotation));
		instantiatedFrisbee.GetComponent<frisbee>().frisbeeVelocity = Camera.main.transform.TransformDirection(Vector3.forward);
		NetworkServer.Spawn(instantiatedFrisbee);
	}


	void Update()
	{
		if(!isLocalPlayer)
			return;
		if(Input.GetButtonDown("Fire1"))
			CmdFire();
	}
}
