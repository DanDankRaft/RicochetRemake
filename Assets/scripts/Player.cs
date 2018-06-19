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
		CmdGeneratePlayerColor();
	}

	//the function that shoots the frisbee
	public GameObject frisbee;
	[SyncVar] public Color playerColor;
	[SyncVar] public Player.PlayerInfo info;
	public Player.PlayerInfo lastAttackingPlayer;

	[Command]
	void CmdGeneratePlayerColor()
	{
		playerColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
		info = new PlayerInfo("This Player", playerColor);
	}

	//information to send to frisbee when firing
	public struct PlayerInfo
	{
		public string name;
		public Color color;

		public PlayerInfo(string newName, Color newColor)
		{
			this.name = newName;
			this.color = newColor;
		}
	}

	public void Fire()
	{
		Vector3 frisbeePosition = GetComponentInChildren<Camera>().ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 2f));
		Vector3 frisbeeRotation = GetComponentInChildren<Camera>().transform.eulerAngles + frisbee.transform.eulerAngles;
		Vector3 initialFrisbeeVelocity = GetComponentInChildren<Camera>().transform.TransformDirection(Vector3.forward);
		PlayerInfo thisPlayerInfo = new PlayerInfo(gameObject.name, playerColor);
		CmdFire(frisbeePosition, frisbeeRotation, initialFrisbeeVelocity, thisPlayerInfo);
	
	}

	[Command]
	void CmdFire(Vector3 frisbeePosition, Vector3 frisbeeRotation, Vector3 initialFrisbeeVelocity, PlayerInfo thisPlayer)
	{
		GameObject instantiatedFrisbee = Instantiate(frisbee, frisbeePosition, Quaternion.Euler(frisbeeRotation));	
		instantiatedFrisbee.GetComponent<Frisbee>().frisbeeVelocity = initialFrisbeeVelocity;
		instantiatedFrisbee.GetComponent<Frisbee>().senderPlayer = thisPlayer;
		NetworkServer.Spawn(instantiatedFrisbee);
		instantiatedFrisbee.GetComponent<Frisbee>().RpcSetColor();
	}


	void Update()
	{
		if(!isLocalPlayer)
			return;
		if(Input.GetButtonDown("Fire1"))
			Fire();
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		print("hit something");

		if(hit.gameObject.GetComponent<border>() != null)
		{
			Kill();
		}
		if(hit.gameObject.GetComponent<Launcher>() != null)
		{
			Launcher playerLauncher = hit.gameObject.GetComponent<Launcher>();
			StartCoroutine(GetComponent<PlayerMovement>().AddLauncherForce(hit.gameObject.GetComponent<Launcher>().movementVector, hit.gameObject.GetComponent<Launcher>().jumpHeight, playerLauncher.time));
		}


	}

	public void Kill()
	{
		Debug.Log("oh no player died");
		transform.position = Vector3.zero; //TODO: change to proper spawn point
	}
}
