using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("MainPanel")]
    public GameObject mainPanel;
    public InputField nicknameInput;
    public Text networkSituation;

    [Header("LobbyPanel")]
    public GameObject lobbyPanel;
    public InputField roomInput;

    [Header("RoomPanel")]
    public GameObject roomPanel;


    public List<RoomInfo> myList = new List<RoomInfo>();

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

    #region 서버연결
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
        lobbyPanel.SetActive(true);
        mainPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        Debug.Log("로비 접속 완료");
        myList.Clear();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("연결끊김");
        lobbyPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    #endregion


    #region 방
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomInput.text == "" ? "Room" + Random.Range(0, 100) : roomInput.text, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomInput.text = "";
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    #endregion


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i]))
                {
                    myList.Add(roomList[i]);
                }
                else
                {
                    myList[myList.IndexOf(roomList[i])] = roomList[i];
                }
            }
            else if (myList.IndexOf(roomList[i]) != -1)
            {
                myList.RemoveAt(myList.IndexOf(roomList[i]));
            }
        }
    }
}