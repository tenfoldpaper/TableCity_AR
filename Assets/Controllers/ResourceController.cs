using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController Instance { get; protected set; }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ResourceController initialized");
        InvokeRepeating("UpdatePlayerMoney", 1.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdatePlayerMoney()
    {
        WorldController.Instance.playerstats.Money += (10 * WorldController.Instance.playerstats.entertainmentCount) 
            + (15 * WorldController.Instance.playerstats.industrialCount) 
            + ((int)(WorldController.Instance.CurrentHappinessRatio * 8.0f) * WorldController.Instance.playerstats.residentialCount);
        WorldController.Instance.playerstats.Money -= (15 * WorldController.Instance.playerstats.electricity) + (10 * WorldController.Instance.playerstats.water);
        Debug.Log("Current money: " + WorldController.Instance.playerstats.Money.ToString());
    }

}
