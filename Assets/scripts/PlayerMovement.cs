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

	float xMovement;
	float zMovement;
	void Update()
	{
		if(isLocalPlayer) //only affect the player we are playing as
		{
			//movement
			xMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
			zMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;

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
			camRotation = Mathf.Clamp(camRotation, -90, 90);
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

	public float forceDuration;
	IEnumerator AddForceInternal(Vector3 force)
	{
		float lastTime = 0;
		float time = Time.deltaTime;
		while(time < forceDuration)
		{
			if(force.y != 0) yMovement = 0;
			if(force.x != 0) xMovement = 0;
			if(force.z != 0) zMovement = 0;

			Vector3 interpolatedMovement = new Vector3(Sinerp(0, force.x, time/forceDuration),Sinerp(0, force.y, time/forceDuration),Sinerp(0, force.z, time/forceDuration)) -
			new Vector3(Sinerp(0, force.x, lastTime/forceDuration),Sinerp(0, force.y, lastTime/forceDuration),Sinerp(0, force.z, lastTime/forceDuration));
			playerController.Move(interpolatedMovement);
			lastTime = time;
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	///<summary> a function that sine interpolates between inputA and inputB </summary>
	///<param name="inputA"> the initial value </param>
	///<param name="inputB"> the final value </param>
	///<param name="inputT"> what percent of the way between inputA and inputB should we interpolate? </param>
	public static float Sinerp(float inputA, float inputB, float inputT)
	{
		float t = Mathf.Clamp01(inputT);
		float delta = inputB - inputA;
		return Mathf.Sin(Mathf.PI * t * 0.5f) * delta + inputA;
	}

	public IEnumerator AddLauncherForce(Vector3 force, float extraHeight, float time = 1.5f)
	{
		playerController.Move(Vector3.up * 10);
		Vector3 flatMovement = new Vector3(force.x, 0, force.z);
		float totalDistance = 0;
		yMovement = extraHeight;
		while(totalDistance < flatMovement.magnitude)
		{
			playerController.Move(flatMovement/time * Time.deltaTime);
			totalDistance += flatMovement.magnitude/time * Time.deltaTime;
			print("loop got to " + totalDistance);
			if(isGrounded())
				break;
		
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
