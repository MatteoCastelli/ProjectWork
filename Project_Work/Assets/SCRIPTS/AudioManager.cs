using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Image buttonImage;
    private AudioSource[] audioSources;
    private bool isMuted = false;

    void Awake()
    {
        UpdateButtonImage();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateAudioSources();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAudioSources();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateButtonImage();

        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateAudioSources();
    }

    private void UpdateAudioSources()
    {
        audioSources = FindObjectsOfType<AudioSource>();

        foreach (var audioSource in audioSources)
        {
            if (audioSource != null)
            {
                audioSource.mute = isMuted;
            }
        }
    }


    private void UpdateButtonImage()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isMuted ? pressedSprite : normalSprite;
        }
    }
}