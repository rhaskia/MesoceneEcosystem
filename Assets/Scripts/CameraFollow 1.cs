using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes the camera follow the player
public class CameraFollow : MonoBehaviour
{
    [Header("CameraMove")]
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 ZoomOffset;

    public Vector2 minMaxZoom;
    public float zoomSpeed;
    public float zoom = 2;

    public float mouseSensitivity = 3.0f;

    public float rotationY;
    public float rotationX;

    public Vector3 currentRotation;
    public Vector3 smoothVelocity = Vector3.zero;

    public float smoothTime = 0.2f;

    public Vector2 rotationXMinMax = new Vector2(-40, 40);

    float playerSize;

    void Start()
    {
        playerSize = target.localScale.x;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;

            //Apply clamping for x rotation 
            rotationX = Mathf.Clamp(rotationX, rotationXMinMax.x, rotationXMinMax.y);

            Vector3 nextRotation = new Vector3(rotationX, rotationY);

            //Apply damping between rotation changes
            currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
            transform.localEulerAngles = currentRotation;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //Zooming 
        var size = zoom * playerSize;
        zoom = Mathf.Clamp(zoom + (zoomSpeed * Input.mouseScrollDelta.y), minMaxZoom.x, minMaxZoom.y);
        target.localScale = Vector3.one * zoom * playerSize;
        target.position = new Vector3(target.position.x, target.position.y + ((zoom * playerSize) - size) / 2, target.position.z);

        //Substract forward vector of the GameObject to point its forward vector to the target
        transform.position = target.position - transform.forward * zoom;
    }
}