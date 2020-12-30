using TMPro;
using UnityEngine;

public class player : MonoBehaviour
{
    //payer
    public bool control;
    //camera
    private Transform cam;
    private float xRotation;
    private float xSensitivty;
    private float xRotationAddition;
    private float yRotation;
    private float ySensitivty;
    private float yRotationAddition;
    private Vector3 transformLocal;
    private Vector3 camLocal;
    //camera reset
    private float resetFraction = 1;
    private Vector3 xStart;
    private Vector3 yStart;
    private float xRotationAdditionStart;
    private float yRotationAdditionStart;
    private Quaternion transformDefault;
    private Quaternion cameraDefault;
    //ui
    private GameObject xCompass;
    private GameObject yCompass;
    //target
    private GameObject target;
    private float distance;
    //gun
    private GameObject gun;
    private float shootDelay;
    private float shootTimer;
    private ParticleSystem muzzleFlash;
    //seed
    private int seed;
    //bullet
    private GameObject bullet;
    private bullet bulletScript;
    private float muzzleForce;
    private float mass;
    private float muzzleAcceleration;
    private float muzzleTime;
    private float gunLength;
    public float gunMass;
    private float bulletArea;
    private float dragCoefficient;
    public float initial;
    //recoil
    public float recoilForce;
    public float recoilMoment;
    public float averageMomentsLength;
    public float halfLength;
    public float recoilAcceleration;
    public float recoil;
    public float recoilTotalAngle;
    public float recoilAngularTarget = 0;
    private float recoilVelocity;
    private float recoilDisplacement;
    private bool recoiled;
    //stars
    private star star;
    private int starValue;
    //planet
    private float planetMass;
    private float radius;
    private float gravity;
    public const float gravitationalConstant = (float)6.67408 * (10 ^ -11);
    //wind
    private float windSpeed;
    private float windBearing;
    private float windPressure;
    private float windForce;
    //map
    private mapGenerate map;

    public float time;
    public float drop;
    public float angle;

    public float a;

    void Start()
    {
        //player
        control = true;
        //camera
        cam = transform.GetChild(0);
        Cursor.lockState = CursorLockMode.Locked;
        xSensitivty = 25;
        ySensitivty = -25;
        transformLocal = transform.localEulerAngles;
        camLocal = cam.transform.localEulerAngles;
        //camera reset
        transformDefault = transform.rotation;
        cameraDefault = cam.rotation;
        //ui
        Cursor.lockState = CursorLockMode.Locked;
        xCompass = transform.GetChild(1).GetChild(1).gameObject;
        yCompass = transform.GetChild(1).GetChild(2).gameObject;
        //target
        target = GameObject.Find("Target");
        //gun
        gun = cam.transform.GetChild(0).gameObject;
        shootDelay = 0.5f;
        shootTimer = 0;
        muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
        //bullet
        bullet = Resources.Load<GameObject>("Weapon/bullet");
        //stars
        star = GameObject.Find("Stars").GetComponent<star>();
        //map
        map = GameObject.Find("mapGenerator").GetComponent<mapGenerate>();

        Randomise(Seed());
    }

    void FixedUpdate()
    {
        //disable when ui open
        if (control)
        {
            //camera rotation stuff
            Turn();
            CameraRotate();
            CameraReset();
            //gun stuff
            Shoot();
            Inspect();
            Recoil();
            //ui stuff
            UI();
        }

        if (Input.GetKey("l"))
        {
            Randomise(Seed());
        }

        if(Input.GetKey("o"))
        {
            Randomise(seed);
        }
    }

    public int Seed()
    {
        return Random.Range(-99999, 99999);
    }

    void Randomise(int seed)
    {
        //seed
        Random.InitState(seed);

        //values
        //target
        distance = Random.Range(-100, -50);
        target.transform.position = new Vector3(distance, 3.5f, 0);
        //planet
        planetMass = Random.Range(100, 1000);
        radius = Random.Range(50, 200);
        gravity = (-50 * planetMass) / (radius * radius);
        Debug.Log("gravity " + gravity);
        //bullet
        muzzleForce = Random.Range(80, 150);
        Debug.Log("muzzle force " + muzzleForce);
        mass = Random.Range(2, 30);
        muzzleAcceleration = muzzleForce / mass;
        muzzleTime = Random.Range(1, 10);
        Debug.Log("muzzle time " + muzzleTime);
        bulletArea = Random.Range(1, 15);
        dragCoefficient = Random.Range(0.1f, 5);
        //gun
        gunLength = muzzleAcceleration * muzzleTime * muzzleTime * 0.5f;
        halfLength = gunLength * 0.5f;
        Debug.Log("gun length " + gunLength);
        gunMass = Random.Range(30, 50);
        Debug.Log("gun mass " + gunMass);
        //recoil
        recoilMoment = (muzzleForce * gunLength) + (halfLength * gravity * gunMass);
        averageMomentsLength = (gunLength + halfLength) / 2;
        recoilForce = recoilMoment / averageMomentsLength;
        recoilAcceleration = recoilForce / gunMass;
        recoil = recoilAcceleration * muzzleTime * muzzleTime * 0.5f;
        recoilTotalAngle = Mathf.Rad2Deg * Mathf.Atan(recoil / gunLength);
        Debug.Log(recoilTotalAngle);
        //wind
        windSpeed = Random.Range(1, 20);
        windPressure = 0.0026f * windSpeed * windSpeed;
        windForce = windPressure * bulletArea * dragCoefficient;

        initial = muzzleAcceleration * muzzleTime;
        time = -distance / initial;
        drop = gravity * 0.5f * time * time;
        angle = Mathf.Atan2(drop, distance) * Mathf.Rad2Deg;

        //world
        starValue = Random.Range(1, 10);
        star.Stars(starValue);
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

    void Shoot()
    {
        if (Input.GetKeyDown("mouse 0") && shootTimer <= 0)
        {
            //bullet
            bullet = Instantiate(bullet, cam.transform.position, Quaternion.identity);
            bulletScript = bullet.GetComponent<bullet>();
            bulletScript.initial = initial;
            bulletScript.gravity = gravity;
            bulletScript.player = transform;
            bullet = Resources.Load<GameObject>("Weapon/bullet");
            //recoil
            recoilAngularTarget += recoilTotalAngle;
            recoilVelocity = 0;
            recoilDisplacement = 0;
            cameraDefault.x -= recoilTotalAngle;
            recoiled = true;
            //shoot
            shootTimer = shootDelay;
            muzzleFlash.Play();
        }

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }

    void Recoil()
    {
        float recoilAngle;
        if (recoilAngularTarget > 0)
        {
            recoilAngle = recoilTotalAngle / muzzleTime;
            cam.Rotate(-recoilAngle, 0, 0);
            recoilAngularTarget -= recoilAngle;
        }

        if (recoilAngularTarget <= 0 && recoiled)
        {
            transformDefault = transform.transform.rotation;
            cameraDefault = cam.transform.rotation;
            camLocal = cam.transform.localEulerAngles;
            recoiled = false;
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
            Debug.Log(xStart);
            Debug.Log("a");
            Debug.Log(transformDefault);
            resetFraction = 0;

            xRotationAdditionStart = xRotationAddition;
            yRotationAdditionStart = yRotationAddition;
        }

        if (resetFraction < 1)
        {
            //transform reset
            transform.rotation = Quaternion.Slerp(transform.rotation, transformDefault, resetFraction);
            xRotationAddition = Mathf.Lerp(xRotationAdditionStart, 0, resetFraction);

            //cam reset
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, cameraDefault, resetFraction);
            yRotationAddition = Mathf.Lerp(yRotationAdditionStart, 0, resetFraction);

            resetFraction += Time.deltaTime;
        }
    }

    void Inspect()
    {

    }

    void UI()
    {
        /*
        float camAngle;
        float transformAngle;
        string camText;
        string transformText;
        camAngle = cam.transform.localEulerAngles.x;
        transformAngle = transform.localEulerAngles.y;
        if (transformAngle > 180)
        {
            transformAngle = 360 - transformAngle;
            transformAngle *= -1;
        }
        camText = (Mathf.Round((camAngle - camLocal.x) * -100) / 100).ToString();
        transformText = (Mathf.Round((transformAngle - transformLocal.x) * -100) / 100).ToString();
        xCompass.GetComponent<TMP_Text>().text = fixAngles(camText);
        yCompass.GetComponent<TMP_Text>().text = fixAngles(transformText);
        */

        //rotation on crosshair
        xCompass.GetComponent<TMP_Text>().text = (Mathf.Round(xRotationAddition * 100) / 100).ToString();
        yCompass.GetComponent<TMP_Text>().text = (Mathf.Round(yRotationAddition * 100) / 100).ToString();
    }

    string fixAngles(string x)
    {
        if(x == "360" || x == "-360")
        {
            return "0";
        }

        return x;
    }
}
