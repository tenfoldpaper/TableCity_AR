using UnityEngine;
using System;
using UnityEngine.UI;
using OpenCvSharp.Aruco;
using OpenCvSharp;

public class MarkerDetector : MonoBehaviour
{

    public WebCamera webCamera;
    public CameraIntrinsics cameraIntrinsics;
    public Camera virtualCam;

    public float markerSize = 0.03683f;
    public bool debug = true;
    //Renders OpenCV Debug information ontop of frame



    //Tracking state statically stored TODO more elegant design here
    private static float p00sub = 0;
    private static float p11sub = 0;
    private static double[][] rvecs = null;
    private static double[][] tvecs = null;
    public static int[] ids;
    private static int nMarkers = 0;
    private static Camera cam;

        
    private Mat grayMat = null;


    Dictionary dictionary;
    DetectorParameters detectorParameters;
    protected virtual void Awake()
    {
        webCamera.Awake();
        cam = virtualCam;

        detectorParameters = DetectorParameters.Create();
        // Mostly default parameters, just here to easily play with them
        detectorParameters.DoCornerRefinement = true;
        detectorParameters.AdaptiveThreshConstant = 7;
        detectorParameters.AdaptiveThreshWinSizeMax = 23;
        detectorParameters.AdaptiveThreshWinSizeMin = 3;
        detectorParameters.AdaptiveThreshWinSizeStep = 10;
        detectorParameters.CornerRefinementMaxIterations = 30;
        detectorParameters.CornerRefinementMinAccuracy = 0.1;
        detectorParameters.CornerRefinementWinSize = 5;
        detectorParameters.ErrorCorrectionRate = 0.6;
        detectorParameters.MarkerBorderBits = 1;
        detectorParameters.MaxErroneousBitsInBorderRate = 0.35;
        detectorParameters.MaxMarkerPerimeterRate = 4.0;
        detectorParameters.MinCornerDistanceRate = 0.05;
        detectorParameters.MinDistanceToBorder = 3;
        detectorParameters.MinMarkerDistanceRate = 0.05;
        detectorParameters.MinMarkerPerimeterRate = 0.03;
        detectorParameters.MinOtsuStdDev = 5.0;
        detectorParameters.PerspectiveRemoveIgnoredMarginPerCell = 0.13;
        detectorParameters.PerspectiveRemovePixelPerCell = 8;

        dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_250);

        //Match FOV of real camera
        //Debug.Log(cam.projectionMatrix);
        float fov = Mathf.Atan(0.5f * (float)cameraIntrinsics.height / (float)cameraIntrinsics.f_y) * Mathf.Rad2Deg * 2.0f;
        float aspect = (float)cameraIntrinsics.AspectRatio;
        //Debug.Log(fov);
        //Debug.Log(aspect);
        /*
        aspect = w / h
        tanFov = tan( fov_y * 0.5 );

        p[0][0] = 2*n/(r-l) = 1.0 / (tanFov * aspect)
        p[1][1] = 2*n/(t-b) = 1.0 / tanFov
        The field of view angle along the Y-axis in degrees:

        fov = 2.0*atan( 1.0/prjMatrix[1][1] ) * 180.0 / PI;
        */
        //cam.fieldOfView = Mathf.Atan(0.5f * (float) cameraIntrinsics.height / (float)cameraIntrinsics.f_y) * Mathf.Rad2Deg * 2.0f;
        float tanFov = 0.5f * (float)cameraIntrinsics.height / (float)cameraIntrinsics.f_y;
        //cam.aspect = (float) cameraIntrinsics.AspectRatio;
        p00sub = 1.0f / (tanFov * (float)cameraIntrinsics.AspectRatio);
        p11sub = 1.0f / tanFov;
        //Debug.Log(cam.projectionMatrix);
        //Debug.Log(cam.worldToCameraMatrix);
        //Debug.Log(cam.transform.localToWorldMatrix);
        grayMat = new Mat();
    }
    void OnDestroy()
    {
        webCamera.Destroy();
    }
    private void Update()
    {
        webCamera.Update(ProcessTexture);
    }
    private Mat ProcessTexture(Mat mat)
    {
        
        //Destroy(output);

        Point2f[][] corners;
        Point2f[][] rejectedImgPoints;
        

           

        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

        // Detect and draw markers
        CvAruco.DetectMarkers(grayMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
        if (debug)
            CvAruco.DrawDetectedMarkers(mat, corners, ids);

        nMarkers = corners.Length;
        if (nMarkers > 0)
        {
            Util.EstimatePoseSingleMarkers(corners, markerSize, cameraIntrinsics.CameraMatrix, 
                cameraIntrinsics.DistortionParams, ref rvecs, ref tvecs);
            if (debug)
                for (int i = 0; i < nMarkers; ++i)
                {
                    CvAruco.DrawAxis(mat, cameraIntrinsics.CameraMatrix, cameraIntrinsics.DistortionParams, rvecs[i], tvecs[i], markerSize);
                }
        }

        return mat;
    }
    public static bool GetTransformationOfMarker(int id, out Quaternion outRotation, out Vector3 outTranslation)
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
            outTranslation = new Vector3();
            return false;
        }
        ConvertOpenCVToUnity(rvecs[iterator], tvecs[iterator], out outRotation, out outTranslation);
        return true;
    }
    public static void ConvertOpenCVToUnity(double[] rvec, double[] tvec, out Quaternion outRotation, out Vector3 outTranslation)
    {
        double[,] R;
        
        Cv2.Rodrigues(rvec, out R);
      
        Vector3 forwardVector, upVector;
        forwardVector.x = -(float)R[0, 2];
        forwardVector.y = (float)R[1, 2];
        forwardVector.z = -(float)R[2, 2];
        upVector.x = (float)R[0, 1];
        upVector.y = (-(float)R[1, 1]); //Derived from negative scaling on y and z axis, seems to offer the correct result
        upVector.z = (float)R[2, 1];

        outRotation = Quaternion.LookRotation(forwardVector, upVector);

        //local to world space
        //outRotation = cam.transform.rotation * outRotation;

        Vector3 local;
        local.x = (float)tvec[0];
        local.y = -(float)tvec[1]; // OpenCV has different Coord Sys
        local.z = (float)tvec[2];
        outTranslation = local;
        //outTranslation = local;
        //local to world Space
        //outTranslation = local;
        //Essentialy invert View and Projection Matrix (latter one on X and Y to combat FOV/AspectRatio)
        //local.x *= p00sub / cam.projectionMatrix.m00;
        //local.y *= p11sub / cam.projectionMatrix.m11;
        //float oldp00 = cam.projectionMatrix.m00;
        //float oldp11 = cam.projectionMatrix.m11;
        //local.x *= p00sub / oldp00;
        //local.y *= p11sub / oldp11;
        //outTranslation = cam.transform.TransformPoint(local);
        // |     ___
        // |    /  /| Pos_cube 
        // |   |  | /
        // |   |__|/
        // 
        // =Projection*ViewMatrix*pos
    }
    public static void ApplyProjectionAndViewInverse(ref Quaternion outRotation, ref Vector3 outTranslation)
    {
        outRotation = cam.transform.rotation * outRotation;
        float oldp00 = cam.projectionMatrix.m00;
        float oldp11 = cam.projectionMatrix.m11;
        outTranslation.x *= p00sub / oldp00;
        outTranslation.y *= p11sub / oldp11;
        outTranslation = cam.transform.TransformPoint(outTranslation);
    }
}
