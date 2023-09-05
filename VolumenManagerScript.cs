using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumenManagerScript : MonoBehaviour
{
    [SerializeField] Slider GameVolumenSlider, MusicVolumenSlider;
    public static float GameVolumen = 1f, MusicVolumen = 1f;

    public GameObject GameMusic, LobbyMusic;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("MusicVolumen"))
        {
            PlayerPrefs.SetFloat("MusicVolumen", 1f);
        }
        if (!PlayerPrefs.HasKey("GameVolumen"))
        {
            PlayerPrefs.SetFloat("GameVolumen", 1f);
        }
        Load(); changeVolumen();
    }

    public void changeVolumen()
    {
        MusicVolumen = MusicVolumenSlider.value;
        GameVolumen = GameVolumenSlider.value;

        if (GameMusic) GameMusic.GetComponent<AudioSource>().volume = MusicVolumen;
        if (LobbyMusic) LobbyMusic.GetComponent<AudioSource>().volume = MusicVolumen;

        Save();
    }

    private void Load()
    {
        MusicVolumen = PlayerPrefs.GetFloat("MusicVolumen");
        GameVolumen = PlayerPrefs.GetFloat("GameVolumen");

        MusicVolumenSlider.value = MusicVolumen;
        GameVolumenSlider.value = GameVolumen;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("MusicVolumen", MusicVolumen);
        PlayerPrefs.SetFloat("GameVolumen", GameVolumen);
    }
}
