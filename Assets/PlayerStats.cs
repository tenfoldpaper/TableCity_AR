using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int Money { get; set; }
    public int population { get; set; }
    public int income { get; set; }
    public int water { get; set; }
    public int electricity { get; set; }


    private int startMoney = 400;
    public int residentialCount { get; set; }
    public int industrialCount { get; set; }
    public int entertainmentCount { get; set; }
    public int countInitValue = 0;

    public PlayerStats()
    {
        Money = startMoney;
        residentialCount = countInitValue;
        industrialCount = countInitValue;
        entertainmentCount = countInitValue;
        water = countInitValue;
        electricity = countInitValue;
        Debug.Log("Player stats initialized");
    }

}
