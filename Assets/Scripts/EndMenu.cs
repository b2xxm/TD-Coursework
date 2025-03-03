using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
}
