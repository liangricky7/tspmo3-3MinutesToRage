using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Tracks")]
    public AudioClip calmTrack;
    public AudioClip intenseTrack;

    [Header("Threshold")]
    public float switchThreshold = 100f;

    private AudioSource audioSource;
    private bool isIntense = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StartMusic()
    {
        audioSource.clip = calmTrack;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        float energy = EnergyMeter.Instance.energy;

        if (!isIntense && energy >= switchThreshold)
        {
            SwitchTo(intenseTrack);
            isIntense = true;
        }
        else if (isIntense && energy < switchThreshold)
        {
            SwitchTo(calmTrack);
            isIntense = false;
        }
    }

    void SwitchTo(AudioClip clip)
    {
        if (audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}