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
        print("��������Ϸ�");

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("�κ�� ���� �õ�");
            //�κ񿬰�
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.NickName = "";
        Debug.Log("�κ� ���� �Ϸ�");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom("Test", roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");
    }
}
