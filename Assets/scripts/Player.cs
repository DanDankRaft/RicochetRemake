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
		playerColor = Random.ColorHSV(0, 1, 1, 1, 1, 1, 0, 0);
	}

	public int shootCounter = 3;
	void Update()
	{
		if(Input.GetButtonDown("Fire1") && isLocalPlayer)
			Fire();	
	}

	public void Fire()
	{
		if(shootCounter <= 0)
			return;
		shootCounter--;
		CmdFire();
	}

	public Color playerColor;
	public GameObject frisbeePrefab;

	[Command]
	void CmdFire()
	{
		GameObject frisbeeInstantiated = Instantiate(frisbeePrefab, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 2f)), frisbeePrefab.transform.rotation);
		frisbeeInstantiated.GetComponent<frisbee>().ownerPlayer = this;
		//TODO: once I add player color support, I need to change the frisbee's color accordingly
		//TODO: once I add powerup support, make sure the instantiation fits the powerup
		NetworkServer.Spawn(frisbeeInstantiated);
		StartCoroutine(GiveAmmoBack());
	}

	IEnumerator GiveAmmoBack()
	{
		yield return new WaitForSeconds(2);
		shootCounter++;
	}

}
