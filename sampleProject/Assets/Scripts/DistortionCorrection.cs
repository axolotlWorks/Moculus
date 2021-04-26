using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DistortionCorrection : MonoBehaviour
{
    [SerializeField]
    private bool UseChromaticFix = false;

    #region Coefficients
    [Header("Distortion coefficents with no chromatic aberration")]
    [SerializeField, Range(-50, 50)]
    private float AllK1;

    [SerializeField, Range(-50, 50)]
    private float AllK2;

    [SerializeField, Range(-50, 50)]
    private float AllK3;

    [Header("Chromatic aberration distortion coefficents")]
    [SerializeField, Range(-50, 50)]
    private float RedK1;

    [SerializeField, Range(-50, 50)]
    private float RedK2;

    [SerializeField, Range(-50, 50)]
    private float RedK3;

    [SerializeField, Range(-50, 50)]
    private float GreenK1;

    [SerializeField, Range(-50, 50)]
    private float GreenK2;

    [SerializeField, Range(-50, 50)]
    private float GreenK3;

    [SerializeField, Range(-50, 50)]
    private float BlueK1;

    [SerializeField, Range(-50, 50)]
    private float BlueK2;

    [SerializeField, Range(-50, 50)]
    private float BlueK3;
    #endregion


    [SerializeField, Tooltip("If set, it will overide the camera render with the distorted texture")]
    private Texture testTexture;

    [SerializeField, Tooltip("Pressing S will save the next post processed image by the distortion shader to this location.")]
    private string SavePath = @"C:\";
    
    private bool saveTexture = false;
    private RenderTexture saveTex;
    private Material material;

    void Awake()
    {
        material = new Material(Shader.Find("Custom/DistortionShader"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            saveTexture = true;
        }
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (UseChromaticFix)
        {
            material.SetFloat("_RedK1", RedK1);
            material.SetFloat("_RedK2", RedK2);
            material.SetFloat("_RedK3", RedK3);

            material.SetFloat("_GreenK1", GreenK1);
            material.SetFloat("_GreenK2", GreenK2);
            material.SetFloat("_GreenK3", GreenK3);

            material.SetFloat("_BlueK1", BlueK1);
            material.SetFloat("_BlueK2", BlueK2);
            material.SetFloat("_BlueK3", BlueK3);

        }
        else
        {
            material.SetFloat("_RedK1", AllK1);
            material.SetFloat("_RedK2", AllK2);
            material.SetFloat("_RedK3", AllK3);

            material.SetFloat("_GreenK1", AllK1);
            material.SetFloat("_GreenK2", AllK2);
            material.SetFloat("_GreenK3", AllK3);

            material.SetFloat("_BlueK1", AllK1);
            material.SetFloat("_BlueK2", AllK2);
            material.SetFloat("_BlueK3", AllK3);
        }

        //If test texture set, fill the screen with it
        if (testTexture != null)
        {
            testTexture.wrapMode = TextureWrapMode.Repeat;
            Graphics.Blit(testTexture, source); 
        }

        //Apply distortion correction shader
        Graphics.Blit(source, destination, material);
                
        if (saveTexture)
        {
            saveTexture = false;
            saveTex = new RenderTexture(source);
            Graphics.Blit(source, saveTex, material);
            SaveTexture();
            Destroy(saveTex);
        }
    }

    // Use this for initialization
    public void SaveTexture()
    {
        byte[] bytes = toTexture2D(saveTex).EncodeToPNG();
        System.IO.File.WriteAllBytes($@"{SavePath}\save.png", bytes);
    }

    private Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        Destroy(tex);
        return tex;
    }


}