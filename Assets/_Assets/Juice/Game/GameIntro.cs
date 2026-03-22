using System.Collections;
using UnityEngine;

public class GameIntro : MonoBehaviour
{
    [Header("Countdown Clips")]
    public AudioClip clip3;
    public AudioClip clip2;
    public AudioClip clip1;
    public AudioClip clipReady;

    [Header("Music")]
    public AudioClip music;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(1f);

        audioSource.PlayOneShot(clip3);
        yield return new WaitForSeconds(1f);

        audioSource.PlayOneShot(clip2);
        yield return new WaitForSeconds(1f);

        audioSource.PlayOneShot(clip1);
        yield return new WaitForSeconds(1f);

        audioSource.PlayOneShot(clipReady);
        yield return new WaitForSeconds(1f);

        // music starts after countdown finishes
        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.Play();
    }
}