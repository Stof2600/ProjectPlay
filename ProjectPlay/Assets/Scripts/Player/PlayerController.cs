using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StatScript
{

    Rigidbody RB;
    Vector2 PlayerInput;

    public Camera Cam;

    public float MoveSpeed = 10;
    public float CamSens;

    public int CurrentWeapon = 0;
    public int[] MaxAmmo;
    public int[] CurrentAmmo;

    public float FireRate;
    float FireTime;
    bool CanFire;

    bool HoldFire;

    public GameObject DecalPrefab;
    public GameObject RocketLauncherProjectile;
    public LineRenderer SniperLaser;

    float CamX;

    public float StunTimer;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        Cam = GetComponentInChildren<Camera>();
        SniperLaser = GetComponentInChildren<LineRenderer>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ResetHealth();

        CurrentAmmo[0] = 50;

        HoldFire = false;
        FireTime = FireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(!HoldFire && StunTimer <= 0)
        {
            Movement();
            Cam.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if(HoldFire)
        {
            SniperAim();
        }

        if (Input.GetKey(KeyCode.X) && FireTime >= FireRate && CanFire)
        {
            WeaponCheck();
        }
        else if(FireTime < FireRate)
        {
            FireTime += Time.deltaTime;
        }

        if(!CanFire && Input.GetKeyUp(KeyCode.X))
        {
            if(HoldFire)
            {
                WeaponFire(5, Vector3.zero, Mathf.Infinity);
                HoldFire = false;
            }

            CanFire = true;
        }
        if(Input.GetKeyDown(KeyCode.Z) || CurrentAmmo[CurrentWeapon] <= 0)
        {
            CurrentWeapon += 1;

            if(CurrentWeapon >= 6)
            {
                CurrentWeapon = 0;
            }
        }

        if(StunTimer > 0)
        {
            StunTimer -= Time.deltaTime;
        }

        CurrentAmmo[5] = 999;
        MaxAmmo[5] = 999;
    }

    void Movement()
    {
        PlayerInput.x = Input.GetAxis("Horizontal");
        PlayerInput.y = Input.GetAxis("Vertical") * MoveSpeed;

        transform.Rotate(0, PlayerInput.x * CamSens * Time.deltaTime, 0);
        RB.velocity = transform.forward * PlayerInput.y + transform.right * 0 + transform.up * RB.velocity.y;

        SniperLaser.gameObject.SetActive(false);
    }

    void WeaponCheck()
    {
        switch (CurrentWeapon)
        {
            case 0:
                WeaponFire(1, Vector3.zero, Mathf.Infinity);
                CanFire = false;
                break;
            case 1:
                for (int i = 0; i < 8; i++)
                {
                    WeaponFire(1, new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0), Mathf.Infinity);
                }
                CanFire = false;
                break;
            case 2:
                WeaponFire(1, new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0), Mathf.Infinity);
                break;
            case 3:
                HoldFire = true;
                CanFire = false;
                break;
            case 4:
                ProjectileFire(RocketLauncherProjectile);
                CanFire = false;
                break;
            case 5:
                WeaponFire(1, Vector3.zero, 2f);
                CanFire = false;
                break;
        }

        FireTime = 0;
    }

    void WeaponFire(int Damage, Vector3 Spread, float Range)
    {
        if(CurrentAmmo[CurrentWeapon] <= 0)
        {
            return;
        }

        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward + transform.right * Spread.x + transform.up * Spread.y, out RaycastHit Hit, Range))
        {
            if (Hit.transform.GetComponent<EnemyController>())
            {
                Hit.transform.GetComponent<EnemyController>().TakeDamage(Damage);
            }

            Instantiate(DecalPrefab, Hit.point, transform.rotation);
            print(Hit.transform.name);
        }

        CurrentAmmo[CurrentWeapon] -= 1;
    }
    void ProjectileFire(GameObject Projectile)
    {
        if (CurrentAmmo[CurrentWeapon] <= 0)
        {
            return;
        }

        Instantiate(Projectile, Cam.transform.position, Cam.transform.rotation);

        CurrentAmmo[CurrentWeapon] -= 1;
    }

    public void AddAmmo(int WeaponID, int Amount)
    {
        CurrentAmmo[WeaponID] += Amount;

        if(CurrentAmmo[WeaponID] > MaxAmmo[WeaponID])
        {
            CurrentAmmo[WeaponID] = MaxAmmo[WeaponID];
        }
    }

    void SniperAim()
    {
        PlayerInput.x = Input.GetAxis("Horizontal");
        PlayerInput.y = Input.GetAxis("Vertical");

        CamX = Cam.transform.localEulerAngles.x - PlayerInput.y * CamSens * Time.deltaTime;

        Cam.transform.localEulerAngles = new Vector3(CamX, 0, 0);
        transform.Rotate(0, PlayerInput.x * CamSens * Time.deltaTime, 0);

        SniperLaser.gameObject.SetActive(true);
        SniperLaser.SetPosition(0, transform.position);
        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out RaycastHit Hit))
        {
            SniperLaser.SetPosition(1, Hit.point);
        }
        else
        {
            SniperLaser.SetPosition(1, Cam.transform.position + Cam.transform.forward * 1000);
        }
    }
}
