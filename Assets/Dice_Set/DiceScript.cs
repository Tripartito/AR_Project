using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour {

	static Rigidbody rb;
	public static Vector3 diceVelocity;
	private float heightTreshold;

	private float thresholdCrossedAt;
	private float groundTimer = 0.5f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		heightTreshold = 0.02f;//GameObject.Find("Bowl_Wall_4").transform.position.y + 0.02f;
	}
	
	// Update is called once per frame
	void Update () {
		diceVelocity = rb.velocity;

		if (Input.GetKeyDown (KeyCode.Space)) {
			rb.isKinematic = false;
			thresholdCrossedAt = 0f;

			//DiceNumberTextScript.diceNumber = 0;
			transform.localPosition = new Vector3(0, 0.2f, 0);
			rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
			rb.AddForce(transform.up * 0.1f, ForceMode.Impulse);
			rb.AddTorque(new Vector3(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500)), ForceMode.Impulse);
		}

		if (thresholdCrossedAt == 0f && transform.position.y < heightTreshold)
			thresholdCrossedAt = Time.time;
		else if (thresholdCrossedAt != 0f && Time.time - thresholdCrossedAt > groundTimer && rb.isKinematic == false)
			rb.isKinematic = true;
	}
}
