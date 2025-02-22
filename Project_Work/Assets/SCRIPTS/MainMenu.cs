using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI difficultyText;

    private int difficultyLevel = 1;

    private void Start()
    {
        UpdateDifficultyText();
    }

    public void IncreaseDifficulty()
    {
        difficultyLevel = (difficultyLevel % 3) + 1;
        UpdateDifficultyText();
    }

    public void DecreaseDifficulty()
    {
        difficultyLevel = (difficultyLevel - 1 <= 0) ? 3 : difficultyLevel - 1;
        UpdateDifficultyText();
    }

    private void UpdateDifficultyText()
    {
        string[] levels = { "EASY", "MEDIUM", "HARD" };
        difficultyText.text = levels[difficultyLevel - 1];
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void AlghorithmVieer()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
