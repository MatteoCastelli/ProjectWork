using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float audioVolume;

    public void Menu()
    {
        Time.timeScale = 1;
        AudioListener.volume = audioVolume;
        SceneManager.LoadSceneAsync(0);
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        AudioListener.volume = audioVolume;
        SceneManager.LoadSceneAsync(1);
    }

    public void PauseGame()
    {
        audioVolume = AudioListener.volume;
        Time.timeScale = 0;
        AudioListener.volume = 0;

    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.volume = audioVolume;
    }
}
