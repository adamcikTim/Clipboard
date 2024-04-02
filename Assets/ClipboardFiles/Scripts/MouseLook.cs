using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouseLook : MonoBehaviour{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;       // hides the mouse cursor to avoid any game disruptions
    }
    void Update(){
        MovementMethod();
    }
    // method handles the mouse movement and calculates math necessary for player to
    // move mouse around and look around the world
    // in this case we have to clamp the rotation along the x axis, due to
    // look restrictions as otherwise we'd end up having a free look which would
    // result in odd gameplay
    void MovementMethod(){
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation,-90f,90f);
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}