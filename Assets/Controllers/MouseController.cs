using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public GameObject circleCursor;
    public Collider gamePlane;
    public GameObject tileMenu;
    GameObject currentMenu;
    Vector3 dragStartPosition;
    Vector3 lastFramePosition;
    Tile tileDragStart;
    bool draging;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void onLeftMouseButtonDown(Vector3 currFramePosition)
    {
        dragStartPosition = currFramePosition;
        if(currentMenu != null && currentMenu.activeSelf)
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
                }
            }
            if (!found)
            {
                currentMenu.SetActive(false);
            }
           
        } else
        {
            if (currentMenu == null)
            {
                currentMenu = Instantiate(tileMenu, currFramePosition, Quaternion.identity);
            }
            currentMenu.transform.position = currFramePosition;
            currentMenu.SetActive(true);
            //call dynamicMenu();
            //transform.parent = currentMenu;
        }
        //Todo: dynamic menu

    }
    void onLeftMouseButtonUp(Vector3 currFramePosition)
    {
        if (draging)
        {
            //if(building_road) {
            //  WorldController.CreateRoad(dragStartPosition, currFramePosition);
            //}
            draging = false;
        }
        /*
        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.y);
        int end_x = Mathf.FloorToInt(currFramePosition.x);
        int end_y = Mathf.FloorToInt(currFramePosition.y);

        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }

        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    t.Type = Tile.TileType.Floor;
                }
            }
        }
        */
    }
    // Update is called once per frame
    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 currFramePosition;
        if (gamePlane.Raycast(ray, out hit, 1000.0f))
        {
            currFramePosition = hit.point;
        }
        else
        {
            return;
        }

        Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
        if (currentMenu == null || !currentMenu.activeSelf)
        {
            if (tileUnderMouse != null) {
                circleCursor.SetActive(true);
                Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
                circleCursor.transform.position = cursorPosition;
                //Debug.Log(cursorPosition.x);       
            }
            else
            {
                circleCursor.SetActive(false);
            }
        }
        // Handle drags
        if (Input.GetMouseButtonDown(0))
        {
            onLeftMouseButtonDown(currFramePosition);
        }


        if (Input.GetMouseButtonUp(0))
        {
            onLeftMouseButtonUp(currFramePosition);
        }

        // Handle left mouse clicks
        if (Input.GetMouseButtonUp(0))
        {
            if(tileUnderMouse != null)
            {
                if(tileUnderMouse.Type == Tile.TileType.Empty)
                {
                    //tileUnderMouse.Type = Tile.TileType.Floor;
                }
                else
                {
                    //tileUnderMouse.Type = Tile.TileType.Empty;
                }
            }
        }

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
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        GameObject.FindObjectOfType<WorldController>();

        return WorldController.Instance.world.GetTileAt(x, y);
    }
}
