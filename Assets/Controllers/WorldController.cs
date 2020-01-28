using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public class WorldController : MonoBehaviour
{

    public static WorldController Instance { get; protected set; }

    public Sprite floorSprite;
    public Sprite roadSprite;
    public Sprite residentialSprite;
    public Sprite industrialSprite;
    public Sprite entertainmentSprite;
    public Sprite waterSprite;
    public Sprite electricitySprite;

    public List<Tile> powerTiles;
    public List<Tile> waterTiles;

    Dictionary<Furniture, GameObject> furnitureGameObjectMap;
    Dictionary<string, Sprite> furnitureSprites;
    //List<Building> buildings;
    

    // The world and tile data
    public World world { get; protected set; }
    public int worldX = 20;
    public int worldY = 20;
    private int MaxHappiness = 20;
    public float CurrentHappinessRatio { get; protected set; }
    public PlayerStats playerstats { get; protected set; }

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
        world = new World(worldX, worldY);
        playerstats = new PlayerStats();
        Vector3 bc_center = new Vector3(0.5f, 0.5f, 0);

        world.RegisterFurnitureCreated(OnFurnitureCreated);

        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

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
                tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
                BoxCollider tile_bc = tile_go.AddComponent<BoxCollider>();
                tile_bc.center = bc_center;
                tile_bc.size = new Vector3(1, 1, 0.01f);
                tile_go.layer = 8;
                tile_data.gameObject = tile_go;

                // Register our callback so that our GameObject gets updated whenever
                // the tile's type changes.
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

    }
    public GameObject WrapInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if(parent != null)
            return Instantiate(prefab, position, rotation, parent); 
        else
            return Instantiate(prefab, position, rotation, this.transform);
    }
    void LoadSprites()
    {
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture");

        
        //Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites)
        {
            //Debug.Log(s);
            furnitureSprites[s.name] = s;
        }
        
    }
    //Creates an L set inbetween start and endpoint
    //TODO apply a function to each tile instead
    public List<Tile> GetLPathSet(int x_start, int y_start, int x_end, int y_end)
    {
        List<Tile> set = new List<Tile>();
        int x = x_start;
        int y = y_start;
        int x_sign = x_end > x_start? 1: -1;
		int y_sign = y_end > y_start? 1: -1;
		if (Mathf.Abs(x_start - x_end) > Mathf.Abs(y_start - y_end))
        {
            for (; x != x_end; x+= x_sign)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
            for (; y != y_end; y+= y_sign)
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
            for (; y != y_end; y+= y_sign)
            {
                Tile t = WorldController.Instance.world.GetTileAt(x, y);
                if (t != null)
                {
                    set.Add(t);
                }
            }
            for (; x != x_end; x+= x_sign)
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
            //t.Type = Tile.TileType.Road;
            if(playerstats.Money >= 10)
            {
                WorldController.Instance.world.PlaceFurniture("Road", t);
                t.Type = Tile.TileType.Road;
                //Debug.Log("Road created at " + t.X + " " + t.Y);
                playerstats.Money -= 10;
            }
            else
            {
                Debug.Log("Not enough money!");
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This function should be called automatically whenever a tile's type gets changed.
    void OnTileTypeChanged(Tile tile_data)
    {
        //Debug.Log("Changing tile type");
		GameObject tile_go = tile_data.gameObject;

        if (tile_go == null)
        {
            Debug.LogError("tileGameObject is null");
            return;
        }

        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
            tile_data.maxPopulation = 0;
            
        }
        else if (tile_data.Type == Tile.TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else if (tile_data.Type == Tile.TileType.Road)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = roadSprite;
            tile_data.setTileData(0, 0, 0, 0, 0, 0);
        }
        else if (tile_data.Type == Tile.TileType.Residential)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = residentialSprite;
            tile_data.setTileData(100, 1, 1, 0, 0, 0);
        }
        else if (tile_data.Type == Tile.TileType.Industrial)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = industrialSprite;

            tile_data.setTileData(0, 3, 2, 0, 0, 0);
        }
        else if (tile_data.Type == Tile.TileType.Entertainment)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = entertainmentSprite;
            tile_data.setTileData(0, 2, 1, 0, 0, 0);
        }
        else if (tile_data.Type == Tile.TileType.Water)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
            tile_data.setTileData(0, 2, 0, 0, 0, 50);
        }
        else if (tile_data.Type == Tile.TileType.Electricity)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = electricitySprite;
            tile_data.setTileData(0, 0, 0, 0, 50, 0);
        }
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return world.GetTileAt(x, y);
    }

    public void OnFurnitureCreated(Furniture furn)
    {
     

        GameObject furn_go = new GameObject();
        //furn_go.AddComponent<SpriteRenderer>().sortingOrder = 1;
        

        // Add our tile/GO pair to the dictionary.
        furnitureGameObjectMap.Add(furn, furn_go);


        furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
        //furn_go.transform.position = new Vector3(furn.tile.X, 0.01f, -furn.tile.Y);

        //furn_go.transform.position = new Vector3(furn.tile.X, furn.tile.Y, -0.01f);
        //furn_go.transform.rotation = Quaternion.Euler(0, 0, 0);

        //furn_go.transform.SetParent(this.transform, true);
        furn_go.transform.parent = furn.tile.gameObject.transform.parent;
        furn_go.transform.position = furn.tile.gameObject.transform.position - new Vector3(0.001f, 0.001f, 0.001f);

        furn_go.transform.rotation = furn.tile.gameObject.transform.rotation;

        if (furn.objectType == "Road") { 
            furn_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);

            // Register our callback so that our GameObject gets updated whenever
            // the object's into changes.
            furn.RegisterOnChangedCallback(OnFurnitureChanged);
        }
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
            Debug.Log(obj.objectType);
            return furnitureSprites[obj.objectType];
        }

        // Otherwise, the sprite name is more complicated.

        string spriteName = obj.objectType + "_";

        // Check for neighbours North, East, South, West

        int x = obj.tile.X;
        int y = obj.tile.Y;

        Tile t;

        t = world.GetTileAt(x, y + 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "N";
        }
        t = world.GetTileAt(x + 1, y);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "E";
        }
        t = world.GetTileAt(x, y - 1);
        if (t != null && t.furniture != null && t.furniture.objectType == obj.objectType)
        {
            spriteName += "S";
        }
        t = world.GetTileAt(x - 1, y);
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

    public void UpdateTileResources(Tile epicentre, bool resourceType, bool resourceBool)
    {
        // From the epicentre, turn on the boolean value of the respective resource of all tiles within
        // 5 Manhattan distance radius.

        // resourceType: 0 = water, 1 = electricity
        Debug.Log("Updating tile resources");
        int currentResource;
        if (!resourceType)
        {
            currentResource = epicentre.waterResources;
            Debug.Log("Water");
            
        }
        else
        {
            currentResource = epicentre.electricityResources;
            Debug.Log("Power");
        }
        
        for(int i = -5; i < 6; i++) // X axis
        {
            for(int j = -5; j < 6; j++) // Y axis
            {
                int currentX = epicentre.X + i;
                int currentY = epicentre.Y + j;
                // Don't accidentally get tiles that are out of range!
                if(currentX > worldX - 1)
                {
                    continue;
                }
                if(currentY > worldY - 1)
                {
                    continue;
                }
                if(currentX < 0)
                {
                    continue;
                }
                if(currentY < 0)
                {
                    continue;
                }

                Tile currentTile = world.GetTileAt(currentX, currentY);

                if (resourceType && currentTile.requiresPower())
                {
                    if (currentResource >= currentTile.needPower && !currentTile.electricity)
                    {
                        currentTile.electricity = resourceBool;
                        //currentTile.hasPower = currentTile.needPower;
                        currentTile.increaseHappiness(5);
                        currentResource -= currentTile.needPower;
                        Debug.Log("Updated the resources on this tile");
                        Debug.Log(currentX.ToString() + " " + currentY.ToString());
                    }
                    else if(currentResource < currentTile.needPower)
                    {
                        Debug.Log("Not enough power!");
                        //TODO: Would be nice to render something here as a visual indicator above the building.
                    }
                    else
                    {
                        Debug.Log("This tile doesn't need water");
                    }
                }
                else if (currentTile.requiresWater())
                {
                    if (currentResource >= currentTile.needWater && !currentTile.water)
                    {
                        currentTile.water = resourceBool;
                        //currentTile.hasWater = currentTile.needWater;
                        currentTile.increaseHappiness(5);
                        currentResource -= currentTile.needWater;
                        Debug.Log("Updated the resources on this tile");
                        Debug.Log(currentX.ToString() + " " + currentY.ToString());
                    }
                    else if (currentResource < currentTile.needWater)
                    {
                        Debug.Log("Not enough water!");
                        //TODO: Render a visual indicator for the lack
                    }
                    else
                    {
                        Debug.Log("This tile doesn't need water");
                    }
                }
                else
                {
                    //Debug.Log("This tile doesn't require water or power");
                    //Debug.Log(currentX.ToString() + " " + currentY.ToString());
                }
            }
        }
    
        if (!resourceType)
        {
            epicentre.waterResources = currentResource;
            Debug.Log("Remaining water resources: " + currentResource.ToString());
        }
        else
        {
            epicentre.electricityResources = currentResource;
            Debug.Log("Remaining electricity resources: " + currentResource.ToString());
        }

        CalculateTotalHappinessRatio();

        Debug.Log("Tile resource has been updated.");
    }


    public void UpdateTileHappiness(Tile epicentre, int happinessSign)
    {
        // Depending on the building constructed, the happiness of individual tiles are changed.
        // Residential tiles are the only ones that are affected, however.
        // happinessSign is for controlling whether this function is used when constructing or 
        // demolishing a building.

        Tile.TileType happinessType = epicentre.Type;
        int[] happinessArrayP = new int[] { 0, 10, 8, 6, 4 };
        int[] happinessArrayE = new int[] { 0, 15, 12, 12, 10, 10, 10, 8, 8, 6, 6 };
        int[] happinessArrayI = new int[] { 0, 10, 10, 10, 10, 8, 8, 8, 8, 6, 6 };

        int affectedX = 0;
        int affectedY = 0;

        if(happinessType == Tile.TileType.Residential) // If a residential building is built, then it doesn't affect surrounding happiness.
        {
            CalculateTotalHappinessRatio();
            return;
        }
        if(happinessType == Tile.TileType.Electricity)
        {
            affectedX = 2;
            affectedY = 2;

        }
        else if(happinessType == Tile.TileType.Entertainment || happinessType == Tile.TileType.Industrial)
        {
            affectedX = 5;
            affectedY = 5;
        }

        for(int i = -affectedX; i < affectedX+1; i++)
        {
            for(int j = -affectedY; j < affectedY+1; j++)
            {
                int currentX = epicentre.X + i;
                int currentY = epicentre.Y + j;
                if (currentX > worldX - 1)
                {
                    continue;
                }
                if (currentY > worldY - 1)
                {
                    continue;
                }
                if (currentX < 0)
                {
                    continue;
                }
                if (currentY < 0)
                {
                    continue;
                }
                Tile currentTile = world.GetTileAt(currentX, currentY);
                
                if (happinessType == Tile.TileType.Electricity)
                {
                    currentTile.happiness -= happinessSign * happinessArrayP[ManhattanDistance(currentTile, epicentre)];
                }
                else if (happinessType == Tile.TileType.Entertainment)
                {
                    currentTile.happiness += happinessSign * happinessArrayE[ManhattanDistance(currentTile, epicentre)];
                }
                else if (happinessType == Tile.TileType.Industrial)
                {
                    currentTile.happiness += happinessSign * happinessArrayI[ManhattanDistance(currentTile, epicentre)];
                }
            }
        }

        CalculateTotalHappinessRatio();

        Debug.Log("Tile happiness has been updated.");

    }

    public int ManhattanDistance(Tile a, Tile b)
    {
        return System.Math.Abs(b.X - a.X) + System.Math.Abs(b.Y - a.Y);
    }

    public void CalculateTotalHappinessRatio()
    {
        int totalHappiness = 0;
        int residentialCount = 0;
        for(int i = 0; i < worldX; i++)
        {
            for(int j = 0; j < worldY; j++)
            {
                Tile currentTile = world.GetTileAt(i, j);
                if(currentTile.Type == Tile.TileType.Residential)
                {
                    int currentHappiness = currentTile.happiness;
                    if(currentHappiness > MaxHappiness)
                    {
                        currentHappiness = MaxHappiness;
                    }

                    totalHappiness += currentHappiness;
                    residentialCount += 1;
                }
            }
        }
        if(residentialCount == 0)
        {
            CurrentHappinessRatio = 0;
        }
        else
        {
            CurrentHappinessRatio = (totalHappiness / ((float)residentialCount * 20));
        }
        Debug.Log("Current HRatio: " + CurrentHappinessRatio.ToString());
    }


}

