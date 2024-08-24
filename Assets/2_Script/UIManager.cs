using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject roomListPanel;

    public void RoomList()
    {
        mainPanel.SetActive(false);
        roomListPanel.SetActive(true);
    }
}
