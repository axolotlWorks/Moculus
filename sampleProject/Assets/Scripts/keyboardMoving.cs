using UnityEngine;
using System.Collections;

public class keyboardMoving : MonoBehaviour
{
    [Range(0.0f, 50.0f)]
    public float moveSpeed;

    [Range(0.0f, 100.0f)]
    public float turnSpeed;

    [Range(0.0f, 5.0f)]
    public float scaleFactor;

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
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + moveSpeed * Time.deltaTime,0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + -moveSpeed * Time.deltaTime, 0);
        }

    }
}