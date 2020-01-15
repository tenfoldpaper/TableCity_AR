using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;

public class MouseController : MonoBehaviour
{

    public static MouseController Instance { get; protected set; }
    public GameObject circleCursor;
    public Collider gamePlane;
    public GameObject tileMenu;
    GameObject currentMenu;
    Vector3 dragStartPosition;
    Vector3 lastFramePosition;
    
    public bool draging;
    public int currentType = -1;
    List<Tile> transparentTiles = new List<Tile>();
    List<Tile> highlightedTiles = new List<Tile>();

    bool buildModeIsObjects;
    string buildModeObjectType;

    public static Tile selected;
    public static GameObject selectedObject;

    private Tile startTile;
    private Tile tileUnderMouse;
    private Vector3 currFramePosition;
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
        currFramePosition = GetGamePlaneIntersectionPoint();
        try
        {
            tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
        }
        catch
        {
            Debug.Log("Null received");
        }
        UpdateCursor();

        foreach (Tile t in highlightedTiles)
        {
            t.highlighted = false;
		}

        //BuildRoad(currFramePosition);
        highlightedTiles.Clear();
        HighlightTiles();
        foreach (Tile t in highlightedTiles)
        {
            t.highlighted = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftMouseButtonDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp();
        }
        UpdateCamera();
    }
    void OnLeftMouseButtonDown()
    {
        if (currentMenu != null && currentMenu.activeSelf)
        {
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = Input.mousePosition;
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
                    break;
                }
            }
            if (!found)
            {
                currentMenu.SetActive(false);
            }
            //BuildManager.Instance.SetObjectToBuild(startTile);

        }
        else
        {
            if (draging)
            {
                if (currentType == 0)
                {
                    Tile tileEnd = GetTileAtWorldCoord(currFramePosition);
                    List<Tile> set = WorldController.Instance.GetLPathSet(startTile.X, startTile.Y, tileEnd.X, tileEnd.Y);
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
                startTile = GetTileAtWorldCoord(currFramePosition);
                currentMenu.transform.position = currFramePosition;
                currentMenu.SetActive(true);
                //call dynamicMenu();
                //transform.parent = currentMenu;
            }
        }
        //Todo: dynamic menu

    }
    void OnLeftMouseButtonUp()
    {
        if (currentMenu != null && currentMenu.activeSelf)
        {
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                switch (result.gameObject.name)
                {
                    
                    case "Industrial":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.industry, startTile);
                        currentMenu.SetActive(false);
                        break;
                    case "Entertainment":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.entertainment, startTile);
                        currentMenu.SetActive(false);
                        break;
                    case "Residential":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.residental, startTile);
                        currentMenu.SetActive(false);
                        break;
                    case "Water":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.watertower, startTile);
                        currentMenu.SetActive(false);
                        break;
                    case "Electricity":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.powerplant, startTile);
                        currentMenu.SetActive(false);
                        break;
                }
            }
        }
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
    void UpdateCursor()
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
    void HighlightTiles()
    {
        if (draging)
        {
            if (currentType == 0)
            {
                if (startTile != null)
                {
                    highlightedTiles.AddRange(WorldController.Instance.GetLPathSet(
                        startTile.X, startTile.Y, tileUnderMouse.X, tileUnderMouse.Y));
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
                //Debug.Log(result.gameObject.name);
                switch (result.gameObject.name)
                {
                    case "Industrial":
                    case "Entertainment":
                    case "Residential":
                        if (startTile != null)
                        {
                            highlightedTiles.Add(startTile);
                        }
                        break;
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

    

    void BuildRoad(Vector3 currFramePosition)
    {
        Debug.Log("Called buildroad");
        if (buildModeIsObjects)
        {
            if (Input.GetMouseButton(0))
            {
                Tile t = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
                if (t != null)
                {
                    WorldController.Instance.world.PlaceFurniture(buildModeObjectType, t);
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
        //build.gameObject.SetActive(false);
        //delete.gameObject.SetActive(false);
    }
}