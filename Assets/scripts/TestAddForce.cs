using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddForce : MonoBehaviour {

	[SerializeField] PlayerMovement player;
	public void forceTesting()
	{
		player.AddForce(Vector3.one * 3, 5, 2);
	}

	void Update()
	{
		player = FindObjectOfType<PlayerMovement>();
	}
}
