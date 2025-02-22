using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverScript : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void NewGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
