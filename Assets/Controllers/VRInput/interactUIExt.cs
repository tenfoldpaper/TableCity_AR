using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

public class interactUIExt : BaseInputModule
{
    public static VRInputModule Instance { get; protected set; }

    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean uiAction;
    public Camera controllerCamera;

    public LayerMask tileMask;

    private Vector3 hitPoint; // Point where the raycast hits

    public GameObject circleCursor;
    public GameObject tileMenu;
    GameObject currentMenu;
    Vector3 dragStartPosition;
    Vector3 lastFramePosition;

    public bool dragging;
    public int currentType = -1;
    List<Tile> transparentTiles = new List<Tile>();
    List<Tile> highlightedTiles = new List<Tile>();

    bool buildModeIsObjects;
    string buildModeObjectType;

    public static Tile selected;
    public static GameObject selectedObject;

    private RaycastHit hit;
    private Tile startTile;
    private Tile sourceTile;
    private Tile tileBeingPointed;
    private Vector3 currFramePosition;

    private PointerEventData m_Data = null;
    private GameObject m_CurrentObject = null;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        m_Data = new PointerEventData(eventSystem);

        selected = null;
        buildModeIsObjects = false;
        Debug.Log("interactUI started");
        tileMenu.GetComponent<Canvas>().worldCamera = controllerCamera;
    }

    // Update is called once per frame
    public override void Process()
    {
        m_Data.Reset();
        m_Data.position = new Vector2(controllerCamera.pixelWidth / 2, controllerCamera.pixelHeight / 2);
        //Debug.Log(controllerCamera.transform.position);
        //Debug.Log(controllerPose.transform.position);
        eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        //Debug.Log(m_RaycastResultCache.ToString());
        m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;
        //Debug.Log(m_CurrentObject.name);
        m_RaycastResultCache.Clear();

        HandlePointerExitAndEnter(m_Data, m_CurrentObject);

        if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, tileMask))
        {
            
            hitPoint = hit.point;
            //Debug.Log("Tile has been hit at: " +hitPoint);
            tileBeingPointed = GetTileAtWorldCoord(hitPoint);
            UpdateCursor();

            foreach (Tile t in highlightedTiles)
            {
                t.highlighted = false;
            }
            highlightedTiles.Clear();
            HighlightTiles();
            foreach (Tile t in highlightedTiles)
            {
                t.highlighted = true;
            }
            
        }
        else
        {
            tileBeingPointed = null;
            //UpdateCursor();
        }
        if (uiAction.GetStateDown(handType))
        {
            //Debug.Log("Down");
            OnUiActionButtonDown(m_Data);
        }
        if (uiAction.GetStateUp(handType))
        {
            //Debug.Log("Up");
            OnUiActionButtonUp();
        }

        if(currentMenu != null)
        {
           // Debug.Log(currentMenu.activeSelf);
        }
        else
        {
           // Debug.Log("currentMenu is null");
        }
    }

    public PointerEventData GetData()
    {
        return m_Data;
    }

    void OnUiActionButtonDown(PointerEventData data)
    {
        Debug.Log("Dragging: " + dragging);
        if (currentMenu == null || !currentMenu.activeSelf) {
            sourceTile = tileBeingPointed;
        }

        if (currentMenu != null && currentMenu.activeSelf)
        {

            Debug.Log("Path 1");
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = new Vector2(controllerCamera.pixelWidth / 2, controllerCamera.pixelHeight / 2);
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            bool found = false;
            foreach (RaycastResult result in results)
            {
                //Debug.Log("Result: " + result.gameObject.name);
                if (result.gameObject.name == "Image")
                {
                    found = true;
                    //break;
                }
            }
            if (!found)
            {
                currentMenu.SetActive(false);
            }
        }
        else
        {
            if (dragging)
            {
                Debug.Log("Path 2");

                if (currentType == 0)
                {
                    Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, tileMask);
                    Tile tileEnd = GetTileAtWorldCoord(hit.point);
                    List<Tile> set = WorldController.Instance.GetLPathSet(startTile.X, startTile.Y, tileEnd.X, tileEnd.Y);
                    Debug.Log("CreateRoad");
                    WorldController.Instance.CreateRoad(set);
                }
                dragging = false;
            }
            else
            {
                Debug.Log("Path 3");
                //dragStartPosition = sourceTile;
                if (currentMenu == null)
                {

                    Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, tileMask);

                    currentMenu = Instantiate(tileMenu, hit.point, Quaternion.identity);

                    //Debug.Log(currentMenu.activeSelf);
                }
                startTile = GetTileAtWorldCoord(hitPoint);
                //Debug.Log(startTile.X + " " + startTile.Y);
                currentMenu.transform.position = currFramePosition;
                currentMenu.SetActive(true);
                //call dynamicMenu();
                //transform.parent = currentMenu;
            }
        }
    }

    void OnUiActionButtonUp()
    {
        if (currentMenu != null && currentMenu.activeSelf)
        {
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = new Vector2(controllerCamera.pixelWidth / 2, controllerCamera.pixelHeight / 2);
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                //Debug.Log("Result: " + result.gameObject.name);
                switch (result.gameObject.name)
                {
                    case "Industrial":
                        //Debug.Log("Industrail let go");
                        //Debug.Log("Start tile: " + startTile.X + " " + startTile.Y);
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.industry, startTile);
                        currentMenu.SetActive(false);

                        currentType = -1;
                        break;
                    case "Entertainment":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.entertainment, startTile);
                        currentMenu.SetActive(false);
                        currentType = -1;
                        //Debug.Log("Start tile: " + startTile.X + " " + startTile.Y);
                        break;
                    case "Residential":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.residental, startTile);
                        currentMenu.SetActive(false);
                        currentType = -1;
                        //Debug.Log("Start tile: " + startTile.X + " " + startTile.Y);
                        break;
                    case "Road":
                        Debug.Log("Start tile: " + sourceTile.X + " " + sourceTile.Y);
                        currentMenu.SetActive(false);
                        dragging = true;
                        currentType = 0;
                        break;
                }
            }
        }
    }

    void UpdateCursor()
    {
        // Draw a new cursor position if current menu is present AND active
        if(currentMenu != null && currentMenu.activeSelf)
        {
            Debug.Log(sourceTile);
            Vector3 cursorPosition = new Vector3(sourceTile.X, 0.1f, sourceTile.Y);
            circleCursor.transform.position = cursorPosition;
            return;
        }
        if (currentMenu == null || !currentMenu.activeSelf)
        {
            if (tileBeingPointed != null)
            {
                circleCursor.SetActive(true);
                Vector3 cursorPosition = new Vector3(tileBeingPointed.X, 0.1f, tileBeingPointed.Y);
                //Debug.Log("current cursor position: " + cursorPosition);
                circleCursor.transform.position = cursorPosition;
            }
        }
    }

    void HighlightTiles()
    {
        if (dragging)
        {
            if (startTile != null)
            {
                highlightedTiles.AddRange(WorldController.Instance.GetLPathSet(
                    startTile.X, startTile.Y, tileBeingPointed.X, tileBeingPointed.Y));
            }
        }
        if (currentMenu != null && currentMenu.activeSelf)
        {
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = new Vector2(controllerCamera.pixelWidth / 2, controllerCamera.pixelHeight / 2);
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
    Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = (int)coord.x;
        int y = (int)coord.z;
        Debug.Log( x +" "+ y);
        return WorldController.Instance.world.GetTileAt(x, y);
    }

    public void SetMode_BuildRoad()
    {
        buildModeIsObjects = true;
        buildModeObjectType = "Road";
    }
}
