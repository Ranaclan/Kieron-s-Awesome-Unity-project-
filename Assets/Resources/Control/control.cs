using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control : MonoBehaviour
{
    //player
    private GameObject player;
    private player playerScript;
    //target
    private GameObject target;
    //values
    private float distance;


    void Start()
    {
        //player
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<player>();
        //target
        target = GameObject.Find("Target");
    }
}
