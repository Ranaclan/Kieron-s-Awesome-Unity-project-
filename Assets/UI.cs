using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    //ui
    private CanvasGroup alpha;
    private int active;
    private List<string> tabKeys = new List<string> { "1", "2" };
    //player
    private GameObject player;
    private player script;
    //calculator
    private GameObject calculator;
    private GameObject calcInput;

    void Start()
    {
        //ui
        alpha = gameObject.GetComponent<CanvasGroup>();
        alpha.alpha = 0;
        active = 0;
        //player
        player = GameObject.Find("Player");
        script = player.GetComponent<player>();
        //calculator
        calculator = transform.GetChild(0).gameObject;
        calcInput = calculator.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        Open();
        Tabs();
    }

    void Open()
    {
        //opening
        if (Input.GetKeyDown("tab"))
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
