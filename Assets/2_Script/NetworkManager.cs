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

    //서버연결
    public void Connect()
    {
        if (nicknameInput.text != null)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("닉네임을 입력해주세요");
        }
    }

    //서버연결완료
    public override void OnConnectedToMaster()
    {
        print("서버연결완료");
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("로비로 접속 시도");
            //로비연결
            PhotonNetwork.JoinLobby();
        }
    }

    //로비연결완료
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 접속 완료");
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("연결끊김");
    }
}