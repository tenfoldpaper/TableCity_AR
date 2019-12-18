using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingScript : MonoBehaviour
{
    public Tile t;

    public int maximumCapacity = 10;
    public int currentElectricity;
    public int maximimInhabitants = 20;

    public int currentWorkers = 0;
    public int currentInhabitants;


    void Start()
    {
        InvokeRepeating("UpdateInhabitants", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateInhabitants()
    {
        //Calculate inhabitants
        currentInhabitants = maximimInhabitants;
    }

    public bool ElectricityFull()
    {
        return maximumCapacity == currentElectricity;
    }

    public void IncreaseElectricity()
    {
        currentElectricity++;
    }
}
