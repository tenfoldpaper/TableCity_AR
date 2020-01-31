using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    public static int width;
    //public int hej;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void goToGame(int n)
    {
        switch (n)
        {
            case 1:
                width = 15;
                break;
            case 2:
                width = 25;
                break;
            case 3:
                width = 40;
            break;
        }

        SceneManager.LoadScene("DesktopScene - Copy", LoadSceneMode.Single);
    }
}
