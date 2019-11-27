using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public enum ButtonType { Exit, Industrial, Residential, Entertainment, Road };
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick(int type)
    {
        ButtonType buttonType = (ButtonType)type;
        switch (buttonType)
        {
            case ButtonType.Exit:
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Entertainment:
                break;
            case ButtonType.Industrial:
                break;
            case ButtonType.Residential:
                break;
            case ButtonType.Road:
                break;
         
        }
        Debug.Log("Clicked" + type);
    }
}
