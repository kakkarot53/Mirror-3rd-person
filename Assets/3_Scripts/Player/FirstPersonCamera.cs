using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 5f;
    [SerializeField]
    private Transform playerBody;

    float xRotation = 0f;

    private float savedMouseSense;

    private void Start()
    {
        savedMouseSense = mouseSensitivity;
    }

    public void LockCam()
    {
        mouseSensitivity = 0;
    }

    public void UnlockCam()
    {
        mouseSensitivity = savedMouseSense;
    }

    void Update()
    {
        //get mouse pos
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //get rotation and limits it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 45f);

        //moves camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //moves body
        playerBody.Rotate(Vector3.up * mouseX);

    }
}
