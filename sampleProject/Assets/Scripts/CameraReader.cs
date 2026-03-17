using UnityEngine;

public class CameraReader : MonoBehaviour
{
    public GameObject CameraScreen;
    private WebCamTexture webcamTexture;

    private void Start()
    {
        webcamTexture = new WebCamTexture();
        CameraScreen.GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();
        Display.displays[1].Activate();
    }
}
