using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ElectricityScript : MonoBehaviour
{
    public int resources = 100;
    public float dist = 5f;

    void Start()
    {
        UpdateElectricity();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateElectricity()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
        buildings = buildings.OrderBy(point => Vector3.Distance(transform.position, point.transform.position)).ToArray();

        foreach (GameObject building in buildings)
        {
            float distanceToBuilding = Vector3.Distance(transform.position, building.transform.position);
            if (resources > 0 && distanceToBuilding <= dist)
            {
                while (resources > 0)
                {
                    if (building.gameObject.GetComponent<BuildingScript>().ElectricityFull())
                        break;

                    building.gameObject.GetComponent<BuildingScript>().IncreaseElectricity();
                    DecreaseAvailibleElectricity();
                }
            }
        }
    }

    void DecreaseAvailibleElectricity()
    {
        resources--;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dist);
    }
}
