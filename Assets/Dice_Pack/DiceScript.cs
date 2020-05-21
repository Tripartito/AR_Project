using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Steps;

public class DiceScript : MonoBehaviour {

	private Rigidbody rb;
	public uint numSides;
	private List<GameObject> sides = new List<GameObject>();

	private GameObject camera;
	private TextMesh rollText;

	private float thresholdCrossedAt;
	private float groundTimer = 1f;
	private GameObject gravityGround;

	public Vector3 spawnPos = new Vector3();

	[Header("Read Only"), ReadOnly(true)]
	public int rollResult;
	public bool rollFinished = true;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();

		gravityGround = GameObject.Find("Bowl_Wall_0");
		rollText = transform.Find("Dice_Roll_Text").gameObject.GetComponent<TextMesh>();
		camera = GameObject.Find("ARCamera");

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
					rollResult = i + 1;	//Array pos is roll number - 1
					nearestDistance = goDistance;
				}
			}

			rollText.gameObject.transform.position = gameObject.transform.position + gravityGround.transform.up * 0.5f;
			rollText.text = rollResult.ToString();

			transform.SetParent(gravityGround.transform.parent, true);

			rb.isKinematic = true;
			rollFinished = true;
		}

		if (rollFinished)
		{
			rollText.gameObject.transform.LookAt(camera.transform);
			rollText.gameObject.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
		}
	}

	public void ThrowMe()
	{
		rollText.text = "";

		rollFinished = false;       // Reset
		rb.isKinematic = false;     // Reset
		thresholdCrossedAt = 0f;	// Reset

		transform.position = gravityGround.transform.rotation * spawnPos;    // Reposition
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
