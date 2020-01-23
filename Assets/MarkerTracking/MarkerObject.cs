using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerObject : MonoBehaviour
{
    //Which MarkerID to track
    public int markerId;
    //Calculate mean of previous Rotations/Translations to smooth the pose
    public bool smoothPose = true;
    //Amount of previous Poses to consider for smoothing
    public int bufferSize = 4;
    public int framesMissingTillInvisible = 5;
    int current;
    Vector4[] rotBuffer;
    Vector3[] transBuffer;
    Vector3 oldScale;
    int frames_missing = 0;
    bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        rotBuffer = new Vector4[bufferSize];
        transBuffer = new Vector3[bufferSize];
        for(int i = 0; i < bufferSize;i++)
        {
            rotBuffer[i] = Vector4.zero;
            transBuffer[i] = Vector3.zero;
        }
        current = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        Quaternion rotation;
        Vector3 translation;

        if (MarkerDetector.GetTransformationOfMarker(markerId, out rotation, out translation))
        {
            MarkerDetector.ApplyProjectionAndViewInverse(ref rotation,ref translation);
            if (!active)
            {
                active = true;
                gameObject.transform.localScale = oldScale;
                frames_missing = 0;
            }
            if (smoothPose)
            {
                rotBuffer[current] = QuatToVec4(rotation);
                transBuffer[current] = translation;
                current = (current + 1) % bufferSize;
                Vector4 meanRotation = Vector4.zero;
                Vector3 meanTranslation = Vector3.zero;
                for (int i = 0; i < bufferSize; i++)
                {
                    meanRotation += rotBuffer[i];
                    meanTranslation += transBuffer[i];
                }
                meanRotation /= bufferSize;
                meanTranslation /= bufferSize;
                transform.localRotation = Vec4ToQuat(meanRotation);
                transform.localRotation.Normalize();
                transform.localPosition = meanTranslation;
            }
            else
            {
                //
                transform.rotation = rotation;
                transform.position = translation;

            }
        }
        else
        {
            frames_missing++;
            if(active && frames_missing > framesMissingTillInvisible)
            {
                active = false;
                oldScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                gameObject.transform.localScale = new Vector3(0,0,0);
            }
        }
    }
    private static Quaternion Vec4ToQuat(Vector4 vec)
    {
        Quaternion ret = new Quaternion();
        ret.x = vec.x;
        ret.y = vec.y;
        ret.z = vec.z;
        ret.w = vec.w;
        return ret;
    }
    private static Vector4 QuatToVec4(Quaternion quat)
    {
        Vector4 ret = new Vector4();
        ret.x = quat.x;
        ret.y = quat.y;
        ret.z = quat.z;
        ret.w = quat.w;
        return ret;
    }
}
