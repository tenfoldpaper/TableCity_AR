using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    private void Awake()
    {
        if( instance!= null)
        {
            Debug.Log("More than one BuildManager in scene.");
            return;
        }
        instance = this;
    }

    public GameObject road;
    public GameObject residental;
    public GameObject entertainment;
    public GameObject industry;

    private Blueprint objectToBuild;

    public bool CanBuild { get { return objectToBuild != null; } }

    public void SetObjectToBuild()
    {
        if (CanBuild)
        {
            if(PlayerStats.Money < objectToBuild.cost)
            {
                Debug.Log("Not enough money");
                return;
            }

            PlayerStats.Money -= objectToBuild.cost;

            Vector3 position = new Vector3(MouseController.selected.X + 0.5f, MouseController.selected.Y + 0.5f);

            GameObject go = (GameObject)Instantiate(objectToBuild.prefab, position, Quaternion.identity);

            Debug.Log("Object build! Money left: " + PlayerStats.Money);
        }

    }

    public void SelectObjectToBuild(Blueprint blueprint)
    {
        objectToBuild = blueprint;
        SetObjectToBuild();
    }

    /* Build on Gameobject
    public void BuildObjectOn(Tile t)
    {
        Gameobject object = (GameObject)Instantiate(objectToBuild.prefab, t.transform.position + t.positionOffset, Quaterion.identity)
    }*/

    public void delete()
    {
        Destroy(MouseController.selectedObject);
        MouseController.selectedObject = null;
    }
}