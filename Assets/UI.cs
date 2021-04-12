using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UI : MonoBehaviour
{
    //ui
    private CanvasGroup alpha;
    private int active;
    private List<string> tabKeys = new List<string> { "1", "2" };
    //player
    private GameObject playerObject;
    private player script;
    //ammo
    private GameObject ammoDisplay;

    void Start()
    {
        //ui
        alpha = gameObject.GetComponent<CanvasGroup>();
        alpha.alpha = 0;
        active = 0;
        //player
        playerObject = GameObject.Find("Player");
        script = playerObject.GetComponent<player>();
        //ammo
        ammoDisplay = playerObject.transform.GetChild(1).GetChild(3).gameObject;
    }

    void Update()
    {
        Ammo();
        Open();
        Tabs();
    }

    void Ammo()
    {
        ammoDisplay.GetComponent<TMP_Text>().text = player.bullets.ToString();
    }

    void Open()
    {
        //opening
        if (Input.GetKeyDown(player.uiButton))
        {
            if (alpha.alpha == 0)
            {
                //activate ui
                alpha.alpha = 1;
                Cursor.lockState = CursorLockMode.None;
                script.control = false;
            }
            else if (alpha.alpha == 1)
            {
                //deactivate ui
                alpha.alpha = 0;
                Cursor.lockState = CursorLockMode.Locked;
                script.control = true;
            }
        }
    }

    void Tabs()
    {
        //disable tab switching if calculator input field selected
        if (!EventSystem.current.currentSelectedGameObject)
        {
            foreach (string i in tabKeys)
            {
                //check if one of the keys corresponding to a tab in the ui is pressed
                if (Input.GetKey(i))
                {
                    int j = int.Parse(i) - 1;
                    //check that pressed key's corresponding tab is not already active
                    if (active != j)
                    {
                        //change active tab
                        transform.GetChild(active).gameObject.SetActive(false);
                        active = j;
                        transform.GetChild(active).gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
