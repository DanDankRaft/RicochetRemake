using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] float yMovement;
	public float speed = 1;
	public float sensitivity = 1;

	float xRotation;
	float camRotation;
	void Update()
	{
		//movement
		float xMovement = Input.GetAxisRaw("Horizontal") * speed;
		float zMovement = Input.GetAxisRaw("Vertical") * speed;

		if(GetComponent<CharacterController>().isGrounded)
			yMovement = 0;
		else
			yMovement -= 9.8f * Time.deltaTime;

		GetComponent<CharacterController>().Move(transform.TransformDirection(xMovement, yMovement, zMovement));

		//rotation
		xRotation += Input.GetAxisRaw("Mouse X") * sensitivity;
		camRotation += Input.GetAxis("Mouse Y") * sensitivity;
		camRotation = Mathf.Clamp(camRotation, -45, 45);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, xRotation, transform.eulerAngles.z);
		transform.GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(-camRotation, transform.eulerAngles.y, transform.eulerAngles.z);
	}
}
