using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject selection;

    public void Awake()
    {
        selection.SetActive(false);
    }

    public void Select()
    {
        selection.SetActive(!selection.activeInHierarchy);
    }

    public void Options()
    {
        selection.SetActive(false);
    }

    public void Quit()
    {
        selection.SetActive(false);
        Application.Quit();
    }

    public void Easy()
    {
        SceneManager.LoadSceneAsync("Easy");
    }
}
