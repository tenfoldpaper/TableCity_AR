using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController Instance { get; protected set; }
    GameObject moneyObject;
    GameObject moneyText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ResourceController initialized");
        InvokeRepeating("UpdatePlayerMoney", 1.0f, 5.0f);
        InvokeRepeating("UpdateScale", 0f, 0.5f);
        InvokeRepeating("UpdateHappiness", 1.0f, 2.0f);
        InvokeRepeating("UpdateStatus", 1.0f, 0.5f);
        InvokeRepeating("UpdatePopulation", 0f, 3.0f);
        this.transform.position = this.gameObject.transform.parent.position;
        moneyObject = this.transform.Find("moneyMeshTemp").gameObject;
        moneyText = moneyObject.transform.Find("moneyText").gameObject;
        moneyText = this.transform.Find("moneyText").gameObject;
        moneyObject.transform.localPosition = this.transform.position + new Vector3(-WorldController.Instance.worldX/2, -WorldController.Instance.worldY/2, -10);
        Debug.Log(moneyObject.gameObject.transform.position);
        moneyText.transform.rotation = this.transform.rotation;
        moneyText.transform.localPosition = moneyObject.transform.localPosition + (this.transform.rotation * new Vector3(0, 0, -2));
        moneyText.GetComponent<TextMesh>().text = 0.ToString();
        //this.GetComponent<MeshRenderer>().
    }

    // Update is called once per frame
    void Update()
    {
        //moneyMesh.ren

    }

    void UpdatePlayerMoney()
    {
        UpdateOnBuildingCount();
        int newMoney = (5 * WorldController.Instance.playerstats.entertainmentOnCount) + (10 * WorldController.Instance.playerstats.industrialOnCount) 
            + ((int)(WorldController.Instance.CurrentHappinessRatio * 5.0f) * WorldController.Instance.playerstats.residentialOnCount);
        newMoney -= (15 * WorldController.Instance.playerstats.electricity) + (10 * WorldController.Instance.playerstats.water);

        WorldController.Instance.playerstats.Money += newMoney;
        Debug.Log("current income: " + newMoney);

    }

    void UpdateScale()
    {
        UpdateResourceScale();
        UpdateHappinessScale();
        UpdatePopulationScale();
    }
    void UpdateStatus()
    {
        UpdateWaterStatus();
        UpdatePowerStatus();
        UpdateIndustrialStatus();
        UpdateEntertainmentStatus();
    }
    void UpdateHappiness()
    {
        foreach(var t in WorldController.Instance.allTiles)
        {
            if(t.Type != Tile.TileType.Empty && t.Type != Tile.TileType.Floor && t.Type != Tile.TileType.Road)
            {
                t.refreshHappiness();
                //t.printTileStats();
            }
        }
    }
    void UpdateResourceScale()
    {
        float moneyScale = Mathf.Min(((float)WorldController.Instance.playerstats.Money / 1000) * 4, 4);

        //Debug.Log(moneyScale);
        moneyObject.transform.localScale = new Vector3(moneyScale, moneyScale, moneyScale);
        moneyText.GetComponent<TextMesh>().text = WorldController.Instance.playerstats.Money.ToString();
    }

    void UpdateHappinessScale()
    {
        foreach(var t in WorldController.Instance.allTiles)
        {
            if(t.Type == Tile.TileType.Residential)
            {
                t.happyStatus.SetActive(true);
                if(t.happiness == 0)
                {
                    t.happyStatus.transform.localScale = new Vector3(1, 1, 1);

                }
                else
                {
                    float happyScale = ((float)t.happiness / 20) * 8;
                    t.happyStatus.transform.localScale = new Vector3(happyScale, happyScale, happyScale);
                }
            }
            else
            {
                //t.happyStatus.SetActive(false);
            }
        }
    }

    void UpdatePopulationScale()
    {
        foreach(var t in WorldController.Instance.allTiles)
        {
            if(t.Type == Tile.TileType.Residential)
            {
                t.populStatus.SetActive(true);
                if(t.population == 0)
                {
                    t.populStatus.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    float populScale = Mathf.Max(((float)t.population / t.maxPopulation) * 8, 1f);
                    t.populStatus.transform.localScale = new Vector3(populScale, populScale, populScale);
                }
            }
            else {  }
        }

    }

    void UpdateWaterStatus()
    {
        foreach(var t in WorldController.Instance.allTiles)
        {
            if(t.Type == Tile.TileType.Residential || t.Type == Tile.TileType.Entertainment || t.Type == Tile.TileType.Industrial)
            {
                if (!t.water) //if water is not toggled, display water status
                {
                    t.waterStatus.SetActive(true);
                }
                else
                {
                    t.waterStatus.SetActive(false);
                }
            }
        }
    }

    void UpdatePowerStatus()
    {
        foreach (var t in WorldController.Instance.allTiles)
        {
            if (t.Type == Tile.TileType.Residential || t.Type == Tile.TileType.Entertainment || t.Type == Tile.TileType.Industrial || t.Type == Tile.TileType.Water)
            {
                if (!t.electricity) //if water is not toggled, display water status
                {
                    t.powerStatus.SetActive(true);
                }
                else
                {
                    t.powerStatus.SetActive(false);
                }
            }
        }
    }

    void UpdateIndustrialStatus()
    {
        foreach (var epicentre in WorldController.Instance.indusTiles)
        {
            int populReq = 20;
            if (epicentre.popIndEnt < populReq)
            {
                epicentre.wrkerStatus.SetActive(true);
            }
            for (int i = -8; i < 9; i++) // X axis
            {
                for (int j = -8; j < 9; j++) // Y axis
                {
                    int currentX = epicentre.X + i;
                    int currentY = epicentre.Y + j;
                    // Don't accidentally get tiles that are out of range!
                    if (currentX > WorldController.Instance.worldX - 1)
                    {
                        continue;
                    }
                    if (currentY > WorldController.Instance.worldY - 1)
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
                    Tile currentTile = WorldController.Instance.world.GetTileAt(currentX, currentY);
                    if (currentTile.Type == Tile.TileType.Residential)
                    {
                        
                        while(epicentre.popIndEnt < populReq && currentTile.popCapacityInd > 0)
                        {
                            epicentre.popIndEnt += 1;
                            currentTile.popCapacityInd -= 1;
                            Debug.Log("Current popIndEnt: " + epicentre.popIndEnt);
                        }
                        if(epicentre.popIndEnt == populReq)
                        {
                            epicentre.wrkerStatus.SetActive(false);
                            epicentre.industrial = true;
                        }
                        
                    }

                }
            }
        }
    }

    void UpdateEntertainmentStatus()
    {
        foreach (var epicentre in WorldController.Instance.enterTiles)
        {
            int populReq = 10;

            if (epicentre.popIndEnt < populReq)
            {
                epicentre.wrkerStatus.SetActive(true);
            }
            for (int i = -8; i < 9; i++) // X axis
            {
                for (int j = -8; j < 9; j++) // Y axis
                {
                    int currentX = epicentre.X + i;
                    int currentY = epicentre.Y + j;
                    // Don't accidentally get tiles that are out of range!
                    if (currentX > WorldController.Instance.worldX - 1)
                    {
                        continue;
                    }
                    if (currentY > WorldController.Instance.worldY - 1)
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
                    Tile currentTile = WorldController.Instance.world.GetTileAt(currentX, currentY);
                    if (currentTile.Type == Tile.TileType.Residential)
                    {
                        while (epicentre.popIndEnt < populReq && currentTile.popCapacityEnt > 0)
                        {
                            epicentre.popIndEnt += 1;
                            currentTile.popCapacityEnt -= 1;
                            Debug.Log("Current popIndEnt: " + epicentre.popIndEnt);
                        }
                        if (epicentre.popIndEnt == populReq)
                        {
                            epicentre.wrkerStatus.SetActive(false);
                            epicentre.entertainment = true;
                        }

                    }

                }
            }
        }
    }

    void UpdateOnBuildingCount()
    {
        int resCount = 0;
        int entCount = 0;
        int indCount = 0;
        foreach (var t in WorldController.Instance.allTiles)
        {
            if(t.Type == Tile.TileType.Residential)
            {
                if(t.electricity && t.water)
                {
                    resCount += 1;
                }
            }
            if(t.Type == Tile.TileType.Entertainment)
            {
                Debug.Log("bEnt:" + t.entertainment);
                if(t.electricity && t.water && t.entertainment)
                {
                    entCount += 1;
                }
            }
            if (t.Type == Tile.TileType.Industrial)
            {
                if (t.electricity && t.water && t.industrial)
                {
                    indCount += 1;
                }
            }
        }
        WorldController.Instance.playerstats.residentialOnCount = resCount;
        WorldController.Instance.playerstats.entertainmentOnCount = entCount;
        WorldController.Instance.playerstats.industrialOnCount= indCount;
        Debug.Log("R: " + resCount + " E: " + entCount + " I: " + indCount);
    }

    void UpdatePopulation()
    {
        foreach (var t in WorldController.Instance.allTiles)
        {
            if(t.Type == Tile.TileType.Residential)
            {

                int newPop = (int)(((float)t.happiness / 20) * 4f);
                if(t.population + newPop <= t.maxPopulation)
                {
                    t.population += newPop;
                    t.popCapacityInd += newPop;
                    t.popCapacityEnt += newPop;
                }
                else if(t.population == t.maxPopulation)
                {

                }
                else
                {
                    t.popCapacityInd += t.population + newPop - t.maxPopulation;
                    t.popCapacityEnt += t.population + newPop - t.maxPopulation;
                    t.population = t.maxPopulation;
                    
                }
                Debug.Log("New pop: " + newPop);
            }
        }
    }



}
