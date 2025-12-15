using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Music clips")]
    public AudioClip[] casinoMusicTracks;

    private int currentTrackIndex = 0;
    private bool wasInCasino = false;
    private bool currentlyInCasino = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource.spatialBlend = 0.0f; // Set to 2D sound
            sfxSource.playOnAwake = false;

            musicSource.spatialBlend = 0.0f; // Set to 2D sound
            musicSource.loop = false;
            musicSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        // Pokud máme hrát casino hudbu a aktuální track dohrál, přehraj další
        if (currentlyInCasino && wasInCasino && 
            !musicSource.isPlaying && casinoMusicTracks.Length > 0)
        {
            PlayNextCasinoTrack();
        }
    }

    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
        sfxSource.pitch = 1.0f; // Reset pitch to default
    }

    public void PlayMusic(AudioClip clip, float volume = 0.5f)
    {
        if (clip == null) 
        {
            return;
        }
        
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }
        
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayCasinoMusic(float volume = 0.5f)
    {
        if (casinoMusicTracks == null || casinoMusicTracks.Length == 0
            || (wasInCasino && musicSource.isPlaying))
        {
            return;
        }

        if (wasInCasino && !musicSource.isPlaying)
        {
            currentlyInCasino = true;
            musicSource.UnPause();
            return;
        }

        currentlyInCasino = true;
        wasInCasino = true;
        currentTrackIndex = Random.Range(0, casinoMusicTracks.Length);

        musicSource.clip = casinoMusicTracks[currentTrackIndex];
        musicSource.volume = volume;
        musicSource.loop = false;

        musicSource.Play();
    }

    public void StopCasinoMusic()
    {
        currentlyInCasino = false;
        musicSource.Pause();
    }

    public void PlayNextCasinoTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % casinoMusicTracks.Length;
        
        musicSource.clip = casinoMusicTracks[currentTrackIndex];
        musicSource.Play();
    }
}
