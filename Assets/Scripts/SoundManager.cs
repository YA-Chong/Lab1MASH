using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip pickSound;  // 拾取士兵音效 [cite: 63]
    public AudioClip dropSound;  // 放下士兵音效 [cite: 64]
    public AudioClip dieSound;   // 坠机死亡音效 [cite: 65]

    private AudioSource audioSource;

    void Awake()
    {
        // 单例模式，方便其他脚本直接调用
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPick()
    {
        if (pickSound) audioSource.PlayOneShot(pickSound);
    }

    public void PlayDrop()
    {
        if (dropSound) audioSource.PlayOneShot(dropSound);
    }

    public void PlayDie()
    {
        if (dieSound) audioSource.PlayOneShot(dieSound);
    }
}
