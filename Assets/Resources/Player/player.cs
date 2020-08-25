using TMPro;
using UnityEngine;

public class player : MonoBehaviour
{
    //payer
    public bool control;
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
    private float resetFraction;
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
    private float distance;
    //shooting
    private GameObject gun;
    private float shootDelay;
    private float shootTimer;
    private ParticleSystem muzzleFlash;
    //bullet
    private GameObject bullet;
    private bullet bulletScript;
    private float time;
    private float initial;
    private float mass;
    //world
    private float gravity;
    //ui
    private GameObject xRotationAdditionCrosshair;
    private GameObject yRotationAdditionCrosshair;

    void Start()
    {
        //player
        control = true;
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
        //target
        target = GameObject.Find("Target");
        distance = Random.Range(-100, -50);
        //Debug.Log("distance " + distance);
        //target.transform.position = transform.position + new Vector3(distance, 0, 0);
        target.transform.position = new Vector3(distance, 3.5f, 0);
        //shooting
        gun = cam.transform.GetChild(0).gameObject;
        shootDelay = 0.5f;
        shootTimer = 0;
        muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
        //bullet
        bullet = Resources.Load<GameObject>("Weapon/bullet");
        initial = Random.Range(30, 150);
        //Debug.Log("initial" + initial);
        time = (distance / initial);
        //world
        gravity = Random.Range(-3, -25);
        //Debug.Log("gravity " + gravity);
        //ui
        Cursor.lockState = CursorLockMode.Locked;
        xRotationAdditionCrosshair = transform.GetChild(1).GetChild(1).gameObject;
        yRotationAdditionCrosshair = transform.GetChild(1).GetChild(2).gameObject;
    }

    void FixedUpdate()
    {
        //disable when ui open
        if (control)
        {
            //camera rotation stuff
            Turn();
            CameraRotate();
            //shooty stuff
            Shoot();
            //ui stuff
            UI();
        }

        CameraReset();
    }

    void Turn()
    {
        //input
        if (!Input.GetKey("y"))
        {
            xRotation = Input.GetAxis("Mouse X") * xSensitivty * Time.deltaTime;
        }

        //rotate
        transform.Rotate(0, xRotation, 0);

        //keep turn rotation -360<x<360
        xRotationAddition += xRotation;
        if (xRotationAddition >= 360)
        {
            xRotationAddition -= 360;
        }
        if (xRotationAddition <= -360)
        {
            xRotationAddition += 360;
        }
    }

    void CameraRotate()
    {
        //input
        if (!Input.GetKey("x"))
        {
            yRotation = Input.GetAxis("Mouse Y") * ySensitivty * Time.deltaTime;
        }
        yRotationAddition -= yRotation;

        //clamp rotation
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

        //rotate
        cam.Rotate(yRotation, 0, 0);
    }

    void CameraReset()
    {

        //reset
        if (Input.GetKeyDown("q") && resetFraction >= 1)
        {
            resetFraction = 0;

            xStart = transform.rotation;
            xRotationAdditionStart = xRotationAddition;

            yStart = cam.transform.rotation;
            yRotationAdditionStart = yRotationAddition;
        }

        if (resetFraction < 1)
        {
            //x
            transform.rotation = Quaternion.Slerp(xStart, xDefault, resetFraction);
            xRotationAddition = Mathf.Lerp(xRotationAdditionStart, 0, resetFraction);

            //y
            cam.transform.rotation = Quaternion.Slerp(yStart, yDefault, resetFraction);
            yRotationAddition = Mathf.Lerp(yRotationAdditionStart, 0, resetFraction);

            resetFraction += Time.deltaTime;
        }
    }

    void Shoot()
    {
        if (Input.GetKeyDown("mouse 0") && shootTimer <= 0)
        {
            //bullet
            bullet = Instantiate(bullet, cam.transform.position, Quaternion.identity);
            bulletScript = bullet.GetComponent<bullet>();
            bulletScript.initial = initial;
            bulletScript.gravity = gravity;
            bulletScript.time = (time + 5);
            bulletScript.player = transform;
            bullet = Resources.Load<GameObject>("Weapon/bullet");
            //shoot
            shootTimer = shootDelay;
            muzzleFlash.Play();
        }

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }

    void UI()
    {
        //rotation on crosshair
        xRotationAdditionCrosshair.GetComponent<TMP_Text>().text = (Mathf.Round(xRotationAddition * 100) / 100).ToString();
        yRotationAdditionCrosshair.GetComponent<TMP_Text>().text = (Mathf.Round(yRotationAddition * 100) / 100).ToString();
    }
}
