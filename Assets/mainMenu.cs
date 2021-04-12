using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    //menu
    private GameObject previous;
    private int quitNum = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        previous = transform.GetChild(1).gameObject;
    }

    public void UpdatePrevious(GameObject newPrevious)
    {
        previous = newPrevious;
    }

    public void ChangeMenu(GameObject newMenu)
    {
        previous.SetActive(false);
        newMenu.SetActive(true);
    }

    public void Quit()
    {
        switch(quitNum)
        {
            case 0:
                quitNum = 1;
                break;
            case 1:
                Application.Quit();
                break;
        }
    }

    public void LoadLevel()
    {
        Transform menu = transform.GetChild(3);
        player.bullets = float.Parse(menu.GetChild(0).GetComponent<TMP_InputField>().text);
        player.multiHits = float.Parse(menu.GetChild(1).GetComponent<TMP_InputField>().text);
        SceneManager.LoadScene(1);
    }

    public void LoadTutorial(int tutorial)
    {
        SceneManager.LoadScene(tutorial);
    }

    public void CustomiseUI(int colourPointer)
    {
        Transform menu = transform.GetChild(6);
        switch (colourPointer)
        {
            case 0:
                player.xColour.r = float.Parse(menu.GetChild(0).GetComponent<TMP_InputField>().text);
                break;
            case 1:
                player.xColour.g = float.Parse(menu.GetChild(1).GetComponent<TMP_InputField>().text);
                break;
            case 2:
                player.xColour.b = float.Parse(menu.GetChild(2).GetComponent<TMP_InputField>().text);
                break;
            case 3:
                player.yColour.r = float.Parse(menu.GetChild(3).GetComponent<TMP_InputField>().text);
                break;
            case 4:
                player.yColour.g = float.Parse(menu.GetChild(4).GetComponent<TMP_InputField>().text);
                break;
            case 5:
                player.yColour.b = float.Parse(menu.GetChild(5).GetComponent<TMP_InputField>().text);
                break;
            case 6:
                player.xRGB = float.Parse(menu.GetChild(5).GetComponent<TMP_InputField>().text);
                break;
            case 7:
                player.yRGB = float.Parse(menu.GetChild(5).GetComponent<TMP_InputField>().text);
                break;
        }
    }

    public void EditControls(int controlsPointer)
    {
        switch (controlsPointer)
        {
            case 0:
                player.resetButton = setKey();
                break;
            case 1:
                player.uiButton = setKey();
                break;
            case 2:
                player.inspectButton = setKey();
                break;
        }
    }

    string setKey()
    {
        foreach (string key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
            {
                return key;
            }
        }

        return "m";
    }
}
