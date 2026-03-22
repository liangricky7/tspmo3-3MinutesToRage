using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Attack Sounds")]
    public AudioClip shootSound;
    public AudioClip batSwingSound;
    public AudioClip grappleSound;

    [Header("Movement Sounds")]
    public AudioClip slideSound;
    public AudioClip jumpSound;
    public AudioClip landSound;

    private AudioSource audioSource;
    private FirstPersonController fpc;
    private CharacterController cc;
    private bool wasSliding  = false;
    private bool wasGrounded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fpc         = GetComponent<FirstPersonController>();
        cc          = GetComponent<CharacterController>();
    }

    void Update()
    {
        // shoot
        if (Input.GetMouseButtonDown(1))
            PlaySound(shootSound);

        // bat swing
        if (Input.GetMouseButtonDown(0))
            PlaySound(batSwingSound);

        // grapple
        if (Input.GetKeyDown(KeyCode.R))
            PlaySound(grappleSound);

        // slide
        if (fpc._isSliding && !wasSliding)
            PlaySound(slideSound);
        wasSliding = fpc._isSliding;

        // jump
        if (Input.GetButtonDown("Jump"))
            PlaySound(jumpSound);

        // land
        if (!wasGrounded && cc.isGrounded)
            PlaySound(landSound);
        wasGrounded = cc.isGrounded;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}