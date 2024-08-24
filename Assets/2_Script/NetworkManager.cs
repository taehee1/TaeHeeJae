using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("MainPanel")]
    public InputField nicknameInput;
    public Text networkSituation;

    [Header("LobbyPanel")]
    public GameObject lobbyPanel;
    public InputField roomInput;

    [Header("RoomPanel")]

    public static NetworkManager instance;

    private void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        Screen.SetResolution(960, 540, false);
        instance = this;
        nicknameInput.text = null;
    }

    private void Update()
    {
        networkSituation.text = PhotonNetwork.NetworkClientState.ToString();
    }

    //��������
    public void Connect()
    {
        if (nicknameInput.text != null)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("�г����� �Է����ּ���");
        }
    }

    //��������Ϸ�
    public override void OnConnectedToMaster()
    {
        print("��������Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("�κ�� ���� �õ�");
            //�κ񿬰�
            PhotonNetwork.JoinLobby();
        }
    }

    //�κ񿬰�Ϸ�
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("�κ� ���� �Ϸ�");
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("�������");
    }
}