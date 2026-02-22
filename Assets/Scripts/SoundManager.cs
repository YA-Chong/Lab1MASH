using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip pickSound;
    public AudioClip dropSound;
    public AudioClip dieSound;

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPick()
    {
        if (pickSound)
            audioSource.PlayOneShot(pickSound);
    }

    public void PlayDrop()
    {
        if (dropSound)
            audioSource.PlayOneShot(dropSound);
    }

    public void PlayDie()
    {
        if (dieSound)
            audioSource.PlayOneShot(dieSound);
    }
}
