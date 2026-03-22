using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceCommands : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private string[] keywords = new string[] { "Grapple", "Come Here", "Kill Yourself", "Hollow Purple", "Smash" };

    private GrapplePull grapplePull;
    private ShootBehavior shootBehavior;
    private BatBehavior batBehavior;

    void Start()
    {
        grapplePull   = FindObjectOfType<GrapplePull>();
        shootBehavior = FindObjectOfType<ShootBehavior>();
        batBehavior   = FindObjectOfType<BatBehavior>();

        keywordRecognizer = new KeywordRecognizer(keywords, ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Voice command heard: " + args.text);

        if (args.text == "Grapple" || args.text == "Come Here")
            grapplePull.StartGrapple();

        if (args.text == "Hollow Purple" || args.text == "Kill Yourself")
            shootBehavior.Fire();

        if (args.text == "Smash")
            batBehavior.Attack();
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