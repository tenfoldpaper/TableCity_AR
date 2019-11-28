using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{

    public static WorldController Instance { get; protected set; }
    public Sprite floorSprite;
    public Sprite roadSprite;

    public World world { get; protected set; }
    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }

        Instance = this;
        // Create a world with Empty tiles
        world = new World();

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                GameObject tile_go = new GameObject();
                Tile tile_data = world.GetTileAt(x, y);
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, false);
                // Add a sprite renderer, but don't set a sprite since they are all empty
                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                tile_data.gameObject = tile_go;

                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); }  );
                
            }
        }
        world.RandomizeTiles();
    }
    //Creates an L set inbetween start and endpoint
    //TODO apply a function to each tile instead
    public List<Tile> GetLPathSet(int x_start, int y_start, int x_end, int y_end)
    {
        List<Tile> set = new List<Tile>();
        int x_s = x_start;
        int y_s = y_start;
        int x_e = x_end;
        int y_e = y_end;
        int x = x_s < x_e ? x_s : x_e;
        int y = y_s < y_e ? y_s : y_e;
        if (Mathf.Abs(x_s - x_e) > Mathf.Abs(y_s - y_e))
        {
            for (; x < (x_s > x_e ? x_s : x_e); x++)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
            for (; y <= (y_s > y_e ? y_s : y_e); y++)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
        }
        else
        {
            for (; y < (y_s > y_e ? y_s : y_e); y++)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
            for (; x <= (x_s > x_e ? x_s : x_e); x++)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
        }
        return set;
    }
    public void CreateRoad(List<Tile> set)
    {
        foreach(Tile t in set) {
            t.Type = Tile.TileType.Road;
        }
    }

    float randomizeTileTimer = 2f;
    // Update is called once per frame
    void Update()
    {
        

        
    }
    // Polling vs Cullback
    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == Tile.TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else if (tile_data.Type == Tile.TileType.Road)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = roadSprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }
}
