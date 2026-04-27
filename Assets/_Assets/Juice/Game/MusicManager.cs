using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Tracks")]
    public AudioClip calmTrack;
    public AudioClip intenseTrack;

    [Header("Threshold")]
    public float switchThreshold = 100f;

    [Header("Intense Track")]
    public float intenseTrackStartTime = 0f;

    private AudioSource audioSource;
    private bool isIntense = false;
    private float calmTrackTime = 0f;
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
        if (audioSource.clip == calmTrack)
            calmTrackTime = audioSource.time;
        audioSource.clip = clip;
        audioSource.Play();
        if (clip == calmTrack)
            audioSource.time = calmTrackTime;
        else if (clip == intenseTrack)
            audioSource.time = intenseTrackStartTime;
    }
}