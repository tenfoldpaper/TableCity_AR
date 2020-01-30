using UnityEngine;
using System;
using UnityEngine.UI;
using OpenCvSharp.Aruco;
using OpenCvSharp;
[System.Serializable]
public class WebCamera
{
    public GameObject Surface;
    public int deviceNum = 0;
    public string deviceName = null;
    public bool fillscreen = true;
    public int RefreshRate;

    private bool old_fill = false;

    private WebCamDevice? webCamDevice = null;
    private WebCamTexture webCamTexture = null;
    private Texture2D renderedTexture = null;
    private OpenCvSharp.Unity.TextureConversionParams TextureParameters;
    public Vector2Int Resolution { get; set; }

    public string DeviceName
    {
        get
        {
            return (webCamDevice != null) ? webCamDevice.Value.name : null;
        }
        set
        {
            // quick test
            if (value == DeviceName)
                return;

            if (null != webCamTexture && webCamTexture.isPlaying)
                webCamTexture.Stop();

            // get device index
            int cameraIndex = -1;
            for (int i = 0; i < WebCamTexture.devices.Length && -1 == cameraIndex; i++)
            {
                if (WebCamTexture.devices[i].name.StartsWith(value))
                    cameraIndex = i;
            }

            // set device up
            if (-1 != cameraIndex)
            {
                webCamDevice = WebCamTexture.devices[cameraIndex];
                if (Resolution != null)
                    if (RefreshRate != 0)
                        webCamTexture = new WebCamTexture(webCamDevice.Value.name, (int)Resolution.x, (int)Resolution.y, RefreshRate);
                    else
                        webCamTexture = new WebCamTexture(webCamDevice.Value.name, (int)Resolution.x, (int)Resolution.y);
                else
                    webCamTexture = new WebCamTexture(webCamDevice.Value.name);
                webCamTexture.Play();
            }
            else
            {
                throw new ArgumentException(String.Format("{0}: provided DeviceName is not correct device identifier", this.GetType().Name));
            }
        }
    }
    public int DeviceNumber
    {
        get
        {
            return 0;
        }
        set
        {
            if (null != webCamTexture && webCamTexture.isPlaying)
                webCamTexture.Stop();
            if (value < WebCamTexture.devices.Length)
            {

                webCamDevice = WebCamTexture.devices[value];
                if (Resolution != null)
                    if (RefreshRate != 0)
                        webCamTexture = new WebCamTexture(webCamDevice.Value.name, (int)Resolution.x, (int)Resolution.y, RefreshRate);
                    else
                        webCamTexture = new WebCamTexture(webCamDevice.Value.name, (int)Resolution.x, (int)Resolution.y);
                else
                    webCamTexture = new WebCamTexture(webCamDevice.Value.name);

                webCamTexture.Play();

            }
            else
            {
                throw new ArgumentException(String.Format("{0}: provided DeviceNumber is not correct device identifier", this.GetType().Name));
            }
        }
    }


    public void Awake()
    {
        Debug.Log("Available Cameras:");
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            Debug.Log(WebCamTexture.devices[i].name);
        }
        TextureParameters = new OpenCvSharp.Unity.TextureConversionParams();
        if (deviceName == null)
        {
            if (WebCamTexture.devices.Length > 0 && deviceNum < WebCamTexture.devices.Length)
                DeviceNumber = deviceNum;

        }
        else
        {
            if (WebCamTexture.devices.Length > 0)
                DeviceName = deviceName;

        }
        Debug.Log(DeviceName);
    }
    public void Destroy()
    {
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }
            webCamTexture = null;
        }

        if (webCamDevice != null)
        {
            webCamDevice = null;
        }
    }
    public void Update(Func<Mat, Mat> processTexture)
    {
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            //Destroy(renderedTexture);
            renderedTexture = OpenCvSharp.Unity.MatToTexture(processTexture(OpenCvSharp.Unity.TextureToMat(webCamTexture, TextureParameters)));
            RenderFrame();

        }
    }
    private void RenderFrame()
    {
        if (renderedTexture != null && Surface != null)
        {
            // apply
            Vector2 oldSize = Surface.GetComponent<RectTransform>().sizeDelta;
            Texture tmp = Surface.GetComponent<RawImage>().texture;
            Surface.GetComponent<RawImage>().texture = renderedTexture;
            UnityEngine.Object.Destroy(tmp);
            //Delete Old texture to avoid memory leak

            // Adjust image ration according to the texture sizes 
            Vector2 transformRect = new Vector2(renderedTexture.width, renderedTexture.height);
            Surface.GetComponent<RectTransform>().sizeDelta = transformRect;
            if (fillscreen)
            {
                if (!old_fill || transformRect != oldSize)
                {
                    Vector3 scale = Surface.GetComponent<RectTransform>().localScale;
                    if (Mathf.Abs((transformRect.x * scale.x) - Screen.width) > 0.5 || Mathf.Abs((transformRect.y * scale.y) - Screen.height) > 0.5)
                    {
                        float widthRatio = Screen.width / transformRect.x;
                        float heightRatio = Screen.height / transformRect.y;
                        Surface.transform.localScale = new Vector3(widthRatio, heightRatio, 1.0f);
                        Debug.Log(transformRect);
                        old_fill = true;
                    }
                }
            }
            else
            {
                old_fill = false;
                Surface.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
}
