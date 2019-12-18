using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite;
    public Sprite roadSprite;

    //Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<Object, GameObject> objectGameObjectMap;
    Dictionary<string, Sprite> objectSprites;
    //List<Building> buildings;

    // The world and tile data
    public World world { get; protected set; }

    // Use this for initialization
    void Start()
    {
        LoadSprites();

        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;

        // Create a world with Empty tiles
        world = new World();

        world.RegisterObjectCreated(OnObjectCreated);

        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        //furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();
        objectGameObjectMap = new Dictionary<Object, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                // Get the tile data
                Tile tile_data = world.GetTileAt(x, y);

                // This creates a new GameObject and adds it to our scene.
                GameObject tile_go = new GameObject();

                // Add our tile/GO pair to the dictionary.

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, false);
                // Add a sprite renderer, but don't set a sprite since they are all empty
                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                tile_data.gameObject = tile_go;

                // Register our callback so that our GameObject gets updated whenever
                // the tile's type changes.
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        // Shake things up, for testing.
        world.RandomizeTiles();
    }
    public GameObject WrapInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation);
    }
    void LoadSprites()
    {
        objectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");

        Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites)
        {
            Debug.Log(s);
            objectSprites[s.name] = s;
        }
    }
    //Creates an L set inbetween start and endpoint
    //TODO apply a function to each tile instead
    public List<Tile> GetLPathSet(int x_start, int y_start, int x_end, int y_end)
    {
        List<Tile> set = new List<Tile>();
        int x = x_start;
        int y = y_start;
        int x_sign = x_end > x_start ? 1 : -1;
        int y_sign = y_end > y_start ? 1 : -1;
        if (Mathf.Abs(x_start - x_end) > Mathf.Abs(y_start - y_end))
        {
            for (; x != x_end; x += x_sign)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
            for (; y != y_end; y += y_sign)
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
            for (; y != y_end; y += y_sign)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
            for (; x != x_end; x += x_sign)
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
        foreach (Tile t in set)
        {
            //t.Type = Tile.TileType.Road;
            WorldController.Instance.world.PlaceObject("Road", t);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // This function should be called automatically whenever a tile's type gets changed.
    void OnTileTypeChanged(Tile tile_data)
    {
        GameObject tile_go = tile_data.gameObject;

        if (tile_go == null)
        {
            Debug.LogError("tileGameObject is null");
            return;
        }
        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == Tile.TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return world.GetTileAt(x, y);
    }

    public void OnObjectCreated(Object obj)
    {
        GameObject obj_go = new GameObject();

        // Add our tile/GO pair to the dictionary.
        objectGameObjectMap.Add(obj, obj_go);

        obj_go.name = obj.objectType + "_" + obj.tile.X + "_" + obj.tile.Y;
        obj_go.transform.position = new Vector3(obj.tile.X, obj.tile.Y, 0);
        obj_go.transform.SetParent(this.transform, true);

        if (obj.objectType == "Road")
        {
            obj_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForObject(obj);



            // Register our callback so that our GameObject gets updated whenever
            // the object's into changes.

            // Debug.Log(GetSpriteForObject(obj));

            obj.RegisterOnChangedCallback(OnObjectChanged);
        }
    }

    void OnObjectChanged(Object obj)
    {
        // Make sure the road's graphics are correct.

        if (objectGameObjectMap.ContainsKey(obj) == false)
        {
            Debug.LogError("OnObjectChanged -- trying to change visuals for object not in our map.");
            return;
        }
        GameObject obj_go = objectGameObjectMap[obj];

        obj_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForObject(obj);
    }

    Sprite GetSpriteForObject(Object obj)
    {
        if (obj.linksToNeighbour == false)
        {
            return objectSprites[obj.objectType];
        }
        // Otherwise, the sprite name is more complicated.

        string spriteName = obj.objectType + "_";

        // Check for neighbours North, East, South, West

        int x = obj.tile.X;
        int y = obj.tile.Y;

        Tile t;

        t = world.GetTileAt(x, y + 1);
        if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
        {
            spriteName += "N";
        }
        t = world.GetTileAt(x + 1, y);
        if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
        {
            spriteName += "E";
        }
        t = world.GetTileAt(x, y - 1);
        if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
        {
            spriteName += "S";
        }
        t = world.GetTileAt(x - 1, y);
        if (t != null && t.objects != null && t.objects.objectType == obj.objectType)
        {
            spriteName += "W";
        }

        if (objectSprites.ContainsKey(spriteName) == false)
        {
            Debug.LogError("GetSpriteForInstalledObject -- No sprites with name: " + spriteName);
            return null;
        }
        return objectSprites[spriteName];
    }
}