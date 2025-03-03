using UnityEngine;
using UnityEngine.SceneManagement;

// Class that holds the methods related with the end menu actions
// Methods are assigned to button click events with the inspector
public class EndMenu : MonoBehaviour
{
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;

    public void Show(bool isWin)
    {
        winText.SetActive(isWin);
        loseText.SetActive(!isWin);
        gameObject.SetActive(true);
    }

    public void Menu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadSceneAsync("Game");
    }
}
