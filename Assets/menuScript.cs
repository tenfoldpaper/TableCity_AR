﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void goToGame()
    {
        SceneManager.LoadScene("DesktopScene");
    }
}
