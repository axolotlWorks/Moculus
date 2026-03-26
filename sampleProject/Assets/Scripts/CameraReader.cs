using Unity.Burst.Intrinsics;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CameraReader : MonoBehaviour
{
    public RenderTextureSensorComponent sensorComponent;
    public RenderTexture targetTexture; // assign in inspector, match CameraSensor resolution
    public GameObject debugScreen; // assign a Plane/Quad in the scene to preview the webcam feed

    private WebCamTexture webcamTexture;

    private void Start()
    {
        /*/WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);*/
        //string obsCamName = "OBS Virtual Camera";
        string obsCamName = "ATM Camera PIL";
        webcamTexture = new WebCamTexture(obsCamName, targetTexture.width, targetTexture.height, 30);
        webcamTexture.Play();
        sensorComponent.RenderTexture = targetTexture;

        if (debugScreen != null)
        {
            debugScreen.GetComponent<Renderer>().material.mainTexture = targetTexture;
        }
    }

    // Blit every frame so the texture is always fresh when observations are collected.
    // Uses LateUpdate to run after agent decisions, and always blits (even if the
    // webcam didn't update) so the RenderTexture is never stale.
    private void LateUpdate()
    {
        Graphics.Blit(webcamTexture, targetTexture);
    }
}