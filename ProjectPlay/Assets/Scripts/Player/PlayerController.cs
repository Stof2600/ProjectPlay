using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : StatScript
{

    Rigidbody RB;
    Vector2 PlayerInput;

    public Camera Cam;

    public float MoveSpeed = 10;
    public float CamSens, SniperSens;

    public int CurrentWeapon = 0;
    public int[] MaxAmmo;
    public int[] CurrentAmmo;
    public GameObject[] WeaponModels;

    public float FireRate;
    float FireTime;
    bool CanFire;

    bool HoldFire;

    public bool FreezePlayer;

    ScoreManager SM;

    public GameObject DecalPrefab;
    public GameObject RocketLauncherProjectile;
    public LineRenderer SniperLaser;
    public Transform LaserPoint;

    public Text WeaponText;
    public Text ScoreText, TimeText, DeathText, EndScoreText;
    public CustomSlider HealthSlider;
    public GameObject DeathScreen;

    float CamX;

    public float StunTimer;
    public float DeathTimer;

    public GameObject ScorePrefab;

    public Transform[] FireEffects;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        Cam = GetComponentInChildren<Camera>();
        SniperLaser = GetComponentInChildren<LineRenderer>();

        HealthSlider.MaxValue = MaxHealth;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ResetHealth();

        CurrentAmmo[0] = 75;
        CurrentAmmo[1] = 25;

        HoldFire = false;
        FireTime = FireRate;
        CanFire = true;

        foreach (Transform T in FireEffects)
        {
            T.gameObject.SetActive(false);
        }

        if (!FindObjectOfType<ScoreManager>())
        {
            Instantiate(ScorePrefab);
        }
        else
        {
            SM = FindObjectOfType<ScoreManager>();
            SM.LoadPlayerPos(transform);
            if(SM.TotalScore < 0)
            {
                SM.TotalScore = 0;
            }
        }

        DeathScreen.SetActive(false);
        DeathTimer = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if(FreezePlayer)
        {
            DeathTimer -= Time.deltaTime;
            if(DeathTimer <= 0)
            {
                SM.TotalScore = -1;
                SM.CurrentLevel = 0;
                SceneManager.LoadScene(0);
            }

            return;
        }

        if(!HoldFire && StunTimer <= 0)
        {
            Movement();
            Cam.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if(HoldFire)
        {
            SniperAim();
        }

        if (Input.GetKey(KeyCode.Space) && FireTime >= FireRate && CanFire)
        {
            WeaponCheck();
        }
        else if(FireTime < FireRate)
        {
            FireTime += Time.deltaTime;
        }

        if(!CanFire && Input.GetKeyUp(KeyCode.Space))
        {
            if(HoldFire)
            {
                WeaponFire(5, Vector3.zero, Mathf.Infinity, 1);
                HoldFire = false;
            }

            CanFire = true;
        }
        if(Input.GetKeyDown(KeyCode.LeftControl) || CurrentAmmo[CurrentWeapon] <= 0)
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

        if(Input.GetKeyDown(KeyCode.F))
        {
            FindObjectOfType<ScoreManager>().SaveScore();
            FindObjectOfType<ScoreManager>().LoadScore();

            SceneManager.LoadScene(0);
        }

        HealthSlider.CurrentValue = CurrentHealth;

        if(!SM)
        {
            SM = FindObjectOfType<ScoreManager>();
        }
        else
        {
            SM.UpdatePlayerPos(transform);
        }
        ScoreText.text = "SCORE: " + SM.TotalScore.ToString("00000") + "\nQOUTA: " + SM.LevelQouta.ToString("00000");

        int TimeInSecondsFixed = (int)SM.LevelTimer;
        int Minutes = TimeInSecondsFixed / 60;
        int Seconds = TimeInSecondsFixed - (Minutes * 60);
        TimeText.text = "TIME\n" + Minutes.ToString("00") + ":" + Seconds.ToString("00");


        string WeaponName = "";
        switch(CurrentWeapon)
        {
            case 0:
                WeaponName = "PISTOL";
                break;
            case 1:
                WeaponName = "SHOTGUN";
                break;
            case 2:
                WeaponName = "SMG";
                break;
            case 3:
                WeaponName = "SNIPER";
                break;
            case 4:
                WeaponName = "RPG";
                break;
            case 5:
                WeaponName = "MELEE";
                break;
        }

        WeaponText.text = WeaponName + "\n" + CurrentAmmo[CurrentWeapon];

        for (int i = 0; i < WeaponModels.Length; i++)
        {
            if(i == CurrentWeapon)
            {
                WeaponModels[i].SetActive(true);
            }
            else
            {
                WeaponModels[i].SetActive(false);
            }
        }
    }

    void Movement()
    {
        PlayerInput.x = Input.GetAxis("Horizontal");
        PlayerInput.y = Input.GetAxis("Vertical") * MoveSpeed;

        transform.Rotate(0, PlayerInput.x * CamSens * Time.deltaTime, 0);
        RB.velocity = transform.forward * PlayerInput.y + transform.right * 0 + transform.up * RB.velocity.y;

        SniperLaser.gameObject.SetActive(false);
        Cam.fieldOfView = 60;
    }

    void WeaponCheck()
    {
        switch (CurrentWeapon)
        {
            case 0:
                WeaponFire(1, Vector3.zero, Mathf.Infinity, 1);
                CanFire = false;
                break;
            case 1:
                if(CurrentAmmo[CurrentWeapon] > 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        WeaponFire(1, new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0), Mathf.Infinity, 0);
                    }

                    CurrentAmmo[CurrentWeapon] -= 1;
                }
                CanFire = false;
                break;
            case 2:
                WeaponFire(1, new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0), Mathf.Infinity, 1);
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
                WeaponFire(1, Vector3.zero, 2f, 0);
                CanFire = false;
                break;
        }

        FireTime = 0;
    }

    void WeaponFire(int Damage, Vector3 Spread, float Range, int AmmoUse)
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
                Hit.transform.GetComponent<EnemyController>().DamageTick = 0.1f;
            }
            else
            {
                Quaternion LookRot = Quaternion.LookRotation(-Hit.normal, Vector3.up);
                Instantiate(DecalPrefab, Hit.point, LookRot);
            }
        }

        CurrentAmmo[CurrentWeapon] -= AmmoUse;

        StartCoroutine(FireEffect());

        if (CurrentAmmo[CurrentWeapon] <= 0)
        {
            CanFire = false;
        }
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

        RB.velocity = transform.forward * 0 + transform.right * 0 + transform.up * RB.velocity.y;

        CamX = Cam.transform.localEulerAngles.x - PlayerInput.y * SniperSens * Time.deltaTime;

        Cam.transform.localEulerAngles = new Vector3(CamX, 0, 0);
        transform.Rotate(0, PlayerInput.x * SniperSens * Time.deltaTime, 0);

        Cam.fieldOfView = 30;

        SniperLaser.gameObject.SetActive(true);
        SniperLaser.SetPosition(0, LaserPoint.position);
        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out RaycastHit Hit))
        {
            SniperLaser.SetPosition(1, Hit.point);
        }
        else
        {
            SniperLaser.SetPosition(1, Cam.transform.position + Cam.transform.forward * 1000);
        }
    }

    public void KillPlayer(string Reason)
    {
        DeathScreen.SetActive(true);
        DeathText.text = Reason;
        EndScoreText.text = "FINAL SCORE\n" + SM.TotalScore.ToString("00000");
        FreezePlayer = true;

        SM.ResetPlayerPos();
        SM.LevelTimer = SM.StartLevelTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DeathPlane"))
        {
            KillPlayer("OUT OF BOUNDS");
        }
    }

    IEnumerator FireEffect()
    {
        foreach(Transform T in FireEffects)
        {
            T.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);

        foreach (Transform T in FireEffects)
        {
            T.gameObject.SetActive(false);
        }
    }
}
