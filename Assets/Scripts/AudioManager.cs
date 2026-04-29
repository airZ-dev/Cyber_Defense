using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField][UnityEngine.Range(0f, 1f)] private float musicVolume = 0.5f;

    [Header("SFX")]
    [SerializeField] private AudioClip shootSoundBase;
    [SerializeField] private AudioClip shootSoundFreeze;
    [SerializeField] private AudioClip shootSoundShootgn;
    [SerializeField] private AudioClip towerBuildSound;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip defaultButtonClickSound;
    [SerializeField] private AudioClip backButtonClickSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip currencySound;
    [SerializeField] private AudioClip clickOnTile;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip startOrEndWave;
    [SerializeField] private AudioClip hitBase;

    [UnityEngine.Range(0f, 1f)][SerializeField] private float sfxVolume = 0.7f;


    [Header("Buttons array")]
    [SerializeField] private Button[] defaultButtons;
    [SerializeField] private Button[] backButtons;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           // DontDestroyOnLoad(gameObject);
            InitAudioSources();
        }
        //else
        //{
        //    Destroy(gameObject);
        //}

        if(defaultButtons != null && defaultButtons.Length > 0)
        {
            foreach(var elem in defaultButtons){
                elem.onClick.AddListener(() => AudioManager.Instance?.PlayDefaultButtonClick());
            }
        }


        if (backButtons != null && backButtons.Length > 0)
        {
            foreach (var elem in backButtons)
            {
                elem.onClick.AddListener(() => AudioManager.Instance?.PlayBackButtonClick());
            }
        }
    }

    private void InitAudioSources()
    {
        // Источник для музыки
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;

        // Источник для звуковых эффектов
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;
        sfxSource.playOnAwake = false;

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    // Воспроизведение звука (один выстрел)
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // Воспроизведение с перегрузкой для громкости
    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
    }

    // Установка громкости музыки
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    // Установка громкости звуков
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // Переключение паузы музыки (для паузы игры)
    public void PauseMusic(bool pause)
    {
        if (musicSource != null)
        {
            if (pause) musicSource.Pause();
            else musicSource.UnPause();
        }
    }

    // Специфические методы для удобства
    public void PlayShootBase() => PlaySFX(shootSoundBase);
    public void PlayShootfreeze() => PlaySFX(shootSoundFreeze);
    public void PlayShootShootgn() => PlaySFX(shootSoundShootgn);
    public void PlayBuildTower() => PlaySFX(towerBuildSound);
    public void PlayEnemyHit() => PlaySFX(enemyHitSound);
    public void PlayEnemyDeath() => PlaySFX(enemyDeathSound);
    public void PlayDefaultButtonClick() => PlaySFX(defaultButtonClickSound); 
    public void PlayBackButtonClick() => PlaySFX(backButtonClickSound); 
    public void PlayGameOver() => PlaySFX(gameOverSound);
    public void PlayVictory() => PlaySFX(victorySound);
    public void PlayCurrency() => PlaySFX(currencySound);
    public void ClickOnTile() => PlaySFX(clickOnTile);
    public void ErrorSound() => PlaySFX(errorSound);
    public void StartOrEndWave() => PlaySFX(startOrEndWave);
    public void PlayHitBase() => PlaySFX(hitBase);
}
