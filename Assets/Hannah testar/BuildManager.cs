using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{
    private static BuildManager instance;
    public static BuildManager Instance
    {
        get
        {
            if (instance == null)
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
    public BuildManager()
    {
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
    }
    public void SetObjectToBuild(Blueprint objectToBuild, Tile parentTile)
    {
        if (PlayerStats.Money < objectToBuild.cost)
        {
            Debug.Log("Not enough money");
            return;
        }
        if (parentTile.objects != null)
        {
            Debug.Log("Already occupied");
            return;
        }

        PlayerStats.Money -= objectToBuild.cost;

        Vector3 position = new Vector3(parentTile.X + 0.5f, parentTile.Y + 0.5f, 0);
        if (objectToBuild == entertainment)
        {
            position = new Vector3(parentTile.X + 0.5f, parentTile.Y + 0.5f, -1);
        }
        WorldController.Instance.world.PlaceObject("Building", parentTile);

        GameObject go = (GameObject)WorldController.Instance.WrapInstantiate(objectToBuild.prefab, position, Quaternion.Euler(180, 0, 0));

        Debug.Log("Object build! Money left: " + PlayerStats.Money);
        objectToBuild = null;
    }
    /* Build on Gameobject
    public void BuildObjectOn(Tile t)
    {
        Gameobject object = (GameObject)Instantiate(objectToBuild.prefab, t.transform.position + t.positionOffset, Quaterion.identity)
    }*/
}