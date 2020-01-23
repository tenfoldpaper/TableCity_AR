using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = new Quaternion();
        Vector3 translation = new Vector3();
        //if(OpenCvSharp.MarkerDetector.GetTransformationOfMarker(id, out rotation, out translation))
        //{
            //transform.rotation = rotation;
            //transform.position = translation;
        //}
        
    }
}
