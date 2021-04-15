using UnityEngine;
using System.Collections;

public class barrier : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float moveSpeed;

    void Update()
    {
        //translate

        if (Input.GetKey(KeyCode.C))
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        if (Input.GetKey(KeyCode.V))
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
    }
}