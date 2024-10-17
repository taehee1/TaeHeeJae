using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviourPunCallbacks
{
    public GameObject menu;

    private bool isOpen;

    public void ContinueBtn()
    {
        menu.SetActive(false);
        isOpen = false;
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public void MainSceneBtn()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Main");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menu != null)
        {
            if (isOpen)
            {
                menu.SetActive(false);
                isOpen = false;
            }
            else if (!isOpen)
            {
                menu.SetActive(true);
                isOpen = true;
            }
        }
    }
}