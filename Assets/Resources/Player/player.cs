using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class player : MonoBehaviour
{
    //movement
    private Rigidbody rb;
    private float acceleration;
    private float decceleration;
    private float max;
    private float forward;
    private float back;
    private float right;
    private float left;
    //camera
    private Transform cam;
    private float xRotation;
    private float xSensitivty;
    private float xRotationAddition;
    private float yRotation;
    private float ySensitivty;
    private float yRotationAddition;
    //camera reset
    private Quaternion xDefault;
    private Quaternion yDefault;
    private float xResetFraction;
    private float yResetFraction;
    private Quaternion xStart;
    private Quaternion yStart;
    private float xRotationAdditionStart;
    private float yRotationAdditionStart;
    //target
    private GameObject target;
    private float zDistance;
    private float time;
    private float bulletDrop;
    private float xAngle;
    //shooty
    private GameObject bullet;
    private bullet bulletScript;
    private float shootDelay;
    private float initial;
    private float mass;
    //ui
    private GameObject xRotationAdditionCrosshair;
    private GameObject yRotationAdditionCrosshair;

    //controller
    private float gravity;

    void Start()
    {
        //movement
        rb = transform.GetComponent<Rigidbody>();
        acceleration = 5;
        decceleration = 15;
        max = 150;
        //camera
        cam = transform.GetChild(0);
        Cursor.lockState = CursorLockMode.Locked;
        xSensitivty = 25;
        ySensitivty = -25;
        //camera reset
        xDefault = transform.rotation;
        yDefault = cam.rotation;
        xResetFraction = 1;
        yResetFraction = 1;
        //controller
        gravity = -10f;
        //shooty
        bullet = Resources.Load<GameObject>("Weapon/bullet");
        initial = 80;
        shootDelay = 0;
        //target
        target = GameObject.Find("Target");
        zDistance = Mathf.Sqrt((target.transform.position - transform.position).x * (target.transform.position - transform.position).x);
        time = zDistance / initial;
        bulletDrop = (-gravity * time * time) / 2;
        xAngle = Mathf.Atan2(bulletDrop, zDistance) * Mathf.Rad2Deg;
        //ui
        Cursor.lockState = CursorLockMode.Locked;
        xRotationAdditionCrosshair = transform.GetChild(1).GetChild(1).gameObject;
        yRotationAdditionCrosshair = transform.GetChild(1).GetChild(2).gameObject;
    }

    void FixedUpdate()
    {
        Look();
        Shoot();
        UI();
    }

    void Look()
    {
        //player rotate
        if(!Input.GetKey("y"))
        {
            xRotation = Input.GetAxis("Mouse X") * xSensitivty * Time.deltaTime;
        }
        transform.Rotate(0, xRotation, 0);
        xRotationAddition += xRotation;
        if(xRotationAddition >= 360)
        {
            xRotationAddition -= 360;
        }
        if(xRotationAddition <= -360)
        {
            xRotationAddition += 360;
        }

        //camera rotate
        if (!Input.GetKey("x"))
        {
            yRotation = Input.GetAxis("Mouse Y") * ySensitivty * Time.deltaTime;
        }
        yRotationAddition -= yRotation;
        if (yRotationAddition >= 90)
        {
            yRotation = 0;
            yRotationAddition = 90;
        }
        if (yRotationAddition <= -90)
        {
            yRotation = 0;
            yRotationAddition = -90;
        }
        cam.Rotate(yRotation, 0, 0);

        //reset
        if (Input.GetKey("q"))
        {
            if (!Input.GetKey("y"))
            {
                xStart = transform.rotation;
                xRotationAdditionStart = xRotationAddition;
                xResetFraction = 0;
            }

            if (!Input.GetKey("x"))
            {
                yStart = cam.transform.rotation;
                yRotationAdditionStart = yRotationAddition;
                yResetFraction = 0;
            }
        }

        if (xResetFraction < 1)
        {
            transform.rotation = Quaternion.Slerp(xStart, xDefault, xResetFraction);
            xResetFraction += Time.deltaTime;
            xRotationAddition = Mathf.Lerp(xRotationAdditionStart, 0, xResetFraction);
        }
        else
        {
            xResetFraction = 1;
        }

        if (yResetFraction < 1)
        {
            cam.transform.rotation = Quaternion.Slerp(yStart, yDefault, yResetFraction);
            yResetFraction += Time.deltaTime;
            yRotationAddition = Mathf.Lerp(yRotationAdditionStart, 0, yResetFraction);
        }
        else
        {
            yResetFraction = 1;
        }
    }

    void Shoot()
    {
        if(Input.GetKeyDown("mouse 0") && shootDelay <= 0)
        {
            bullet = Instantiate(bullet, cam.transform.position, Quaternion.identity);
            bulletScript = bullet.GetComponent<bullet>();
            bulletScript.initial = initial;
            bulletScript.gravity = gravity;
            bulletScript.player = transform;
            bullet = Resources.Load<GameObject>("Weapon/bullet");
            shootDelay = 0.5f;
        }

        if(shootDelay > 0)
        {
            shootDelay -= Time.deltaTime;
        }
    }

    void UI()
    {
        //rotation on crosshair
        xRotationAdditionCrosshair.GetComponent<TMP_Text>().text = xRotationAddition.ToString();
        yRotationAdditionCrosshair.GetComponent<TMP_Text>().text = yRotationAddition.ToString();
    }
}
