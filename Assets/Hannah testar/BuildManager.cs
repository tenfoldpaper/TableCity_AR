using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            entertainment = new Blueprint();
            entertainment.cost = 0;
            entertainment.number = 4;
            entertainment.prefab = new GameObject[4];
            entertainment.prefab[0] = Resources.Load("Buildings/a1") as GameObject;
            entertainment.prefab[1] = Resources.Load("Buildings/a2") as GameObject;
            entertainment.prefab[2] = Resources.Load("Buildings/a3") as GameObject;
            entertainment.prefab[3] = Resources.Load("Buildings/a4") as GameObject;

        }
        if (residental == null)
        {
            residental = new Blueprint();
            GameObject happiness = Resources.Load("Status/happiness") as GameObject;
            residental.status = happiness;
            residental.cost = 0;
            residental.number = 3;
            residental.prefab = new GameObject[3];
            residental.prefab[0] = Resources.Load("Buildings/b1") as GameObject;
            residental.prefab[1] = Resources.Load("Buildings/b2") as GameObject;
            residental.prefab[2] = Resources.Load("Buildings/b3") as GameObject;
        }

        if (industry == null)
        {
            industry = new Blueprint();
            industry.cost = 0;
            industry.number = 2;
            industry.prefab = new GameObject[3];
            industry.prefab[0] = Resources.Load("Buildings/c1") as GameObject;
            industry.prefab[1] = Resources.Load("Buildings/c2") as GameObject;
        }

        if (watertower == null)
        {
            watertower = new Blueprint();
            watertower.cost = 0;
            watertower.number = 1;
            watertower.prefab = new GameObject[1];
            watertower.prefab[0] = Resources.Load("Buildings/watertower") as GameObject;
        }
        if (powerplant == null)
        {
            powerplant = new Blueprint();
            powerplant.cost = 0;
            powerplant.number = 1;
            powerplant.prefab = new GameObject[1];
            powerplant.prefab[0] = Resources.Load("Buildings/powerplant") as GameObject;
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
            Debug.Log("Current residential count: " + WorldController.Instance.playerstats.residentialCount.ToString() + " Happiness: " + parentTile.happiness.ToString() );
            WorldController.Instance.world.PlaceFurniture("Building", parentTile);
            GameObject happyObject = (GameObject)WorldController.Instance.WrapInstantiate(objectToBuild.status, parentTile.gameObject.transform.position, parentTile.gameObject.transform.rotation);
            happyObject.transform.position = happyObject.transform.rotation * new Vector3(0, 1, 0);
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
            WorldController.Instance.world.PlaceFurniture("Resource", parentTile);
            parentTile.Type = Tile.TileType.Water;
            WorldController.Instance.UpdateTileResources(parentTile, false, true);
            WorldController.Instance.waterTiles.Add(parentTile);
            Debug.Log("Current water tower count: " + WorldController.Instance.playerstats.water);
        }
        if (objectToBuild == powerplant)
        {
            WorldController.Instance.playerstats.electricity += 1;
            WorldController.Instance.world.PlaceFurniture("Resource", parentTile);
            parentTile.Type = Tile.TileType.Electricity;
            WorldController.Instance.UpdateTileResources(parentTile, true, true);
            WorldController.Instance.powerTiles.Add(parentTile);
            Debug.Log("Current power plant count: " + WorldController.Instance.playerstats.electricity);
        }
        WorldController.Instance.UpdateTileHappiness(parentTile, 1);
        Debug.Log("Object built! Money left: " + WorldController.Instance.playerstats.Money.ToString());
        if (WorldController.Instance.waterTiles.Count > 0)
        {
            foreach (var resourceTile in WorldController.Instance.waterTiles)
            {
                WorldController.Instance.UpdateTileResources(resourceTile, false, true);
                Debug.Log("Looping through water");
            }
        }
        if(WorldController.Instance.powerTiles.Count > 0)
        {
            foreach (var resourceTile in WorldController.Instance.powerTiles)
            {
                WorldController.Instance.UpdateTileResources(resourceTile, true, true);
                Debug.Log("Looping through power");
            } 
        }
        GameObject go = (GameObject)WorldController.Instance.WrapInstantiate(objectToBuild.Randomize(), parentTile.gameObject.transform.position, parentTile.gameObject.transform.rotation);
        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
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
