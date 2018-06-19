using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
public class Launcher : NetworkBehaviour {

	public float jumpHeight;
	public Vector3 movementVector;
	public float time;

	public Transform landingIndicator;
	void Update()
	{
		float effectiveHeight = 10;
		for(float jumpDelta = jumpHeight; jumpDelta > 0; jumpDelta -= .98f)
			effectiveHeight += jumpDelta;
		Vector3 midpoint =new Vector3(transform.position.x + movementVector.x/2, transform.position.y+effectiveHeight, transform.position.z + movementVector.z/2);
		Debug.DrawLine(transform.position, midpoint, Color.magenta);
		Debug.DrawLine(midpoint, transform.position+movementVector, Color.magenta);
		Debug.DrawLine(transform.position, transform.position + movementVector, Color.red);
	}
}
