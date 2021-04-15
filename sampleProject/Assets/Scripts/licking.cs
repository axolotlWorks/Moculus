using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Moculus: lick port controller

public class licking : MonoBehaviour {

	private GramophoneDevice device;
	void Start () {
		device = GramophoneDevice.Instance();
	}

    void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnter");
        /* you can do some control here

        device.OpenA();
        device.CloseA();
        device.ControlServo(2);

        */
        device.OpenA();
    }	

    void OnTriggerExit(Collider other) {
        Debug.Log("OnCollisionExit");
        /* you can do some control here

        device.OpenA();
        device.CloseA();
        device.ControlServo(2);

        */
        device.CloseA();
    }

}
