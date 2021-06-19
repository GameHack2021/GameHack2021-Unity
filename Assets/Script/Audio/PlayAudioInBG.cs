using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioInBG : MonoBehaviour
{
    public AudioClip beginning;
    public AudioClip calm;
    public AudioClip joy;
    public AudioClip sadness;
    AudioSource audioSource;
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(LoopAudio(beginning));
    }

    IEnumerator LoopAudio(AudioClip audioClip)
    {
        audioSource = GetComponent<AudioSource>();
        float length = audioClip.length;
        print(1);
        while (true)
        {
            audioSource.PlayOneShot(audioClip, 0.4f);
            yield return new WaitForSeconds(length);
        }
    }
}
