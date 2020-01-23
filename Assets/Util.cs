using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Util;
namespace OpenCvSharp.Aruco
{
    public class Util
    {
        public static void getSingleMarkerObjectPoints(float markerLength, OutputArray objPoints)
        {
            Mat points = objPoints.GetMat();
            points.Set<Vec3f>(0, new Vec3f(-markerLength / 2f, markerLength / 2f, 0f));
            points.Set<Vec3f>(1, new Vec3f(markerLength / 2f, markerLength / 2f, 0f));
            points.Set<Vec3f>(2, new Vec3f(markerLength / 2f, -markerLength / 2f, 0f));
            points.Set<Vec3f>(3, new Vec3f(-markerLength / 2f, -markerLength / 2f, 0f));
        }
        public static void estimatePoseSingleMarkers(Point2f[][] corners, float markerLength,
                                                    double[,] cameraMatrix, IEnumerable<double> distCoeffs,
                                                    ref double[][] rvecs, ref double[][] tvecs)
        {
            int numMarkers = corners.Length;
            if (numMarkers == 0)
                return;
            if (cameraMatrix == null)
                throw new ArgumentNullException("nameof(cameraMatrix)");
            if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
                throw new ArgumentException("");

            double[] distCoeffsArray = EnumerableEx.ToArray(distCoeffs);
            int distCoeffsLength = (distCoeffs == null) ? 0 : distCoeffsArray.Length;
            Mat matDistCoeffs = new Mat(new Size(distCoeffsLength, 1), MatType.CV_64FC1);
            //Debug.Assert(distCoeffsLength == 4 || distCoeffsLength == 0 || distCoeffsLength == 5, "WRONG AMOUNT OF DISTORTION COEFFICIENTS");
            for (int i = 0; i < distCoeffsLength; ++i)
                matDistCoeffs.Set<double>(i, distCoeffsArray[i]);
            //Debug.Log(matDistCoeffs.Cols + "," +matDistCoeffs.Rows);
            Mat matCamera = new Mat(new Size(3, 3), MatType.CV_64FC1);

            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    matCamera.Set<double>(i, j, cameraMatrix[i, j]);

            Mat markerObjPoints = new Mat(4, 1, MatType.CV_32FC3);
            getSingleMarkerObjectPoints(markerLength, markerObjPoints);
            
            Mat rvec;
            Mat tvec;
            rvec = new Mat(1, 3, MatType.CV_64F);
            tvec = new Mat(1, 3, MatType.CV_64F);
            bool extrinsicsEstimate = false;
            bool mask = false;
            if (rvecs != null && numMarkers == rvecs.Length)
            {
                mask = true;
            }
            else
            {
                mask = false;
                rvecs = new double[numMarkers][];
                tvecs = new double[numMarkers][];
            }
            Mat matCorners = new Mat(4, 1, MatType.CV_32FC2);
            for (int i = 0; i < numMarkers; i++)
            {
                matCorners.SetArray(0, 0, corners[i]);
                if (mask)
                {
                    extrinsicsEstimate = true;
                    rvec.SetArray(0, 0, rvecs[i]);
                    tvec.SetArray(0, 0, tvecs[i]);
                }
                else
                {
                    extrinsicsEstimate = false;
                }
                Cv2.SolvePnP(markerObjPoints, matCorners, matCamera, matDistCoeffs, rvec, tvec, extrinsicsEstimate);
                rvecs[i] = new double[3];
                tvecs[i] = new double[3];
                rvec.GetArray(0, 0, rvecs[i]);
                tvec.GetArray(0, 0, tvecs[i]);
            }
        }
    }
    public static void estimatePoseSingleMarkers(ref IEnumerable<Marker> markers,
                                                 double[,] cameraMatrix, IEnumerable<double> distCoeffs)
    {
        int numMarkers = markers.Length;
        if (numMarkers == 0)
            return;
        if (cameraMatrix == null)
            throw new ArgumentNullException("nameof(cameraMatrix)");
        if (cameraMatrix.GetLength(0) != 3 || cameraMatrix.GetLength(1) != 3)
            throw new ArgumentException("");

        double[] distCoeffsArray = EnumerableEx.ToArray(distCoeffs);
        int distCoeffsLength = (distCoeffs == null) ? 0 : distCoeffsArray.Length;
        Mat matDistCoeffs = new Mat(new Size(distCoeffsLength, 1), MatType.CV_64FC1);
        //Debug.Assert(distCoeffsLength == 4 || distCoeffsLength == 0 || distCoeffsLength == 5, "WRONG AMOUNT OF DISTORTION COEFFICIENTS");
        for (int i = 0; i < distCoeffsLength; ++i)
            matDistCoeffs.Set<double>(i, distCoeffsArray[i]);
        //Debug.Log(matDistCoeffs.Cols + "," +matDistCoeffs.Rows);
        Mat matCamera = new Mat(new Size(3, 3), MatType.CV_64FC1);

        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
                matCamera.Set<double>(i, j, cameraMatrix[i, j]);

        Mat markerObjPoints = new Mat(4, 1, MatType.CV_32FC3);


        Mat rvec = new Mat(1, 3, MatType.CV_64F);
        Mat tvec = new Mat(1, 3, MatType.CV_64F);
        
        Mat matCorners = new Mat(4, 1, MatType.CV_32FC2);
        foreach(Marker marker in markers)
        {
            getSingleMarkerObjectPoints(marker.size, markerObjPoints);
            matCorners.SetArray(0, 0, marker.corners);
            bool extrinsicsEstimate = false;
            if (marker.tvec != null)
            {
                extrinsicsEstimate = true;
                rvec.SetArray(0, 0, marker.rvec);
                tvec.SetArray(0, 0, marker.tvec);
            }
            else
            {
                extrinsicsEstimate = false;
                marker.rvec = new double[3];
                marker.tvec = new double[3];
            }
            Cv2.SolvePnP(markerObjPoints, matCorners, matCamera, matDistCoeffs, rvec, tvec, extrinsicsEstimate);

            rvec.GetArray(0, 0, marker.rvec);
            tvec.GetArray(0, 0, marker.tvec);
        }
    }
}
}