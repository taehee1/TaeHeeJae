using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Network : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("서버연결완료");

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("로비로 접속 시도");
            //로비연결
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.NickName = "";
        Debug.Log("로비 접속 완료");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom("Test", roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 접속 완료");
    }
}
