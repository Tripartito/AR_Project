using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiceScript : MonoBehaviour {

	static Rigidbody rb;
	private float heightTreshold;
	private List<GameObject> sides = new List<GameObject>();

	private float thresholdCrossedAt;
	private float groundTimer = 0.5f;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();
		heightTreshold = GameObject.Find("Bowl_Wall_0").transform.position.y + 0.02f;

		int i = 1;
		bool listFinished = false;

		while (!listFinished)
		{
			GameObject tmpGO = GameObject.Find(gameObject.name + "_Side_" + i++);
			if (tmpGO != null)
				sides.Add(tmpGO);
			else
				listFinished = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			rb.isKinematic = false;
			thresholdCrossedAt = 0f;

			transform.localPosition = new Vector3(0, 0.2f, 0);
			transform.rotation = UnityEngine.Random.rotation;

			rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
			rb.AddForce(transform.up * 0.1f, ForceMode.Impulse);
			rb.AddTorque(new Vector3(UnityEngine.Random.Range(0, 500), UnityEngine.Random.Range(0, 500), UnityEngine.Random.Range(0, 500)), ForceMode.Impulse);
		}

		if (thresholdCrossedAt == 0f)
		{
			if (transform.position.y < heightTreshold)
				thresholdCrossedAt = Time.time;
		}
		else if (Time.time - thresholdCrossedAt > groundTimer && rb.isKinematic == false)
		{
			GameObject highest = sides[0];

			foreach (GameObject go in sides)
				if (go.transform.position.y > highest.transform.position.y)
					highest = go;

			Text text = GameObject.Find("Roll_Result").GetComponent<Text>();

			text.text = "You rolled a " + highest.name.Remove(0, 9);
			rb.isKinematic = true;
		}
	}
}
