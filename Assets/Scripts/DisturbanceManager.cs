using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DisturbType { sound, movement }

[System.Serializable]
public class Disturbance
{
    public Vector3 position;

    public float range;
    public float strength;
    public float decrease;

    public DisturbType type;
}

public class DisturbanceManager : MonoBehaviour
{
    public DisturbanceManager Instance;

    public Disturbance[] sounds;
    public Disturbance[] movements;

    //Singleton
    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
