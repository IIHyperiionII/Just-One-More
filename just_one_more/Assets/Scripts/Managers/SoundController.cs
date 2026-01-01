using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public AudioSource sfxSource;
    public AudioSource casinoMusicSource;
    public AudioSource gameMusicSource;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    [Header("Music clips")]
    public AudioClip[] casinoMusicTracks;
    public AudioClip[] gameMusicTracks;
    public AudioClip mainMenuMusic;

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

    private int currentCasinoTrackIndex = 0;
    private int currentGameTrackIndex = 0;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Copy tracks if the existing instance doesn't have them
            if ((Instance.casinoMusicTracks == null || Instance.casinoMusicTracks.Length == 0) &&
                casinoMusicTracks != null && casinoMusicTracks.Length > 0)
            {
                Instance.casinoMusicTracks = casinoMusicTracks;
            }
            
            if ((Instance.gameMusicTracks == null || Instance.gameMusicTracks.Length == 0) &&
                gameMusicTracks != null && gameMusicTracks.Length > 0)
            {
                Instance.gameMusicTracks = gameMusicTracks;
            }
            
            if (Instance.mainMenuMusic == null && mainMenuMusic != null)
            {
                Instance.mainMenuMusic = mainMenuMusic;
            }
            
            // Copy mixer groups if the existing instance doesn't have them
            if (Instance.musicMixerGroup == null && musicMixerGroup != null)
            {
                Instance.musicMixerGroup = musicMixerGroup;
                if (Instance.casinoMusicSource != null)
                {
                    Instance.casinoMusicSource.outputAudioMixerGroup = musicMixerGroup;
                }
                if (Instance.gameMusicSource != null)
                {
                    Instance.gameMusicSource.outputAudioMixerGroup = musicMixerGroup;
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
            
            // Copy enemy death sounds if the existing instance doesn't have them
            if (Instance.office1DeathSound == null && office1DeathSound != null)
                Instance.office1DeathSound = office1DeathSound;
            if (Instance.office2DeathSound == null && office2DeathSound != null)
                Instance.office2DeathSound = office2DeathSound;
            if (Instance.office3DeathSound == null && office3DeathSound != null)
                Instance.office3DeathSound = office3DeathSound;
            if (Instance.toilet1DeathSound == null && toilet1DeathSound != null)
                Instance.toilet1DeathSound = toilet1DeathSound;
            if (Instance.toilet2DeathSound == null && toilet2DeathSound != null)
                Instance.toilet2DeathSound = toilet2DeathSound;
            if (Instance.toilet3DeathSound == null && toilet3DeathSound != null)
                Instance.toilet3DeathSound = toilet3DeathSound;
            if (Instance.bossOffice1DeathSound == null && bossOffice1DeathSound != null)
                Instance.bossOffice1DeathSound = bossOffice1DeathSound;
            if (Instance.bossOffice2DeathSound == null && bossOffice2DeathSound != null)
                Instance.bossOffice2DeathSound = bossOffice2DeathSound;
            if (Instance.bossOffice3DeathSound == null && bossOffice3DeathSound != null)
                Instance.bossOffice3DeathSound = bossOffice3DeathSound;
            if (Instance.buttonClickSound == null && buttonClickSound != null)
                Instance.buttonClickSound = buttonClickSound;
            
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize SFX source
        if(sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.spatialBlend = 0.0f;
        sfxSource.playOnAwake = false;
        if (sfxMixerGroup != null)
        {
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }
        
        // Initialize Casino Music source
        if(casinoMusicSource == null)
        {
            casinoMusicSource = gameObject.AddComponent<AudioSource>();
        }
        casinoMusicSource.spatialBlend = 0.0f;
        casinoMusicSource.loop = false;
        casinoMusicSource.playOnAwake = false;
        if (musicMixerGroup != null)
        {
            casinoMusicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        
        // Initialize Game Music source
        if(gameMusicSource == null)
        {
            gameMusicSource = gameObject.AddComponent<AudioSource>();
        }
        gameMusicSource.spatialBlend = 0.0f;
        gameMusicSource.loop = false;
        gameMusicSource.playOnAwake = false;
        if (musicMixerGroup != null)
        {
            gameMusicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        
        // Start playing main menu music automatically
        PlayMainMenuMusic();
    }
    
    private void Update()
    {
        // Auto-play next casino track when current one finishes
        if (casinoMusicSource != null && !casinoMusicSource.isPlaying && 
            casinoMusicTracks != null && casinoMusicTracks.Length > 0)
        {
            // Check if we should be playing casino music (not paused)
            if (casinoMusicSource.clip != null && casinoMusicSource.time == 0f)
            {
                // Track finished naturally, play next
                PlayNextCasinoTrack();
            }
        }
        
        // Auto-play next game track when current one finishes
        if (gameMusicSource != null && !gameMusicSource.isPlaying && 
            gameMusicTracks != null && gameMusicTracks.Length > 0)
        {
            // Check if we should be playing game music (not paused)
            if (gameMusicSource.clip != null && gameMusicSource.time == 0f)
            {
                // Track finished naturally, play next
                PlayNextGameTrack();
            }
        }
    }
    
    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        if (clip == null || sfxSource == null) return;
        
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
        sfxSource.pitch = 1.0f;
    }

    // === UI SOUNDS ===
    public void PlayButtonClick()
    {
        if (buttonClickSound != null)
            PlaySound(buttonClickSound, 0.3f, 1.0f);
    }
    
    // === ENEMY DEATH SOUNDS ===
    public void PlayOffice1Death()
    {
        if (office1DeathSound != null)
            PlaySound(office1DeathSound, 0.4f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayOffice2Death()
    {
        if (office2DeathSound != null)
            PlaySound(office2DeathSound, 0.25f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayOffice3Death()
    {
        if (office3DeathSound != null)
            PlaySound(office3DeathSound, 0.2f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayToilet1Death()
    {
        if (toilet1DeathSound != null)
            PlaySound(toilet1DeathSound, 0.3f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayToilet2Death()
    {
        if (toilet2DeathSound != null)
            PlaySound(toilet2DeathSound, 0.3f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayToilet3Death()
    {
        if (toilet3DeathSound != null)
            PlaySound(toilet3DeathSound, 0.3f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayBossOffice1Death()
    {
        if (bossOffice1DeathSound != null)
            PlaySound(bossOffice1DeathSound, 0.3f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayBossOffice2Death()
    {
        if (bossOffice2DeathSound != null)
            PlaySound(bossOffice2DeathSound, 0.3f, Random.Range(0.8f, 1.2f));
    }
    
    public void PlayBossOffice3Death()
    {
        if (bossOffice3DeathSound != null)
            PlaySound(bossOffice3DeathSound, 0.3f, Random.Range(0.8f, 1.2f));
    }
    
    // === CASINO MUSIC ===
    public void PlayCasinoMusic()
    {
        if (casinoMusicSource == null || casinoMusicTracks == null || casinoMusicTracks.Length == 0)
        {
            return;
        }
        
        // If already playing, just unpause
        if (casinoMusicSource.clip != null && !casinoMusicSource.isPlaying)
        {
            casinoMusicSource.UnPause();
            return;
        }
        
        // If already playing, don't restart
        if (casinoMusicSource.isPlaying)
        {
            return;
        }
        
        // Start new track
        currentCasinoTrackIndex = Random.Range(0, casinoMusicTracks.Length);
        AudioClip selectedClip = casinoMusicTracks[currentCasinoTrackIndex];
        
        if (selectedClip == null)
        {
            return;
        }
        
        casinoMusicSource.clip = selectedClip;
        casinoMusicSource.Play();
    }
    
    public void StopCasinoMusic()
    {
        if(casinoMusicSource == null)
        {
            return;
        }
        
        casinoMusicSource.Pause();
    }
    
    private void PlayNextCasinoTrack()
    {
        if(casinoMusicSource == null || casinoMusicTracks == null || casinoMusicTracks.Length == 0)
        {
            return;
        }
        
        currentCasinoTrackIndex = (currentCasinoTrackIndex + 1) % casinoMusicTracks.Length;
        AudioClip nextClip = casinoMusicTracks[currentCasinoTrackIndex];
        
        if (nextClip == null)
        {
            return;
        }
        
        casinoMusicSource.clip = nextClip;
        casinoMusicSource.Play();
    }
    
    // === GAME MUSIC ===
    public void PlayGameMusic()
    {
        if (gameMusicSource == null || gameMusicTracks == null || gameMusicTracks.Length == 0)
        {
            return;
        }
        
        // If already playing, just unpause
        if (gameMusicSource.clip != null && !gameMusicSource.isPlaying)
        {
            gameMusicSource.UnPause();
            return;
        }
        
        // If already playing, don't restart
        if (gameMusicSource.isPlaying)
        {
            return;
        }
        
        // Start new track
        currentGameTrackIndex = Random.Range(0, gameMusicTracks.Length);
        AudioClip selectedClip = gameMusicTracks[currentGameTrackIndex];
        
        if (selectedClip == null)
        {
            return;
        }
        
        gameMusicSource.clip = selectedClip;
        gameMusicSource.Play();
    }
    
    public void StopGameMusic()
    {
        if(gameMusicSource == null)
        {
            return;
        }
        
        gameMusicSource.Pause();
    }
    
    private void PlayNextGameTrack()
    {
        if(gameMusicSource == null || gameMusicTracks == null || gameMusicTracks.Length == 0)
        {
            return;
        }
        
        currentGameTrackIndex = (currentGameTrackIndex + 1) % gameMusicTracks.Length;
        AudioClip nextClip = gameMusicTracks[currentGameTrackIndex];
        
        if (nextClip == null)
        {
            return;
        }
        
        gameMusicSource.clip = nextClip;
        gameMusicSource.Play();
    }
    
    // === MAIN MENU MUSIC ===
    public void PlayMainMenuMusic()
    {
        if (gameMusicSource == null || mainMenuMusic == null)
        {
            return;
        }
        
        if (gameMusicSource.clip == mainMenuMusic && gameMusicSource.isPlaying)
        {
            return;
        }
        
        gameMusicSource.Stop();
        gameMusicSource.clip = mainMenuMusic;
        gameMusicSource.loop = true;
        gameMusicSource.Play();
    }
    
    public void StopMainMenuMusic()
    {
        if (gameMusicSource == null)
        {
            return;
        }
        
        gameMusicSource.Stop();
        gameMusicSource.loop = false;
    }
    
    // === LEGACY METHODS (pro kompatibilitu) ===
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || gameMusicSource == null) return;
        
        if (gameMusicSource.clip == clip && gameMusicSource.isPlaying) return;
        
        gameMusicSource.Stop();
        gameMusicSource.clip = clip;
        gameMusicSource.Play();
    }
    
    public void StopMusic()
    {
        if(gameMusicSource == null) return;
        gameMusicSource.Stop();
    }
}
