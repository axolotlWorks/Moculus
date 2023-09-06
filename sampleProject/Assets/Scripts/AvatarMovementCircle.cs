using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMovementCircle : MonoBehaviour
{
    [Range(0.0f, 50.0f)]
    public float moveSpeed;

  

    void Update()
    {
        //translate

        /*if (Input.GetKey(KeyCode.UpArrow))
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        if (Input.GetKey(KeyCode.DownArrow))
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime, Space.World);*/

        //Rotate

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + moveSpeed * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + -moveSpeed * Time.deltaTime, 0);
        }

    }
}
