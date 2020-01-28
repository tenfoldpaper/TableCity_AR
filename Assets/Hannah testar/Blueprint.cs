using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Blueprint
{
    public GameObject status;
    public GameObject[] prefab;
    public int cost;
    public int number;

    public GameObject Randomize()
    {
        return prefab[Random.Range(0, number)];
    }
}