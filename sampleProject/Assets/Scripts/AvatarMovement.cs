using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMovement : MonoBehaviour {

	// Use this for initialization
    private GramophoneDevice device;

	[Tooltip("Use to add more speed")]
	[Range(1.0f, 50.0f)]
	public float addedSpeed;



	Rigidbody m_Rigidbody;

	void Start () {

		device = GramophoneDevice.Instance();
        m_Rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

		//m_Rigidbody.velocity = transform.forward * device.GetVelocity();

		transform.parent.transform.localEulerAngles = new Vector3(0, transform.parent.transform.localEulerAngles.y + device.GetVelocity()* addedSpeed * Time.deltaTime, 0);

	}
}
