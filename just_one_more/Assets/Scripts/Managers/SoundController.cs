using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.spatialBlend = 0.0f; // Set to 2D sound
            audioSource.playOnAwake = false;
        }
    }

    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
        audioSource.pitch = 1.0f; // Reset pitch to default
    }
}
