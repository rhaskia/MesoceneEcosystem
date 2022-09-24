using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    AudioSource audio;
    public AudioClip[] sfx;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySFX(string name)
    {
        foreach (var item in sfx)
        {
            if (item.name.ToLower() == name.ToLower())
            {
                audio.clip = item;
                audio.Play();
            }
        }
    }
}
