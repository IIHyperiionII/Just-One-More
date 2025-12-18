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

    [Header("Music clips")]
    public AudioClip[] casinoMusicTracks;
    private int currentTrackIndex = 0;
    private bool wasInCasino = false;
    private bool currentlyInCasino = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If the existing instance has no tracks but this one does, copy them over
            if ((Instance.casinoMusicTracks == null || Instance.casinoMusicTracks.Length == 0) &&
                casinoMusicTracks != null && casinoMusicTracks.Length > 0)
            {
                Instance.casinoMusicTracks = casinoMusicTracks;
            }
            
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if(sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        
        if(musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        sfxSource.spatialBlend = 0.0f;
        sfxSource.playOnAwake = false;
        
        musicSource.spatialBlend = 0.0f;
        musicSource.loop = false;
        musicSource.playOnAwake = false;
    }
    
    private void Update()
    {
        if (currentlyInCasino && wasInCasino && 
            musicSource != null && !musicSource.isPlaying && casinoMusicTracks.Length > 0)
        {
            PlayNextCasinoTrack();
        }
    }
    
    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        if (clip == null || sfxSource == null) return;
        
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
        sfxSource.pitch = 1.0f;
    }
    
    public void PlayMusic(AudioClip clip, float volume = 0.5f)
    {
        if (clip == null || musicSource == null) return;
        
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }
    
    public void StopMusic()
    {
        if(musicSource == null) return;
        musicSource.Stop();
    }
    
    public void PlayCasinoMusic(float volume = 0.5f)
    {
        if (musicSource == null)
        {
            return;
        }
        
        if (casinoMusicTracks == null)
        {
            return;
        }
        
        if (casinoMusicTracks.Length == 0)
        {
            return;
        }
        
        if (wasInCasino && musicSource.isPlaying)
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
        AudioClip selectedClip = casinoMusicTracks[currentTrackIndex];
        
        if (selectedClip == null)
        {
            return;
        }
        
        musicSource.clip = selectedClip;
        musicSource.volume = volume;
        musicSource.loop = false;
        musicSource.Play();
    }
    
    public void StopCasinoMusic()
    {
        if(musicSource == null)
        {
            return;
        }
        
        currentlyInCasino = false;
        musicSource.Pause();
    }
    
    public void PlayNextCasinoTrack()
    {
        if(musicSource == null || casinoMusicTracks == null || casinoMusicTracks.Length == 0)
        {
            return;
        }
        
        currentTrackIndex = (currentTrackIndex + 1) % casinoMusicTracks.Length;
        AudioClip nextClip = casinoMusicTracks[currentTrackIndex];
        
        if (nextClip == null)
        {
            return;
        }
        
        musicSource.clip = nextClip;
        musicSource.Play();
    }
}
