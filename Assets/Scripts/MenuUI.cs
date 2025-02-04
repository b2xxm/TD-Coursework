using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject selection;
    [SerializeField] private SpawnSchedule easySchedule;
    [SerializeField] private SpawnSchedule mediumSchedule;
    [SerializeField] private SpawnSchedule hardSchedule;

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
        Spawner.schedule = easySchedule;
        SceneManager.LoadSceneAsync("Game");
    }

    public void Medium()
    {
        Spawner.schedule = mediumSchedule;
        SceneManager.LoadSceneAsync("Game");
    }

    public void Hard()
    {
        Spawner.schedule = easySchedule;
        SceneManager.LoadSceneAsync("Game");
    }
}
