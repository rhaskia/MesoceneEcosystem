
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("CameraMove")]
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 ZoomOffset;

    public Vector2 minMaxZoom;
    public float zoomSpeed;
    public float zoom = 2;
    PhotonView pv;

    public float mouseSensitivity = 3.0f;

    public float rotationY;
    public float rotationX;

    public Vector3 currentRotation;
    public Vector3 smoothVelocity = Vector3.zero;

    public float smoothTime = 0.2f;

    public Vector2 rotationXMinMax = new Vector2(-40, 40);


    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (!pv.IsMine) Destroy(gameObject);
    }

    void Update()
    {
        if (!pv.IsMine) return;

        Cursor.visible = false;

        //Zooming 
        zoom = Mathf.Clamp(zoom + (zoomSpeed * Input.mouseScrollDelta.y), minMaxZoom.x, minMaxZoom.y);

        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;

        //Apply clamping for x rotation 
        rotationX = Mathf.Clamp(rotationX, rotationXMinMax.x, rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(rotationX, rotationY);

        //Apply damping between rotation changes
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
        transform.localEulerAngles = currentRotation;

        //Substract forward vector of the GameObject to point its forward vector to the target
        transform.position = target.position - transform.forward * zoom;

        //Set target's rotation
        RoomManager.Instance.rotation = rotationY;
    }
}