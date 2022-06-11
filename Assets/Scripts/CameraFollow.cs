
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

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (!pv.IsMine) Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (!pv.IsMine) return;

        //Camera Follow
        Vector3 desiredPosition = target.position + (ZoomOffset * zoom);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        //Zooming 
        zoom = Mathf.Clamp(zoom + (zoomSpeed * Input.mouseScrollDelta.y), minMaxZoom.x, minMaxZoom.y);
    }
}