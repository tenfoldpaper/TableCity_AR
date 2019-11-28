﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public enum ButtonType { Exit, Industrial, Residential, Entertainment, Road, Electricity, Water};
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
            //TODO Subcategories?
            case ButtonType.Exit:
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Entertainment:
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Industrial:
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Residential:
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Road:
                MouseController.Instance.draging = true;
                MouseController.Instance.currentType = 0;
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Electricity:
                MouseController.Instance.draging = true;
                MouseController.Instance.currentType = 1;
                this.transform.root.gameObject.SetActive(false);
                break;
            case ButtonType.Water:
                MouseController.Instance.draging = true;
                MouseController.Instance.currentType = 2;
                this.transform.root.gameObject.SetActive(false);
                break;

        }
        Debug.Log("Clicked" + type);
    }
}
