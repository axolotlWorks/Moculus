using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportTime : MonoBehaviour
{

    public GameObject Player;
    public GameObject teleportationTarget;

    public float timer = 3.5f;
    bool reduce = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter_state:");

        if (other.tag == "Player")
        {
            reduce = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("exit_state:");

        if (other.tag == "Player")
        {
            reduce = false;
            timer = 3.5f;
        }
    }

    void Update()
    {
        
        if (reduce == true)
        {
            Debug.Log(timer);
            timer -= 1 * Time.deltaTime;
        }

        if (timer <= 0)
        {
            Debug.Log("TELEPORT");
            reduce = false;

            //teleport
            Player.transform.position = teleportationTarget.transform.position;
        }
    }
}