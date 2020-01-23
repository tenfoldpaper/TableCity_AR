using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Util;
public class Marker
{
    //public Point2f[] corners;
    public float size;
    public double[] tvec;
    public double[] rvec;
    public int id;
}
[System.Serializable]
public class MultiMarker
{
    public int id;
    public Vector2 offset;
}
[System.Serializable]
public class CameraIntrinsics
{
    public double f_x  = 640, f_y = 640;
    public double c_x = 320, c_y = 240;
    public double c_z = 0;
    public double k_1 = 0, k_2 = 0;
    public double p_1 = 0, p_2 = 0; //p_3
    public double width = 640, height = 480;
    double[,] cameraMatrix;
    public double[,] CameraMatrix
    {
        get
        {
            if (cameraMatrix == null)
            {
                // This OpenCV Library expects (for some reason?!):
                // ⎡fx 0  0⎤
                // ⎢0  fy 0⎥
                // ⎣cx cy 1⎦
                //
                cameraMatrix = new double[3, 3];
                cameraMatrix[0, 0] = f_x;
                cameraMatrix[1, 1] = f_y;
                cameraMatrix[1, 0] = c_z;
                cameraMatrix[2, 2] = 1;
                cameraMatrix[0, 2] = c_x;
                cameraMatrix[1, 2] = c_y;

            }
            return cameraMatrix;
        }
        private set
        {
        }
    }
    private double[] distortionParams;
    public double[] DistortionParams
    {
        get
        {
            if (distortionParams == null)
            {
                distortionParams = new double[4];
                distortionParams[0] = k_1;
                distortionParams[1] = k_2;
                distortionParams[2] = p_1;
                distortionParams[3] = p_2;
            }
            return distortionParams;
        }
        private set { }
    }
    public double AspectRatio
    {
        get
        {
            return width / height;
        }
        private set { }
    }
}