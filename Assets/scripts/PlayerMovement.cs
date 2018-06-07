using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	float yMovement;
	public float speed = 1;
	public float sensitivity = 1;
	public float jumpHeight;

	float xRotation;
	float camRotation;

	CharacterController playerController;
	public override void OnStartLocalPlayer()
	{
		playerController = GetComponent<CharacterController>();
		lastCollidedObject = gameObject;
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if(!isLocalPlayer)
			GetComponent<MeshRenderer>().material.color = Color.blue;
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

	public LayerMask groundedLayers;
	bool isGrounded()
	{
		const float wiggleRoom = 0.2f;
		 
		//cast a ray that ignores the player, if it hits something within height/2 + wiggle room, than we are grounded
		Ray groundRay = new Ray(transform.position, Vector3.down);
		return Physics.Raycast(groundRay, playerController.height/2 + wiggleRoom, groundedLayers);
	}
	

	///<summary> a function that emulates Rigidbody.AddForce() </summary>
	///<param name="force"> the force added, equals to the distance the player will move </param>
	public void AddForce(Vector3 force)
	{
		StartCoroutine(AddForceInternal(force));
	}

	IEnumerator AddForceInternal(Vector3 force)
	{
		float movementSpeed = force.magnitude;
		float initialMovementSpeed = movementSpeed;
		float totalDistance = 0f;
		while ( totalDistance < force.magnitude)
		{
			playerController.Move(force.normalized * movementSpeed / 60);
			totalDistance += movementSpeed / 60f;
			movementSpeed -= initialMovementSpeed / 600f;
			yield return new WaitForEndOfFrame();
		}
	}

	GameObject lastCollidedObject;
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.gameObject.GetComponent<bouncer>() != null && lastCollidedObject != hit.gameObject)
		{
			AddForce(Vector3.Reflect(hit.moveDirection.normalized * 20, hit.normal));
		}
		if(hit.gameObject.tag != "Floor")
			lastCollidedObject = hit.gameObject;
	}


}
