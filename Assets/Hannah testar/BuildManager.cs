using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{
    private static BuildManager instance;
    public static BuildManager Instance { 
        get {
            if(instance == null)
            {
                instance = new BuildManager();
            }
            return instance;
        }
         protected set => instance = value; 
    }

    public Blueprint residental;
    public Blueprint entertainment;
    public Blueprint industry;
    public Blueprint watertower;
    public Blueprint powerplant;
    public BuildManager()
    {
        Debug.Log("Initializing BuildManager");
        
        if (entertainment == null)
        {
            GameObject a1 = Resources.Load("Buildings/a1") as GameObject;
            entertainment = new Blueprint();
            entertainment.cost = 0;
            entertainment.prefab = a1;
        }
        if (residental == null)
        {
            GameObject b1 = Resources.Load("Buildings/b1") as GameObject;
            residental = new Blueprint();
            residental.cost = 0;
            residental.prefab = b1;
        }
        
        if (industry == null)
        {
            GameObject c1 = Resources.Load("Buildings/c1") as GameObject;
            industry = new Blueprint();
            industry.cost = 0;
            industry.prefab = c1;
        }

        if (watertower == null)
        {
            GameObject w1 = Resources.Load("Buildings/TestBuilding") as GameObject;
            watertower = new Blueprint();
            watertower.cost = 0;
            watertower.prefab = w1;
        }
        if (powerplant == null)
        {
            GameObject p1 = Resources.Load("Buildings/powerplant") as GameObject;
            powerplant = new Blueprint();
            powerplant.cost = 0;
            powerplant.prefab = p1;
        }
    }


    public void SetObjectToBuild(Blueprint objectToBuild, Tile parentTile)
    {
        if(WorldController.Instance.playerstats.Money < objectToBuild.cost)
        {
            Debug.Log("Not enough money");
            return;
        }
        if (parentTile.furniture != null)
        {
            Debug.Log("Already occupied");
            return;
        }

        if (!parentTile.AdjacencyCheck())
        {
            Debug.Log("Adjacency check failed.");
            return;
        }

        WorldController.Instance.playerstats.Money -= objectToBuild.cost;

        Vector3 position = ConvertTileToWorldspace(parentTile);
        if(objectToBuild == entertainment)
        {
            WorldController.Instance.playerstats.entertainmentCount += 1;
            Debug.Log("Current entertainment count: " + WorldController.Instance.playerstats.entertainmentCount);
            position.y += 1f;
            WorldController.Instance.world.PlaceFurniture("Building", parentTile);
            parentTile.Type = Tile.TileType.Entertainment;
        }
        if (objectToBuild == residental)
        {
            WorldController.Instance.playerstats.residentialCount += 1;
            Debug.Log("Current residential count: " + WorldController.Instance.playerstats.residentialCount);
            WorldController.Instance.world.PlaceFurniture("Building", parentTile);
            parentTile.Type = Tile.TileType.Residential;
        }
        if (objectToBuild == industry)
        {
            WorldController.Instance.playerstats.industrialCount += 1;
            Debug.Log("Current industry count: " + WorldController.Instance.playerstats.industrialCount);
            WorldController.Instance.world.PlaceFurniture("Building", parentTile);
            parentTile.Type = Tile.TileType.Industrial;
        }

        if (objectToBuild == watertower)
        {
            WorldController.Instance.playerstats.water += 1;
            Debug.Log("Current water tower count: " + WorldController.Instance.playerstats.water);
            WorldController.Instance.world.PlaceFurniture("Resource", parentTile);
            WorldController.Instance.UpdateTileResources(parentTile, false, true);
            parentTile.Type = Tile.TileType.Water;
        }
        if (objectToBuild == powerplant)
        {
            WorldController.Instance.playerstats.electricity += 1;
            Debug.Log("Current power plant count: " + WorldController.Instance.playerstats.electricity);
            WorldController.Instance.world.PlaceFurniture("Resource", parentTile);
            WorldController.Instance.UpdateTileResources(parentTile, true, true);
            parentTile.Type = Tile.TileType.Electricity;
        }
        Debug.Log("Check?");
        WorldController.Instance.UpdateTileHappiness(parentTile, 1);
        Debug.Log("Object built! Money left: " + WorldController.Instance.playerstats.Money.ToString());
        GameObject go = (GameObject)WorldController.Instance.WrapInstantiate(objectToBuild.prefab, position, Quaternion.Euler(270, 0,0));
        objectToBuild = null;
    }

    public Vector3 ConvertTileToWorldspace(Tile tile)
    {
        return new Vector3(tile.X + 0.5f, 0, -(tile.Y + 0.5f));
    }
    /* Build on Gameobject
    public void BuildObjectOn(Tile t)
    {
        Gameobject object = (GameObject)Instantiate(objectToBuild.prefab, t.transform.position + t.positionOffset, Quaterion.identity)
    }*/
    /*
    public void delete()
    {
        Destroy(MouseController.selectedObject);
        MouseController.selectedObject = null;
    }
    */
}