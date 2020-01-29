using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController Instance { get; protected set; }
    GameObject moneyObject;
    GameObject electObject;
    GameObject populObject;
    GameObject happyObject;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ResourceController initialized");
        InvokeRepeating("UpdatePlayerMoney", 1.0f, 5.0f);
        InvokeRepeating("UpdateResourceScale", 0f, 0.5f);
        InvokeRepeating("UpdateHappiness", 1.0f, 2.0f);
        this.transform.position = this.gameObject.transform.parent.position;
        moneyObject = this.transform.Find("moneyMeshTemp").gameObject;
        electObject = this.transform.Find("electMeshTemp").gameObject;
        populObject = this.transform.Find("populMeshTemp").gameObject;
        happyObject = this.transform.Find("happyMeshTemp").gameObject;
        moneyObject.transform.position = this.transform.position + new Vector3(-1, 0, 1);
        electObject.transform.position = this.transform.position + new Vector3(-2, 0, 1);
        populObject.transform.position = this.transform.position + new Vector3(-3, 0, 1);
        happyObject.transform.position = this.transform.position + new Vector3(-4, 0, 1);
        //this.GetComponent<MeshRenderer>().
    }

    // Update is called once per frame
    void Update()
    {
        //moneyMesh.ren

    }

    void UpdatePlayerMoney()
    {
        WorldController.Instance.playerstats.Money += (10 * WorldController.Instance.playerstats.entertainmentCount) 
            + (15 * WorldController.Instance.playerstats.industrialCount) 
            + ((int)(WorldController.Instance.CurrentHappinessRatio * 8.0f) * WorldController.Instance.playerstats.residentialCount);
        WorldController.Instance.playerstats.Money -= (15 * WorldController.Instance.playerstats.electricity) + (10 * WorldController.Instance.playerstats.water);
        //Debug.Log("Current money: " + WorldController.Instance.playerstats.Money.ToString());
        
    }

    void UpdateResourceScale()
    {
        float moneyScale = Mathf.Min(((float)WorldController.Instance.playerstats.Money / 1000), 1);
        
        //Debug.Log(moneyScale);
        moneyObject.transform.localScale = new Vector3(moneyScale, moneyScale, moneyScale);
    }

    void UpdateHappiness()
    {
        foreach(var t in WorldController.Instance.allTiles)
        {
            if(t.Type != Tile.TileType.Empty && t.Type != Tile.TileType.Floor && t.Type != Tile.TileType.Road)
            {
                t.refreshHappiness();
                t.printTileStats();
            }
        }
    }

}
