
namespace OpenCvSharp
{
    
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using Aruco;
    using OpenCvSharp;

    public class MarkerDetector : MonoBehaviour
    {
        public GameObject Surface;

        private Nullable<WebCamDevice> webCamDevice = null;
        private WebCamTexture webCamTexture = null;
        private Texture2D renderedTexture = null;
        private Unity.TextureConversionParams TextureParameters;

        private bool debug = true;
        private double[,] cameraMatrix;
        private List<double> distCoeffs;
        private static double[][] rvecs = null;
        private static double[][] tvecs = null;
        private static int[] ids;
        private static int nMarkers = 0;
        private float markerSize = 0.038f;

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
                    if (WebCamTexture.devices[i].name == value)
                        cameraIndex = i;
                }

                // set device up
                if (-1 != cameraIndex)
                {
                    webCamDevice = WebCamTexture.devices[cameraIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.Value.name);

                    webCamTexture.Play();
                }
                else
                {
                    throw new ArgumentException(String.Format("{0}: provided DeviceName is not correct device identifier", this.GetType().Name));
                }
            }
        }

        Dictionary dictionary;
        DetectorParameters detectorParameters;
        protected virtual void Awake()
        {
            TextureParameters = new Unity.TextureConversionParams();
            if (WebCamTexture.devices.Length > 0)
                DeviceName = WebCamTexture.devices[WebCamTexture.devices.Length - 1].name;
            Debug.Log(DeviceName);
            detectorParameters = DetectorParameters.Create();

            dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_50);
            
            cameraMatrix = new double[3, 3];
            cameraMatrix[0, 0] = 940; //f_x
            cameraMatrix[1, 1] = 931; //f_y
            cameraMatrix[2, 2] = 1;
            cameraMatrix[2, 0] = -320; //c_x
            cameraMatrix[2, 1] = -247; //c_y
            distCoeffs = null;
            //SolvePnP crashes with non-zero distortion params
            //radial
            //distCoeffs.Add(-0.471005); //k_1
            //distCoeffs.Add( 4.62132);   //k_2
            //tangential
            //distCoeffs.Add(-0.0135634); //p_1
            //distCoeffs.Add(0.0014167);  //p_2 
        }
        void OnDestroy()
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
        private void Update()
        {
            if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
            {
                if (ProcessTexture(webCamTexture, ref renderedTexture))
                {
                    RenderFrame();
                }
            }
        }
        private void RenderFrame()
        {
            if (renderedTexture != null)
            {
                // apply
                Surface.GetComponent<RawImage>().texture = renderedTexture;

                // Adjust image ration according to the texture sizes 
                Surface.GetComponent<RectTransform>().sizeDelta = new Vector2(renderedTexture.width, renderedTexture.height);
            }
        }
        private bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            // Create default parameres for detection


            Point2f[][] corners;
            Point2f[][] rejectedImgPoints;

            Mat mat = Unity.TextureToMat(input, TextureParameters);

            Mat grayMat = new Mat();
            Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

            // Detect and draw markers
            CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
            if (debug)
                CvAruco.DrawDetectedMarkers(mat, corners, ids);

            nMarkers = corners.Length;
            if (nMarkers > 0)
            {
                Aruco.Util.estimatePoseSingleMarkers(corners, markerSize, cameraMatrix, distCoeffs, ref rvecs, ref tvecs);
                if (debug)
                    for (int i = 0; i < nMarkers; ++i)
                        CvAruco.DrawAxis(mat, cameraMatrix, distCoeffs, rvecs[i], tvecs[i], markerSize);
            }

            // Create Unity output texture with detected markers
            output = Unity.MatToTexture(mat);


            return true;
        }
        public static bool GetTransformationOfMarker(int id, out Quaternion outRotation, out Vector3 translation)
        {
            int iterator = -1;
            for (int i = 0; i < nMarkers; i++)
            {
                if (id == ids[i])
                {
                    iterator = i;
                    break;
                }
            }
            if (iterator == -1)
            {
                outRotation = new Quaternion();
                translation = new Vector3();
                return false;
            }
            double[,] rotation = new double[3,3];
            Cv2.Rodrigues(rvecs[iterator], out rotation);
            outRotation = Quaternion.LookRotation(new Vector3((float) rotation[2, 0], (float)-rotation[2, 1], (float)rotation[2, 2]),
                                                  new Vector3((float) rotation[1, 0], (float)-rotation[1, 1], (float)rotation[1, 2]));
            translation = new Vector3((float)tvecs[iterator][0], (float)tvecs[iterator][1], (float)tvecs[iterator][2]);
            return true;
        }
    }
}