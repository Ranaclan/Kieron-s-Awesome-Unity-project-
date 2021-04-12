using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    //payer
    public bool control;
    //difficulty
    public static float bullets;
    public static float multiHits;
    //camera
    private Transform cam;
    private float xRotation;
    public static float xSensitivty;
    private float xRotationAddition;
    private float yRotation;
    public static float ySensitivty;
    private float yRotationAddition;
    //camera reset
    private float resetFraction = 1;
    private float xRotationAdditionStart;
    private float yRotationAdditionStart;
    private Quaternion transformDefault;
    private Quaternion cameraDefault;
    //ui
    private GameObject xCompass;
    public static Color xColour = new Color(0, 0, 0);
    public static float xRGB = 0;
    private GameObject yCompass;
    public static Color yColour = new Color(0, 0, 0);
    public static float yRGB = 0;
    //target
    private GameObject target;
    private float distance;
    private float forwardOffset;
    private float rightOffset;
    private float hits;
    //gun
    private GameObject gun;
    private float shootDelay;
    private float shootTimer;
    private Animation inspection;
    private ParticleSystem muzzleFlash;
    private AudioSource gunshotSound;
    private TMP_Text gunValues;
    //bullet
    private GameObject bullet;
    private bullet bulletScript;
    private float muzzleForce;
    private float bulletMass;
    private float muzzleAcceleration;
    private float muzzleTime;
    private float gunLength;
    public float gunMass;
    public float initial;
    private float terminalVelocity;
    private float terminalVelocityDistance;
    //wind
    private float windForce;
    public float windAcceleration;
    private float windSpeed;
    private float windPressure;
    private float bulletArea;
    private float dragCoefficient;
    //recoil
    private float recoilForce;
    private float recoilMoment;
    private float averageMomentsLength;
    private float halfLength;
    private float recoilAcceleration;
    private float recoil;
    public float recoilTotalAngle;
    private float recoilAngularTarget = 0;
    private bool recoiled;
    //stars
    private star star;
    private int starValue;
    //planet
    private float planetMass;
    private float radius;
    public float gravity;
    public const float gravitationalConstant = (float)6.67408 * (10 ^ -11);
    public string planetName;
    private int seed;

    private mapGenerate map;
    private nameGenerator nameGen;

    public float angle;

    //controls
    public static string resetButton;
    public static string uiButton;
    public static string inspectButton;

    void Start()
    {
        //player
        control = true;
        //camera
        cam = transform.GetChild(0);
        //camera reset
        transformDefault = transform.rotation;
        cameraDefault = cam.rotation;
        //ui
        Cursor.lockState = CursorLockMode.Locked;
        xCompass = transform.GetChild(1).GetChild(1).gameObject;
        yCompass = transform.GetChild(1).GetChild(2).gameObject;
        xCompass.GetComponent<TMP_Text>().color = xColour;
        yCompass.GetComponent<TMP_Text>().color = yColour;
        //target
        target = GameObject.Find("Target");
        hits = Mathf.Round(multiHits);
        //gun
        gun = cam.transform.GetChild(0).gameObject;
        shootDelay = 0.5f;
        shootTimer = 0;
        inspection = gun.GetComponent<Animation>();
        muzzleFlash = gun.transform.GetChild(0).GetComponent<ParticleSystem>();
        gunshotSound = gun.GetComponent<AudioSource>();
        gunValues = gun.transform.GetChild(1).GetComponent<TMP_Text>();
        gunValues.enabled = false;
        //bullet
        bullet = Resources.Load<GameObject>("Weapon/bullet");
        //stars
        star = GameObject.Find("Stars").GetComponent<star>();
        //map
        map = GameObject.Find("mapGenerator").GetComponent<mapGenerate>();
        nameGen = transform.GetComponent<nameGenerator>();

        Seed();
        Randomise();
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
    }

    public void Seed()
    {
        seed = Random.Range(-99999, 99999);
    }

    void Randomise()
    {
        //seed
        Random.InitState(seed);

        //values
        //target
        forwardOffset = Random.Range(50, 100);
        rightOffset = Random.Range(0, 60);
        target.transform.position = new Vector3(-forwardOffset, 3.5f, rightOffset);
        distance = Mathf.Sqrt(forwardOffset * forwardOffset + rightOffset * rightOffset);
        //planet
        planetMass = Random.Range(100, 1000);
        radius = Random.Range(50, 200);
        gravity = (-50 * planetMass) / (radius * radius);
        //bullet
        muzzleForce = Random.Range(80, 150);
        bulletMass = Random.Range(2, 30);
        muzzleAcceleration = muzzleForce / bulletMass;
        muzzleTime = Random.Range(1, 10);
        initial = muzzleAcceleration * muzzleTime; //max initial is 750
        terminalVelocityDistance = Random.Range(0, distance);
        //gun
        gunLength = muzzleAcceleration * muzzleTime * muzzleTime * 0.5f;
        halfLength = gunLength * 0.5f;
        gunMass = Random.Range(30, 50);
        //recoil
        recoilMoment = (muzzleForce * gunLength) + (halfLength * gravity * gunMass);
        averageMomentsLength = (gunLength + halfLength) / 2;
        recoilForce = recoilMoment / averageMomentsLength;
        recoilAcceleration = recoilForce / gunMass;
        recoil = recoilAcceleration * muzzleTime * muzzleTime * 0.5f;
        recoilTotalAngle = Mathf.Rad2Deg * Mathf.Atan(recoil / gunLength);
        //wind
        windSpeed = Random.Range(1, 40);
        windPressure = 0.00256f * windSpeed * windSpeed;
        bulletArea = Random.Range(1, 15);
        dragCoefficient = Random.Range(1, 5);
        windForce = windPressure * bulletArea * dragCoefficient;
        windAcceleration = windForce / bulletMass;
        terminalVelocity = Mathf.Sqrt(initial * initial + 2 * windAcceleration * terminalVelocityDistance);
        //world
        starValue = Random.Range(1, 10);
        star.Stars(starValue);
        map.Generate(seed);
        nameGen.Select(seed);
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

    void Shoot()
    {
        if (Input.GetKeyDown("mouse 0") && shootTimer <= 0)
        {
            if (bullets == 0)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                //bullet
                bullets -= 1;
                bullet = Instantiate(bullet, cam.transform.position, Quaternion.identity);
                bulletScript = bullet.GetComponent<bullet>();
                bulletScript.initial = initial;
                bulletScript.terminal = terminalVelocity;
                bulletScript.gravity = gravity;
                bulletScript.wind = windAcceleration;
                bulletScript.hits = hits;
                bulletScript.playerTransform = transform;
                bullet = Resources.Load<GameObject>("Weapon/bullet");
                //recoil
                recoilAngularTarget += recoilTotalAngle;
                cameraDefault.x -= recoilTotalAngle;
                recoiled = true;
                //shoot
                shootTimer = shootDelay;
                muzzleFlash.Play();
                gunshotSound.Play();
            }
        }

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }

    void Recoil()
    {
        if (recoilAngularTarget > 0)
        {
            float recoilAngle;
            recoilAngle = recoilTotalAngle / muzzleTime;
            cam.Rotate(-recoilAngle, 0, 0);
            recoilAngularTarget -= recoilAngle; 
        }

        if (recoilAngularTarget <= 0 && recoiled)
        {
            transformDefault = transform.rotation;
            cameraDefault = cam.transform.rotation;
            recoiled = false;
        }
    }

    void CameraReset()
    {
        //reset
        if (Input.GetKeyDown(resetButton) && resetFraction >= 1)
        {
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
        if(Input.GetKey("f"))
        {
            inspection.Play();
            gunValues.enabled = true;
        }
        if(Input.anyKey)
        {
            inspection.Stop();
            gunValues.enabled = false;
        }
    }

    void UI()
    {
        //rotation on crosshair
        xCompass.GetComponent<TMP_Text>().text = (Mathf.Round(xRotationAddition * 100) / 100).ToString();
        yCompass.GetComponent<TMP_Text>().text = (Mathf.Round(yRotationAddition * 100) / 100).ToString();

        //rgb
        if (xRGB != 0)
        {
            xCompass.GetComponent<TMP_Text>().color = RGB(xRGB, xCompass.GetComponent<TMP_Text>().color);
        }
        if (yRGB != 0)
        {
            yCompass.GetComponent<TMP_Text>().color = RGB(yRGB, xCompass.GetComponent<TMP_Text>().color);
        }

    }

    Color RGB(float speed, Color colour)
    {
        float red = colour.r;
        float green = colour.g;
        float blue = colour.b;

        if (blue <= 0f && red >= 0f)
        {
            red -= speed * Time.deltaTime;
            green += speed * Time.deltaTime;
        }
        if (red <= 0f && green >= 0f)
        {
            green -= speed * Time.deltaTime;
            blue += speed * Time.deltaTime;
        }
        if (green <= 0 && blue >= 0f)
        {
            blue -= speed * Time.deltaTime;
            red += speed * Time.deltaTime;
        }

        red = valueRange(red);
        green = valueRange(green);
        blue = valueRange(blue);

        Color rgb = new Color(red, green, blue);

        return rgb;
    }

    float valueRange(float value)
    {
        if (value > 1f)
        {
            value = 1f;
        }
        else if (value < 0f)
        {
            value = 0f;
        }

        return value;
    }

    public void Win()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = seed.ToString();
    }
}
