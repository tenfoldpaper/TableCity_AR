using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject circleCursor;

    Vector3 dragStartPosition;
    Vector3 lastFramePosition;
    Tile tileDragStart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
        if(tileUnderMouse != null) {
            circleCursor.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            circleCursor.transform.position = cursorPosition;
            Debug.Log(cursorPosition.x);
        }
        else
        {
            circleCursor.SetActive(false);
        }

        // Handle drags
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currFramePosition;
        }


        if (Input.GetMouseButtonUp(0))
        {
            int start_x = Mathf.FloorToInt(dragStartPosition.x);
            int start_y = Mathf.FloorToInt(dragStartPosition.y);
            int end_x = Mathf.FloorToInt(currFramePosition.x);
            int end_y = Mathf.FloorToInt(currFramePosition.y);

            if(end_x < start_x)
            {
                int tmp = end_x;
                end_x = start_x;
                start_x = tmp;
            }

            if(end_y < start_y)
            {
                int tmp = end_y;
                end_y = start_y;
                start_y = tmp;
            }

            for(int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.world.GetTileAt(x, y);
                    if(t != null)
                    {
                        t.Type = Tile.TileType.Floor;
                    }
                }
            }
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
            Vector3 diff = lastFramePosition - currFramePosition;
            Debug.Log(diff);
            Camera.main.transform.Translate(diff);
        }

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
