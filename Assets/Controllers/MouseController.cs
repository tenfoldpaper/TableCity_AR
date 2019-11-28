using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{

    public static MouseController Instance { get; protected set; }
    public GameObject circleCursor;
    public Collider gamePlane;
    public GameObject tileMenu;
    GameObject currentMenu;
    Vector3 dragStartPosition;
    Vector3 lastFramePosition;
    Tile tileDragStart;
    public bool draging;
    public int currentType = -1;
    List<Tile> transparentTiles = new List<Tile>();
    List<Tile> highlightedTiles = new List<Tile>();

    bool buildModeIsObjects;
    string buildModeObjectType;

    public static Tile selected;
    public static GameObject selectedObject;

    public Canvas build;
    public Canvas delete;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two mouse controllers.");
        }
        Instance = this;
        selected = null;
        buildModeIsObjects = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector3 currFramePosition = GetGamePlaneIntersectionPoint();
        Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
        UpdateCursor(tileUnderMouse);

        foreach (Tile t in highlightedTiles)
        {
            t.highlighted = false;
		}
        
        //BuildRoad();
        highlightedTiles.Clear();
        HighlightTiles(tileUnderMouse);
        foreach (Tile t in highlightedTiles)
        {
            t.highlighted = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftMouseButtonDown(currFramePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp(currFramePosition);
        }
        UpdateCamera();        
    }
    void OnLeftMouseButtonDown(Vector3 currFramePosition)
    {
        if (currentMenu != null && currentMenu.activeSelf)
        {
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>())
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            bool found = false;
            foreach (RaycastResult result in results)
            {
                //Ugly hack
                //TODO
                if (result.gameObject.name == "Image")
                {
                    found = true;
                }
            }
            if (!found)
            {
                currentMenu.SetActive(false);
            }

        }
        else
        {
            if (draging)
            {
                if (currentType == 0)
                {
                    Tile tileEnd = GetTileAtWorldCoord(currFramePosition);
                    List<Tile> set = WorldController.Instance.GetLPathSet(tileDragStart.X, tileDragStart.Y, tileEnd.X, tileEnd.Y);
                    WorldController.Instance.CreateRoad(set);
                }
                draging = false;
            }
            else
            {
                dragStartPosition = currFramePosition;
                if (currentMenu == null)
                {
                    currentMenu = Instantiate(tileMenu, currFramePosition, Quaternion.identity);
                }
                tileDragStart = GetTileAtWorldCoord(currFramePosition);
                currentMenu.transform.position = currFramePosition;
                currentMenu.SetActive(true);
                //call dynamicMenu();
                //transform.parent = currentMenu;
            }
        }
        //Todo: dynamic menu

    }
    void OnLeftMouseButtonUp(Vector3 currFramePosition)
    {
    }
    Vector3 GetGamePlaneIntersectionPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (gamePlane.Raycast(ray, out hit, 1000.0f))
        {
            return hit.point;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
    void UpdateCursor(Tile tileUnderMouse)
    {
        if (currentMenu == null || !currentMenu.activeSelf)
        {
            if (tileUnderMouse != null)
            {
                circleCursor.SetActive(true);
                Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
                circleCursor.transform.position = cursorPosition;
            }
            else
            {
                circleCursor.SetActive(false);
            }
        }
    }
    void HighlightTiles(Tile tileUnderMouse)
    {
        if (draging)
        {
            if (currentType == 0)
            {
                if (tileDragStart != null)
                {
                    highlightedTiles.AddRange(
                        WorldController.Instance.GetLPathSet(tileDragStart.X, tileDragStart.Y,
                        tileUnderMouse.X, tileUnderMouse.Y));
                }
            }
        }
        if (currentMenu != null && currentMenu.activeSelf)
        {
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                //Ugly hack
                //TODO
                if (result.gameObject.name == "Industrial")
                {
                    if (tileDragStart != null)
                    {
                        highlightedTiles.Add(tileDragStart);
                    }
                }
            }
        }
    }
    void UpdateCamera()
    {
        // Handle screen dragging
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1)) // Right or Middle mouse button
        {
            //Vector3 diff = lastFramePosition - currFramePosition;
            //Debug.Log(diff);
            //Camera.main.transform.Translate(diff);
            float speedH = 2.0f;
            float speedV = 2.0f;

            float yaw = speedH * Input.GetAxis("Mouse X");
            float pitch = -speedV * Input.GetAxis("Mouse Y");

            Camera.main.transform.eulerAngles += new Vector3(pitch, yaw, 0.0f);
        }
        float speed = 10.0f;
        float t_y = Input.GetAxis("Vertical") * speed;
        float t_x = Input.GetAxis("Horizontal") * speed;
        t_x *= Time.deltaTime;
        t_y *= Time.deltaTime;
        Camera.main.transform.Translate(new Vector3(t_x, t_y, 0));

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }
    Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;

        return WorldController.Instance.world.GetTileAt(x, y);
    }
	/*
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
		*/
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