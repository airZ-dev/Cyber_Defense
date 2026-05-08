using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;




    private void Start()
    {
        
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        }
    }

    private void OnDestroy()
    {
        if (musicSlider != null) PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        if (sfxSlider != null) PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }
}
