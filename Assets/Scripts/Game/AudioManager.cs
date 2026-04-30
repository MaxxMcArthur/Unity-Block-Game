// Assets/Scripts/Game/AudioManager.cs
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Sources")]
    public AudioSource levelMusicSource;   // plays level background music
    public AudioSource pauseMusicSource;   // plays pause menu music

    [Header("SFX")]
    public AudioSource sfxSource;          // plays short sound effects
    public AudioClip lineClearClip;        // sound for clearing lines

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --------- Music control ---------

    public void PlayLevelMusic()
    {
        if (pauseMusicSource) pauseMusicSource.Stop();

        if (levelMusicSource && !levelMusicSource.isPlaying)
        {
            levelMusicSource.loop = true;
            levelMusicSource.Play();
        }
    }

    public void StopLevelMusic()
    {
        if (levelMusicSource) levelMusicSource.Stop();
    }

    public void PlayPauseMusic()
    {
        if (levelMusicSource && levelMusicSource.isPlaying)
            levelMusicSource.Pause();

        if (pauseMusicSource && !pauseMusicSource.isPlaying)
        {
            pauseMusicSource.loop = true;
            pauseMusicSource.Play();
        }
    }

    public void StopPauseMusic()
    {
        if (pauseMusicSource) pauseMusicSource.Stop();
        if (levelMusicSource) levelMusicSource.UnPause();
    }

    // --------- SFX ---------

    public void PlayLineClearSfx()
    {
        if (sfxSource && lineClearClip)
        {
            sfxSource.PlayOneShot(lineClearClip);
        }
    }
}
