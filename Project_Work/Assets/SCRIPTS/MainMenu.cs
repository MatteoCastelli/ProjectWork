using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI difficultyText;

    public static int SelectedDifficultyLevel { get; private set; } = 1;
    public static int SelectedMazeSize { get; private set; } = 10;

    private void Start()
    {
        UpdateDifficultyText();
    }

    public void IncreaseDifficulty()
    {
        SelectedDifficultyLevel = (SelectedDifficultyLevel % 3) + 1;
        UpdateDifficultyText();
    }

    public void DecreaseDifficulty()
    {
        SelectedDifficultyLevel = (SelectedDifficultyLevel - 1 <= 0) ? 3 : SelectedDifficultyLevel - 1;
        UpdateDifficultyText();
    }

    private void UpdateDifficultyText()
    {
        string[] levels = { "EASY", "MEDIUM", "HARD" };
        difficultyText.text = levels[SelectedDifficultyLevel - 1];

        switch (SelectedDifficultyLevel)
        {
            case 1:
                SelectedMazeSize = 10;
                break;
            case 2:
                SelectedMazeSize = 15;
                break;
            case 3:
                SelectedMazeSize = 20;
                break;
        }
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