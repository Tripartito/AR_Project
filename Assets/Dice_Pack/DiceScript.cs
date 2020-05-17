using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Steps;

public class DiceScript : MonoBehaviour {

	static Rigidbody rb;
	public uint numSides;
	private List<GameObject> sides = new List<GameObject>();

	private float thresholdCrossedAt;
	private float groundTimer = 1f;
	private GameObject gravityGround;

	[ReadOnly(true)]
	public int rollResult;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
		gravityGround = GameObject.Find("Bowl_Wall_0");

		int i = 1;
		bool listFinished = false;

		while (!listFinished)
		{
			Transform tmpTrs = transform.Find("Side_" + i++);
			if (tmpTrs != null)
				sides.Add(tmpTrs.gameObject);
			else
				listFinished = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (thresholdCrossedAt != 0f && rb.isKinematic == false && Time.time - thresholdCrossedAt > groundTimer)
		{
			Vector3 target = gameObject.transform.position + gravityGround.transform.up;	// Dice Pos + Ground Vector
			GameObject nearestSide = sides[0];
			float nearestDistance = (target - nearestSide.transform.position).magnitude;    // Nearest distance to target

			foreach (GameObject go in sides)
			{
				float goDistance = (target - go.transform.position).magnitude;
				if (goDistance < nearestDistance)
				{
					nearestSide = go;
					nearestDistance = goDistance;
				}
			}

			rollResult = int.Parse(nearestSide.name.Remove(0, 5));
			rb.isKinematic = true;
		}
	}

	public void ThrowMe()
	{
		rb.isKinematic = false;     // Reset
		thresholdCrossedAt = 0f;    // Reset

		transform.position = gravityGround.transform.position + gravityGround.transform.up * 2f;    // Reposition
		transform.rotation = UnityEngine.Random.rotation;           // Random rotate

		rb.AddForce(gravityGround.transform.up * 10f, ForceMode.Impulse);   // Move Upwards from bowl
		rb.AddForce(transform.up * 5f, ForceMode.Impulse);              // Move into a random direction
		rb.AddTorque(new Vector3(UnityEngine.Random.Range(0, 500), UnityEngine.Random.Range(0, 500), UnityEngine.Random.Range(0, 500)), ForceMode.Impulse); // Random Torque
	}

	private void OnCollisionEnter(Collision other)
	{
		if (thresholdCrossedAt == 0f && other.gameObject.name == "Bowl_Wall_0")
			thresholdCrossedAt = Time.time;
	}
}
