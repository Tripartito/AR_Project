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

	[Header("Read Only"), ReadOnly(true)]
	public int rollResult;
	public bool rollFinished = true;

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

		numSides = (uint)sides.Count();
	}

	// Update is called once per frame
	void Update () {
		if (!rollFinished && thresholdCrossedAt != 0f && Time.time - thresholdCrossedAt > groundTimer)
		{
			Vector3 target = gameObject.transform.position + gravityGround.transform.up;	// Dice Pos + Ground Vector

			float nearestDistance = (target - sides[0].transform.position).magnitude;	// Default roll at 1
			rollResult = 1;

			for (int i = 1; i < numSides; ++i)
			{
				float goDistance = (target - sides[i].transform.position).magnitude;
				if (goDistance < nearestDistance)
				{
					rollResult = i + 1;
					nearestDistance = goDistance;
				}
			}

			rb.isKinematic = true;
			rollFinished = true;
		}
	}

	public void ThrowMe()
	{
		rollFinished = false;       // Reset
		rb.isKinematic = false;     // Reset
		thresholdCrossedAt = 0f;	// Reset

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
