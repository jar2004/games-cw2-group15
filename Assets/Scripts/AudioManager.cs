using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    public AudioClip gameMusic;
    public AudioClip bossMusic;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("SFX")]
    public AudioClip punchSFX;
    public AudioClip kickSFX;
    public AudioClip gunshotSFX;
    public AudioClip enemyHitSFX;
    public AudioClip enemyDeathSFX;
    public AudioClip playerHurtSFX;
    public AudioClip bossMusicSting;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persists between scenes

        // Create two AudioSources on this GameObject
        // one for music, one for SFX
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;
    }

    public void PlayGameMusic()
    {
        if (gameMusic == null) return;
        PlayMusic(gameMusic);
    }

    public void PlayBossMusic()
    {
        if (bossMusic == null) return;
        PlayMusic(bossMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    public void PlayPunch()     => PlaySFX(punchSFX);
    public void PlayKick()      => PlaySFX(kickSFX);
    public void PlayGunshot()   => PlaySFX(gunshotSFX);
    public void PlayEnemyHit()  => PlaySFX(enemyHitSFX);
    public void PlayEnemyDeath()=> PlaySFX(enemyDeathSFX);
    public void PlayPlayerHurt()=> PlaySFX(playerHurtSFX);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
    
    public void PlaySFXAt(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }
}