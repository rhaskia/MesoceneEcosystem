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

    public int active = 1;

    public void Start()
    {
        xsave = freelook.m_XAxis.m_MaxSpeed;
        ysave = freelook.m_YAxis.m_MaxSpeed;
    }

    public void Awake()
    {
        freelook = GetComponentInChildren<CinemachineFreeLook>();
        originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];

        for (int i = 0; i < freelook.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
        }
    }

    public void Update()
    {
        zoomPercent -= Input.GetAxis("Mouse ScrollWheel") * active;
        zoomPercent = Mathf.Clamp(zoomPercent, minMaxZoom.x, minMaxZoom.y);

        freelook.m_XAxis.m_MaxSpeed = Mathf.Abs(xsave * X.value / 10 * active);
        freelook.m_YAxis.m_MaxSpeed = Mathf.Abs(ysave * Y.value / 10 * active);

        freelook.m_XAxis.m_InvertInput = X.value > 0;
        freelook.m_YAxis.m_InvertInput = Y.value > 0;

        for (int i = 0; i < freelook.m_Orbits.Length; i++)
        {
            freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * zoomPercent;
            freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomPercent;
        }
    }
}
