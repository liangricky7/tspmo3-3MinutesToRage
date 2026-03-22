using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommands : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private string[] keywords = new string[] { "grapple", "come here", "bang", "hollow purple", "smash" };

    private GrapplePull grapplePull;
    private ShootBehavior shootBehavior;
    private BatBehavior batBehavior;
    private HollowPurpleEffect hollowPurpleEffect;

    private AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip grappleSound;
    public AudioClip smashSound;
    public AudioClip shootSound;
    public AudioClip hollowPurpleSound;

    void Start()
    {
        grapplePull        = FindObjectOfType<GrapplePull>();
        shootBehavior      = FindObjectOfType<ShootBehavior>();
        batBehavior        = FindObjectOfType<BatBehavior>();
        hollowPurpleEffect = GetComponent<HollowPurpleEffect>();
        audioSource        = GetComponent<AudioSource>();

        keywordRecognizer = new KeywordRecognizer(keywords, ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Voice command heard: " + args.text);

        if (args.text == "grapple" || args.text == "come here")
        {
            grapplePull.StartGrapple();
            PlaySound(grappleSound);
        }

        if (args.text == "bang")
        {
            shootBehavior.Fire(); // plays shoot sound normally
            PlaySound(shootSound);
        }

        if (args.text == "hollow purple")
        {
            shootBehavior.Fire(false); // no shoot sound
            PlaySound(hollowPurpleSound);
            hollowPurpleEffect.Trigger();
        }

        if (args.text == "smash")
        {
            batBehavior.Attack();
            PlaySound(smashSound);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void OnDestroy()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
}