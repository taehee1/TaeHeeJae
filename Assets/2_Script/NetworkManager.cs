using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject connectPanel;
    public GameObject connectingTxt;
    public GameObject connectedTxt;


    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("서버연결완료");
    }
}
