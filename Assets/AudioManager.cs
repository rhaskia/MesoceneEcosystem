using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource player;

    public AudioClip[] audiolist;

    public float delay = 5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<AudioSource>();

        StartCoroutine("ManageAudio");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ManageAudio()
    {
        player.clip = audiolist[Random.Range(0, audiolist.Length)];
        player.Play();

        yield return new WaitUntil(() => !player.isPlaying);

        yield return new WaitForSeconds(delay);

        StartCoroutine("ManageAudio");
    }
}
