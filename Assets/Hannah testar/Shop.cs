using UnityEngine;

public class Shop : MonoBehaviour
{
    public Blueprint road;
    public Blueprint entertainment;
    public Blueprint residental;
    public Blueprint industry;

    BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }

    public void PurchaseRoad()
    {
        Debug.Log("Road purchased");
        buildManager.SelectObjectToBuild(road);
    }

    public void SelectEntertainment()
    {
        Debug.Log("Entertainment Purchased");
        buildManager.SelectObjectToBuild(entertainment);
    }

    public void SelectResidental()
    {
        Debug.Log("Residental Purchased" + buildManager.residental);
        buildManager.SelectObjectToBuild(residental);
    }

    public void SelectIndustry()
    {
        Debug.Log("Industry Purchased");
        buildManager.SelectObjectToBuild(industry);
    }

    public void SoldObject()
    {
        Debug.Log("Object deleted");
        buildManager.delete();
    }
}
