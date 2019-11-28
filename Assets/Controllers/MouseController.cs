using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public GameObject circleCursor;

    Vector3 lastFramePosition;
    Vector3 currFramePosition;

    bool buildModeIsObjects;
    string buildModeObjectType;

    public static Tile selected;
    public static GameObject selectedObject;

    public Canvas build;
    public Canvas delete;

    void Start()
    {
        selected = null;
        buildModeIsObjects = false;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {             return;         }

        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        BuildRoad();
        UpdateCameraMovement();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
        if(tileUnderMouse != null) {
            circleCursor.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            circleCursor.transform.position = cursorPosition;
        }
        else
        {
            circleCursor.SetActive(false);
        }

        if (!buildModeIsObjects)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                    if (hit.transform != null)
                    {
                        Debug.Log("Hit " + hit.transform.gameObject.name + " " + hit.transform.gameObject);
                        selectedObject = hit.transform.gameObject;
                        delete.gameObject.SetActive(true);
                        build.gameObject.SetActive(false);
                        return;
                    }

                if (tileUnderMouse != null && selected != tileUnderMouse)
                {
                    selected = tileUnderMouse;
                    Debug.Log(selected.X + "," + selected.Y);
                    build.gameObject.SetActive(true);
                    delete.gameObject.SetActive(false);
                }
                else if (tileUnderMouse != null && selected == tileUnderMouse)
                {
                    Debug.Log("Deselected " + selected.X + "," + selected.Y);
                    selected = null;

                    build.gameObject.SetActive(false);
                    delete.gameObject.SetActive(false);
                }
            }
        }
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }

    void UpdateCameraMovement()
    {
        // Handle screen panning
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {  
            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);
        }
        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
    }

    void BuildRoad()
    {
        if (buildModeIsObjects)
        {
            if (Input.GetMouseButton(0))
            {
                Tile t = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
                if (t != null)
                {
                    WorldController.Instance.World.PlaceFurniture(buildModeObjectType, t);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                buildModeIsObjects = false;
            }
        }
        else
        {
            //When selecting objects
        }
    }

    public void SetMode_BuildRoad()
    {
        buildModeIsObjects = true;
        buildModeObjectType = "Road";
        build.gameObject.SetActive(false);
        delete.gameObject.SetActive(false);
    }
}