using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{

    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite; 

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;

    Dictionary<string, Sprite> furnitureSprites;

    // The world and tile data
    public World World { get; protected set; }

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
        World = new World();

        World.RegisterFurnitureCreated(OnFurnitureCreated);

        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                // Get the tile data
                Tile tile_data = World.GetTileAt(x, y);

                // This creates a new GameObject and adds it to our scene.
                GameObject tile_go = new GameObject();

                // Add our tile/GO pair to the dictionary.
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);

                // Add a sprite renderer, but don't bother setting a sprite
                // because all the tiles are empty right now.
                tile_go.AddComponent<SpriteRenderer>();

                // Register our callback so that our GameObject gets updated whenever
                // the tile's type changes.
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        // Shake things up, for testing.
        World.RandomizeTiles();
    }

    void LoadSprites()
    {
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");

        Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites)
        {
            Debug.Log(s);
            furnitureSprites[s.name] = s;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This function should be called automatically whenever a tile's type gets changed.
    void OnTileTypeChanged(Tile tile_data)
    {

        if (tileGameObjectMap.ContainsKey(tile_data) == false)
        {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        if (tile_data.Type == TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == TileType.Empty)
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

        return World.GetTileAt(x, y);
    }

    public void OnFurnitureCreated(Furniture furn)
    {
        //Debug.Log("OnFurnitureCreated");
        // Create a visual GameObject linked to this data.

        // FIXME: Does not consider multi-tile objects nor rotated objects

        // This creates a new GameObject and adds it to our scene.
        GameObject furn_go = new GameObject();
        

        // Add our tile/GO pair to the dictionary.
        furnitureGameObjectMap.Add(furn, furn_go);

        furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
        furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, 0);
        furn_go.transform.SetParent(this.transform, true);

        furn_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

        // Register our callback so that our GameObject gets updated whenever
        // the object's into changes.
        furn.RegisterOnChangedCallback(OnFurnitureChanged);

    }

    void OnFurnitureChanged(Furniture furn)
    {
        //Debug.Log("OnFurnitureChanged");
        // Make sure the furniture's graphics are correct.

        if (furnitureGameObjectMap.ContainsKey(furn) == false)
        {
            Debug.LogError("OnFurnitureChanged -- trying to change visuals for furniture not in our map.");
            return;
        }

        GameObject furn_go = furnitureGameObjectMap[furn];
        //Debug.Log(furn_go);
        //Debug.Log(furn_go.GetComponent<SpriteRenderer>());

        furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
    }




    Sprite GetSpriteForFurniture(Furniture obj)
    {
        if (obj.linksToNeighbour == false)
        {
            return furnitureSprites[obj.objectType];
        }

        // Otherwise, the sprite name is more complicated.

        string spriteName = obj.objectType + "_";

        // Check for neighbours North, East, South, West

        int x = obj.tile.X;
        int y = obj.tile.Y;

        Tile t;

        t = World.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "N";
        }
        t = World.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "E";
        }
        t = World.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "S";
        }
        t = World.GetTileAt(x - 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "W";
        }

        // For example, if this object has all four neighbours of
        // the same type, then the string will look like:
        //       Wall_NESW

        if (furnitureSprites.ContainsKey(spriteName) == false)
        {
            Debug.LogError("GetSpriteForInstalledObject -- No sprites with name: " + spriteName);
            return null;
        }

        return furnitureSprites[spriteName];

    }

}
