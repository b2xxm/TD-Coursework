using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
