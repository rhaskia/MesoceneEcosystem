using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Creature;

public class RotTest : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook cam;
    public CAnimation anim;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float rot = cam.m_XAxis.Value;

        if (rot > -135 && rot < -45) { setRot(-90); anim.currentDir = Directions.Back; }
        if (rot > -45 && rot < 45) { setRot(0); anim.currentDir = Directions.Side; anim.flip = false; }
        if (rot > 45 && rot < 135) { setRot(90); anim.currentDir = Directions.Front; }

        if (rot > 135 || rot < -135) { setRot(360); anim.currentDir = Directions.Side; anim.flip = true; }

    }

    void setRot(int r)
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, r, transform.rotation.z));
    }
}
