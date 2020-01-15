using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

public class interactUI : MonoBehaviour
{
    public static interactUI Instance { get; protected set; }
    public Transform cameraRigTransform;
    public Transform headTransform; // The camera rig's head

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
    private Tile tileBeingPointed;
    private Vector3 currFramePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null)
        {
            Debug.LogError("There should never be two controllers.");
        }
        Instance = this;
        selected = null;
        buildModeIsObjects = false;
        Debug.Log("interactUI started");
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, tileMask))
        {
            hitPoint = hit.point;
            //Debug.Log("Tile has been hit at: " +hitPoint);
            tileBeingPointed = GetTileAtWorldCoord(hitPoint);
            UpdateCursor();
            
            foreach(Tile t in highlightedTiles)
            {
                t.highlighted = false;
            }
            highlightedTiles.Clear();
            HighlightTiles();
            foreach(Tile t in highlightedTiles)
            {
                t.highlighted = true;
            }
            if (uiAction.GetStateDown(handType))
            {
                OnUiActionButtonDown();
            }
            if (uiAction.GetStateUp(handType))
            {
                Debug.Log("trigger2");
                OnUiActionButtonUp();
            }
        }
        else
        {
            tileBeingPointed = null;
            //UpdateCursor();
        }
    }

    void OnUiActionButtonDown()
    {
        if (currentMenu != null && currentMenu.activeSelf)
        {
            Debug.Log("Path 1");
            PointerEventData eventData = new PointerEventData(GetComponent<EventSystem>());
            eventData.position = new Vector2(controllerCamera.pixelWidth / 2, controllerCamera.pixelHeight / 2);
            Debug.Log(eventData.position);
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            bool found = false;
            foreach (RaycastResult result in results)
            {
                Debug.Log("Result: " + result.gameObject.name);
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
                if(currentType == 0)
                {
                    Tile tileEnd = GetTileAtWorldCoord(currFramePosition);
                    List<Tile> set = WorldController.Instance.GetLPathSet(startTile.X, startTile.Y, tileEnd.X, tileEnd.Y);
                    WorldController.Instance.CreateRoad(set);
                }
                dragging = false;
            }
            else
            {
                dragStartPosition = currFramePosition;
                if (currentMenu == null)
                {
                    currentMenu = Instantiate(tileMenu, currFramePosition, Quaternion.identity);

                    Debug.Log(currentMenu.activeSelf);
                }
                startTile = GetTileAtWorldCoord(hitPoint);
                Debug.Log(startTile.X + " " + startTile.Y);
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
            Debug.Log(eventData.position);
            List<RaycastResult> results = new List<RaycastResult>();
            currentMenu.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                Debug.Log("Result: " + result.gameObject.name);
                switch (result.gameObject.name)
                {
                    case "Industrial":
                        Debug.Log("Industrail let go");
                        Debug.Log("Start tile: " + startTile.X + " " + startTile.Y);
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.industry, startTile);
                        currentMenu.SetActive(false);
                        break;
                    case "Entertainment":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.entertainment, startTile);
                        currentMenu.SetActive(false);
                        Debug.Log("Start tile: " + startTile.X + " " + startTile.Y);
                        break;
                    case "Residential":
                        BuildManager.Instance.SetObjectToBuild(BuildManager.Instance.residental, startTile);
                        currentMenu.SetActive(false);
                        Debug.Log("Start tile: " + startTile.X + " " + startTile.Y);
                        break;
                }
            }
        }
    }

    void UpdateCursor()
    {
        if (currentMenu == null || currentMenu.activeSelf)
        {
            if(tileBeingPointed != null)
            {
                circleCursor.SetActive(true);
                Vector3 cursorPosition = new Vector3(tileBeingPointed.X, 0.1f, -tileBeingPointed.Y);
                //Debug.Log("current cursor position: " + cursorPosition);
                circleCursor.transform.position = cursorPosition;
            }
            else
            {
                circleCursor.SetActive(false);
            }
        }
        else
        {
            circleCursor.SetActive(false);
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
        if(currentMenu != null && currentMenu.activeSelf)
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
        int y = -(int)coord.z;
        //Debug.Log( x +" "+ y);
        return WorldController.Instance.world.GetTileAt(x, y);
    }

}
