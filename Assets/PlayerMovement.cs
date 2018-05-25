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
	void Update()
	{
		if(isLocalPlayer) //only affect the player we are playing as
		{
			//movement
			float xMovement = Input.GetAxisRaw("Horizontal") * speed;
			float zMovement = Input.GetAxisRaw("Vertical") * speed;

			//gravity
			if(GetComponent<CharacterController>().isGrounded)
				yMovement = 0;
			else
				yMovement -= .98f * Time.deltaTime;

			//jumping
			if(Input.GetButtonDown("Jump") && GetComponent<CharacterController>().isGrounded)
				yMovement = jumpHeight;
			GetComponent<CharacterController>().Move(transform.TransformDirection(xMovement, yMovement, zMovement));

			//rotation
			xRotation += Input.GetAxisRaw("Mouse X") * sensitivity;
			camRotation += Input.GetAxis("Mouse Y") * sensitivity;
			camRotation = Mathf.Clamp(camRotation, -45, 45);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, xRotation, transform.eulerAngles.z);
			transform.GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(-camRotation, transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}


	public override void OnStartLocalPlayer()
	{
		transform.position = FindObjectOfType<NetworkManager>().transform.position; 
	}
}
