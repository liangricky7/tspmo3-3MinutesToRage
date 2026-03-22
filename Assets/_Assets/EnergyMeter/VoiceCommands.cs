using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommands : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private string[] keywords = new string[] { "grapple", "come here", "net ann yahoo please", "hollow purple", "smash" };

    private GrapplePull grapplePull;
    private ShootBehavior shootBehavior;
    private BatBehavior batBehavior;

    private AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip grappleSound;
    public AudioClip smashSound;
    public AudioClip shootSound;

    void Start()
    {
        grapplePull   = FindObjectOfType<GrapplePull>();
        shootBehavior = FindObjectOfType<ShootBehavior>();
        batBehavior   = FindObjectOfType<BatBehavior>();
        audioSource   = GetComponent<AudioSource>();

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

        if (args.text == "hollow purple" || args.text == "net ann yahoo please")
        {
            shootBehavior.Fire();
            PlaySound(shootSound);
        }

        if (args.text == "smash")
        {
            batBehavior.Attack();
            PlaySound(smashSound);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
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