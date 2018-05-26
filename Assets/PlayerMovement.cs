using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	[SerializeField] float yMovement;
	public float speed = 1;
	public float sensitivity = 1;
	public float jumpHeight;

	float xRotation;
	float camRotation;

	CharacterController playerController;
	public static int count = 0;
	public override void OnStartLocalPlayer()
	{
		playerController = GetComponent<CharacterController>();
		transform.position = FindObjectOfType<NetworkManager>().transform.position; //gives us a changable spawn position TODO: find if there is a built-in way of doing this

		transform.GetComponentInChildren<Camera>().enabled = true;
		transform.GetComponentInChildren<AudioListener>().enabled = true;

		count++;
		transform.name = "Player " + count;
	}

	void Update()
	{
		if(isLocalPlayer) //only affect the player we are playing as
		{
			//movement
			float xMovement = Input.GetAxis("Horizontal") * speed;
			float zMovement = Input.GetAxis("Vertical") * speed;

			//gravity
			if(playerController.isGrounded)
				yMovement = 0;
			else
				yMovement -= .98f * Time.deltaTime;

			//jumping
			if(Input.GetButtonDown("Jump") && isGrounded()) //normal isGrounded didn't have enough wiggle room for jumping to feel good so I made a custom one
				yMovement = jumpHeight;
			playerController.Move(transform.TransformDirection(xMovement, yMovement, zMovement));

			//rotation
			xRotation += Input.GetAxisRaw("Mouse X") * sensitivity;
			camRotation += Input.GetAxis("Mouse Y") * sensitivity;
			camRotation = Mathf.Clamp(camRotation, -45, 45);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, xRotation, transform.eulerAngles.z);
			transform.GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(-camRotation, transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}

	bool isGrounded()
	{
		const float wiggleRoom = 0.2f;
		//cast a ray that ignores the player, if it hits something within height/2 + wiggle room, than we are grounded
		Ray groundRay = new Ray(transform.position, Vector3.down);
		LayerMask mask = ~(1 << 8 << 9);
		//Debug.Log(Physics.Raycast(groundRay, playerController.height/2 + wiggleRoom));
		return Physics.Raycast(groundRay, playerController.height/2 + wiggleRoom);
	}
}
