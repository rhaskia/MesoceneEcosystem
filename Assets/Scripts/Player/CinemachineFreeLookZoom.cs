using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CinemachineFreeLookZoom : MonoBehaviour
{
    private CinemachineFreeLook freelook;
    private CinemachineFreeLook.Orbit[] originalOrbits;

    public Vector2 minMaxZoom;
    public float zoomPercent;

    public Slider X, Y;
    float xsave, ysave;

    public void Start()
    {
        xsave = freelook.m_XAxis.m_MaxSpeed;
        ysave = freelook.m_YAxis.m_MaxSpeed;
    }

    public void Awake()
    {
        freelook = GetComponentInChildren<CinemachineFreeLook>();
        originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];

        freelook.m_XAxis.m_MaxSpeed = xsave * X.value;
        freelook.m_YAxis.m_MaxSpeed = ysave * Y.value;

        for (int i = 0; i < freelook.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
        }
    }

    public void Update()
    {
        zoomPercent -= Input.GetAxis("Mouse ScrollWheel");
        zoomPercent = Mathf.Clamp(zoomPercent, minMaxZoom.x, minMaxZoom.y);

        for (int i = 0; i < freelook.m_Orbits.Length; i++)
        {
            freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * zoomPercent;
            freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomPercent;
        }
    }
}
