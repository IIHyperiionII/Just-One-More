using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    [Header("Music clips")]
    public AudioClip[] casinoMusicTracks;

    [Header("UI Sound Clips")]
    public AudioClip buttonClickSound;
    
    [Header("Enemy Death Sounds")]
    public AudioClip office1DeathSound;
    public AudioClip office2DeathSound;
    public AudioClip office3DeathSound;
    public AudioClip toilet1DeathSound;
    public AudioClip toilet2DeathSound;
    public AudioClip toilet3DeathSound;
    public AudioClip bossOffice1DeathSound;
    public AudioClip bossOffice2DeathSound;
    public AudioClip bossOffice3DeathSound;

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
            
            // Copy mixer groups if the existing instance doesn't have them
            if (Instance.musicMixerGroup == null && musicMixerGroup != null)
            {
                Instance.musicMixerGroup = musicMixerGroup;
                if (Instance.musicSource != null)
                {
                    Instance.musicSource.outputAudioMixerGroup = musicMixerGroup;
                }
            }
            
            if (Instance.sfxMixerGroup == null && sfxMixerGroup != null)
            {
                Instance.sfxMixerGroup = sfxMixerGroup;
                if (Instance.sfxSource != null)
                {
                    Instance.sfxSource.outputAudioMixerGroup = sfxMixerGroup;
                }
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
        if (sfxMixerGroup != null)
        {
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }
        
        musicSource.spatialBlend = 0.0f;
        musicSource.loop = false;
        musicSource.playOnAwake = false;
        if (musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }
    }
    
    private void Update()
    {
        if (currentlyInCasino && wasInCasino && 
            musicSource != null && !musicSource.isPlaying && casinoMusicTracks.Length > 0)
        {
            PlayNextCasinoTrack();
        }
    }
    
    // Override for code calls
    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        if (clip == null || sfxSource == null) return;
        
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
        sfxSource.pitch = 1.0f;
    }

    // === UI SOUNDS (bez parametrů pro UnityEvents) ===
    public void PlayButtonClick()
    {
        if (buttonClickSound != null)
            PlaySound(buttonClickSound, 0.3f, 1.0f);
    }
    
    // === ENEMY DEATH SOUNDS (bez parametrů pro UnityEvents) ===
    public void PlayOffice1Death()
    {
        if (office1DeathSound != null)
            PlaySound(office1DeathSound, 0.3f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayOffice2Death()
    {
        if (office2DeathSound != null)
            PlaySound(office2DeathSound, 0.3f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayOffice3Death()
    {
        if (office3DeathSound != null)
            PlaySound(office3DeathSound, 0.3f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayToilet1Death()
    {
        if (toilet1DeathSound != null)
            PlaySound(toilet1DeathSound, 0.3f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayToilet2Death()
    {
        if (toilet2DeathSound != null)
            PlaySound(toilet2DeathSound, 0.3f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayToilet3Death()
    {
        if (toilet3DeathSound != null)
            PlaySound(toilet3DeathSound, 0.3f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayBossOffice1Death()
    {
        if (bossOffice1DeathSound != null)
            PlaySound(bossOffice1DeathSound, 0.3f, Random.Range(0.85f, 1.0f));
    }
    
    public void PlayBossOffice2Death()
    {
        if (bossOffice2DeathSound != null)
            PlaySound(bossOffice2DeathSound, 0.3f, Random.Range(0.85f, 1.0f));
    }
    
    public void PlayBossOffice3Death()
    {
        if (bossOffice3DeathSound != null)
            PlaySound(bossOffice3DeathSound, 0.3f, Random.Range(0.85f, 1.0f));
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }
    
    public void StopMusic()
    {
        if(musicSource == null) return;
        musicSource.Stop();
    }
    
    public void PlayCasinoMusic()
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
