using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Util;
namespace OpenCvSharp.Aruco
{
    public class Marker
    {
        public Point2f[] corners;
        public float size;
        public double[] tvec;
        public double[] rvec;
        public int id;
    }
}