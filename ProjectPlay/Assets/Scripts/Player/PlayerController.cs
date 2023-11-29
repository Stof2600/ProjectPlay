using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody RB;
    Vector2 PlayerInput;

    public Camera Cam;

    public float MoveSpeed = 10;
    public float CamSens;

    float CamX;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        Cam = GetComponentInChildren<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if(Input.GetKeyDown(KeyCode.X))
        {
            WeaponFire();
        }
    }

    void Movement()
    {
        PlayerInput.x = Input.GetAxis("Horizontal");
        PlayerInput.y = Input.GetAxis("Vertical") * MoveSpeed;

        transform.Rotate(0, PlayerInput.x * CamSens * Time.deltaTime, 0);
        RB.velocity = transform.forward * PlayerInput.y + transform.right * 0 + transform.up * RB.velocity.y;
    }

    void WeaponFire()
    {
        if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out RaycastHit Hit))
        {
            print(Hit.transform.name);
        }
    }
}
