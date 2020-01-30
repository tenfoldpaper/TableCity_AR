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
    public List<Tile> allTiles;
    public List<Tile> indusTiles;
    public List<Tile> enterTiles;
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
        powerTiles = new List<Tile>();
        waterTiles = new List<Tile>();
        indusTiles = new List<Tile>();
        enterTiles = new List<Tile>();
        allTiles = new List<Tile>();
        // Create a world with Empty tiles
        world = new World(worldX, worldY);
        playerstats = new PlayerStats();
        Vector3 bc_center = new Vector3(0.5f, 0.5f, 0);

        world.RegisterFurnitureCreated(OnFurnitureCreated);

        // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        Mesh waterMesh = Resources.Load<Mesh>("Status/water");
        Mesh powerMesh = Resources.Load<Mesh>("Status/lightning");
        Mesh happyMesh = Resources.Load<Mesh>("Status/heart");
        Mesh populMesh = Resources.Load<Mesh>("Status/pop");

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



                // Add the status game objects, then disable them 
                tile_data.waterStatus = new GameObject();
                tile_data.waterStatus.transform.parent = tile_go.transform;
                MeshRenderer wmr = tile_data.waterStatus.AddComponent<MeshRenderer>();
                wmr.material = Resources.Load<Material>("Status/Materials/waterMaterial");
                MeshFilter wmf = tile_data.waterStatus.AddComponent<MeshFilter>();
                wmf.mesh = waterMesh;
                Animator wanimator = tile_data.waterStatus.AddComponent<Animator>();
                wanimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Tile_0_0WaterStatus");
                tile_data.waterStatus.transform.position = tile_go.transform.position + (tile_go.transform.rotation * new Vector3(0.33f, 0.33f, -2.91f));
                tile_data.waterStatus.transform.rotation = tile_go.transform.rotation * Quaternion.Euler(180, 0, 270);
                tile_data.waterStatus.transform.localScale = new Vector3(8f, 8f, 9f);
                tile_data.waterStatus.name = (tile_go.name + "WaterStatus");
                tile_data.waterStatus.SetActive(false);

                tile_data.powerStatus = new GameObject();
                tile_data.powerStatus.transform.parent = tile_go.transform;
                MeshRenderer pmr = tile_data.powerStatus.AddComponent<MeshRenderer>();
                pmr.material = Resources.Load<Material>("Status/Materials/powerMaterial");
                MeshFilter pmf = tile_data.powerStatus.AddComponent<MeshFilter>();
                pmf.mesh = powerMesh;
                Animator panimator = tile_data.powerStatus.AddComponent<Animator>();
                panimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Tile_0_8PowerStatus");
                tile_data.powerStatus.transform.position = tile_go.transform.position + (tile_go.transform.rotation * new Vector3(0.66f, 0.33f, -3));
                tile_data.powerStatus.transform.rotation = tile_go.transform.rotation * Quaternion.Euler(180, 0, 270);
                tile_data.powerStatus.transform.localScale = new Vector3(5f, 5f, 5f);
                tile_data.powerStatus.name = (tile_go.name + "PowerStatus");
                tile_data.powerStatus.SetActive(false);

                tile_data.happyStatus = new GameObject();
                tile_data.happyStatus.transform.parent = tile_go.transform;
                MeshRenderer hmr = tile_data.happyStatus.AddComponent<MeshRenderer>();
                hmr.material = Resources.Load<Material>("Status/Materials/happyMaterial");
                MeshFilter hmf = tile_data.happyStatus.AddComponent<MeshFilter>();
                hmf.mesh = happyMesh;
                Animator hanimator = tile_data.happyStatus.AddComponent<Animator>();
                hanimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Tile_0_0HappyStatus");
                hanimator.SetFloat("AnimSpeed", 0.3f);
                tile_data.happyStatus.transform.position = tile_go.transform.position + (tile_go.transform.rotation * new Vector3(0.33f, 0.66f, -3));
                tile_data.happyStatus.transform.rotation = tile_go.transform.rotation * Quaternion.Euler(180, 0, 0);
                tile_data.happyStatus.transform.localScale = new Vector3(5f, 5f, 5f);
                tile_data.happyStatus.name = (tile_go.name + "HappyStatus");
                tile_data.happyStatus.SetActive(false);

                tile_data.populStatus = new GameObject();
                tile_data.populStatus.transform.parent = tile_go.transform;
                MeshRenderer lmr = tile_data.populStatus.AddComponent<MeshRenderer>();
                lmr.material = Resources.Load<Material>("Status/Materials/populMaterial");
                MeshFilter lmf = tile_data.populStatus.AddComponent<MeshFilter>();
                lmf.mesh = populMesh;
                Animator lanimator = tile_data.populStatus.AddComponent<Animator>();
                lanimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Tile_0_0HappyStatus");
                lanimator.SetFloat("AnimSpeed", 0.3f);
                tile_data.populStatus.transform.position = tile_go.transform.position + (tile_go.transform.rotation * new Vector3(0.66f, 0.66f, -3));
                tile_data.populStatus.transform.rotation = tile_go.transform.rotation * Quaternion.Euler(180, 0, 90);
                tile_data.populStatus.transform.localScale = new Vector3(5f, 5f, 5f);
                tile_data.populStatus.name = (tile_go.name + "PopulStatus");
                tile_data.populStatus.SetActive(false);

                tile_data.wrkerStatus = new GameObject();
                tile_data.wrkerStatus.transform.parent = tile_go.transform;
                MeshRenderer kmr = tile_data.wrkerStatus.AddComponent<MeshRenderer>();
                kmr.material = Resources.Load<Material>("Status/Materials/wrkerMaterial");
                MeshFilter kmf = tile_data.wrkerStatus.AddComponent<MeshFilter>();
                kmf.mesh = populMesh;
                Animator kanimator = tile_data.wrkerStatus.AddComponent<Animator>();
                kanimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Tile_0_8PowerStatus");
                tile_data.wrkerStatus.transform.position = tile_go.transform.position + (tile_go.transform.rotation * new Vector3(0.5f, 0.5f, -3));
                tile_data.wrkerStatus.transform.rotation = tile_go.transform.rotation * Quaternion.Euler(180, 0, 0);
                tile_data.wrkerStatus.transform.localScale = new Vector3(5f, 5f, 5f);
                tile_data.wrkerStatus.name = (tile_go.name + "WrkerStatus");
                tile_data.wrkerStatus.SetActive(false);


                tile_bc.center = bc_center;
                tile_bc.size = new Vector3(1, 1, 0.01f);
                tile_go.layer = 8;
                tile_data.gameObject = tile_go;

                // Register our callback so that our GameObject gets updated whenever
                // the tile's type changes.
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
                this.allTiles.Add(tile_data);
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
            tile_data.setTileData(0, 2, 0, 0, 0, 20);
        }
        else if (tile_data.Type == Tile.TileType.Electricity)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = electricitySprite;
            tile_data.setTileData(0, 0, 0, 0, 20, 0);
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
            //Debug.Log(obj.objectType);
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
        //Debug.Log("Updating tile resources");
        int currentResource;
        if (!resourceType)
        {
            currentResource = epicentre.waterResources;
            if (!epicentre.electricity)
            {
                return;
            }
            //Debug.Log("Water");
            
        }
        else
        {
            currentResource = epicentre.electricityResources;
            //Debug.Log("Power");
        }
        
        for(int i = -6; i < 7; i++) // X axis
        {
            for(int j = -6; j < 7; j++) // Y axis
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
                        //Debug.Log("Updated the resources on this tile");
                        //Debug.Log(currentX.ToString() + " " + currentY.ToString());
                    }
                    else if(currentResource < currentTile.needPower)
                    {
                        Debug.Log("Not enough power!");
                        //TODO: Would be nice to render something here as a visual indicator above the building.
                    }
                    else
                    {
                        Debug.Log("This tile doesn't need power");
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
                        //Debug.Log("Updated the resources on this tile");
                        //Debug.Log(currentX.ToString() + " " + currentY.ToString());
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
            //Debug.Log("Remaining water resources: " + currentResource.ToString());
        }
        else
        {
            epicentre.electricityResources = currentResource;
            //Debug.Log("Remaining electricity resources: " + currentResource.ToString());
        }


        //Debug.Log("Tile resource has been updated.");
    }


    public void UpdateTileHappiness(Tile epicentre, int happinessSign)
    {
        // Depending on the building constructed, the happiness of individual tiles are changed.
        // Residential tiles are the only ones that are affected, however.
        // happinessSign is for controlling whether this function is used when constructing or 
        // demolishing a building.

        Tile.TileType happinessType = epicentre.Type;
        int[] happinessArrayP = new int[] { 0, 8, 6, 4, 2 };
        int[] happinessArrayE = new int[] { 0, 5, 5, 5, 4, 4, 3, 3, 2, 2, 1 };
        int[] happinessArrayI = new int[] { 0, 4, 4, 3, 3, 3, 3, 3, 2, 2, 2 };

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
                    currentTile.decreaseHappiness(happinessArrayP[ManhattanDistance(currentTile, epicentre)]);
                    //currentTile.happiness -= happinessSign * happinessArrayP[ManhattanDistance(currentTile, epicentre)];
                }
                else if (happinessType == Tile.TileType.Entertainment)
                {
                    currentTile.increaseHappiness(happinessArrayE[ManhattanDistance(currentTile, epicentre)]);
                    //currentTile.happiness += happinessSign * happinessArrayE[ManhattanDistance(currentTile, epicentre)];
                }
                else if (happinessType == Tile.TileType.Industrial)
                {
                    currentTile.increaseHappiness(happinessArrayI[ManhattanDistance(currentTile, epicentre)]);
                    //currentTile.happiness += happinessSign * happinessArrayI[ManhattanDistance(currentTile, epicentre)];
                }
            }
        }

        CalculateTotalHappinessRatio();

    }

    public int ManhattanDistance(Tile a, Tile b)
    {
        return System.Math.Abs(b.X - a.X) + System.Math.Abs(b.Y - a.Y);
    }

    public void CalculateTotalHappinessRatio()
    {
        int totalHappiness = 0;
        int residentialCount = 0;
        foreach(var t in allTiles){
            if(t.Type == Tile.TileType.Residential)
            {
                totalHappiness += t.happiness;
                residentialCount += 1;
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

