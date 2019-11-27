using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMenuScript : MonoBehaviour
{
    private Vector3 initialScaling;
    private Vector3 initialTransform;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        initialScaling = transform.localScale;
        initialTransform = transform.position;
        if(camera == null)
        {
            camera = Camera.main;
        }
    }
    void OnEnable()
    {
        if(gameObject.activeSelf == true)
        {
            initialTransform = transform.position;
            if (camera == null)
            {
                camera = Camera.main;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        float distance = new Plane(camera.transform.forward, camera.transform.position).GetDistanceToPoint(transform.position);
        transform.localScale = initialScaling * distance / 10.0f;
        transform.position = (initialTransform+0.7f* Vector3.Scale(transform.localScale,
            new Vector3(transform.GetComponent<RectTransform>().sizeDelta.x, transform.GetComponent<RectTransform>().sizeDelta.y, 0.0f)))
            * 0.7f +  camera.transform.position*0.3f;
    }
}
