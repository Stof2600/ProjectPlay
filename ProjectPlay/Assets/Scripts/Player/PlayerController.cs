using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody RB;
    Vector2 PlayerInput, cameraInput;

    public Transform cam;
    public float MoveSpeed = 10;
    public Vector2 CamSens;

    float CamX;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        PlayerInput.x = Input.GetAxis("Horizontal") * MoveSpeed;
        PlayerInput.y = Input.GetAxis("Vertical") * MoveSpeed;

        cameraInput.x = Input.GetAxis("Mouse X") * CamSens.x;
        cameraInput.y = Input.GetAxis("Mouse Y");

        transform.Rotate(0, cameraInput.x, 0);
        CamX = cam.transform.localEulerAngles.x - cameraInput.y * CamSens.y;

        cam.localEulerAngles = new Vector3(CamX, 0, 0);
        RB.velocity = transform.forward * PlayerInput.y + transform.right * PlayerInput.x + transform.up * RB.velocity.y;
    }
}
