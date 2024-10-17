using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerUI : MonoBehaviour
{
    public Text winnerText;

    private void Start()
    {
        winnerText.text = Winner.instance.winner;
    }
}
