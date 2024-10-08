using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("LoadingPanel")]
    public GameObject connectLoadingPanel;

    [Header("MainPanel")]
    public GameObject mainPanel;
    public InputField nicknameInput;
    public Text networkSituation;

    [Header("LobbyPanel")]
    public GameObject lobbyPanel;
    public Button createRoomButton;
    public InputField roomInput;
    public GameObject roomListContent;
    public GameObject roomListItemPrefab;

    [Header("RoomPanel")]
    public GameObject roomPanel;


    private Dictionary<string, RoomInfo> myList = new Dictionary<string, RoomInfo>();

    public static NetworkManager instance;

    private void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        Screen.SetResolution(1920, 1080, true);
        instance = this;
        nicknameInput.text = null;
    }

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
    }

    private void Update()
    {
        networkSituation.text = PhotonNetwork.NetworkClientState.ToString();
    }

    #region 서버연결
    //서버연결
    public void Connect()
    {
        if (nicknameInput.text != "")
        {
            PhotonNetwork.ConnectUsingSettings();
            connectLoadingPanel.SetActive(true);
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
        connectLoadingPanel.SetActive(false);
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
        if (!string.IsNullOrEmpty(roomInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(roomInput.text, roomOptions);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방참가 완료");
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        CheckPlayerCount();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomInput.text = "";
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckPlayerCount();
    }

    void CheckPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            // 방의 모든 플레이어를 특정 씬으로 전환
            PhotonNetwork.LoadLevel("InGame"); // "GameScene"을 원하는 씬 이름으로 교체
        }
    }
    #endregion


    #region 방리스트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (myList.ContainsKey(room.Name))
                {
                    myList.Remove(room.Name);  // 방이 삭제되면 리스트에서 제거
                }
            }
            else
            {
                if (myList.ContainsKey(room.Name))
                {
                    myList[room.Name] = room;  // 이미 있는 방 정보 업데이트
                }
                else
                {
                    myList.Add(room.Name, room);  // 새로운 방 추가
                }
            }
        }
        UpdateRoomList();  // 방 리스트 UI 업데이트
    }

    void UpdateRoomList()
    {
        Debug.Log("방 리스트 UI 업데이트 시작");
        Debug.Log($"현재 myList의 방 개수: {myList.Count}");
        foreach (Transform child in roomListContent.transform)
        {
            Destroy(child.gameObject);  // 기존 UI 항목들 삭제
        }

        foreach (RoomInfo roomInfo in myList.Values)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent.transform);
            roomItem.GetComponentInChildren<Text>().text = roomInfo.Name;

            Debug.Log($"새로운 방 항목 추가: {roomInfo.Name}");

            Button roomButton = roomItem.GetComponent<Button>();
            roomButton.onClick.AddListener(() => JoinRoom(roomInfo.Name));
        }

        Debug.Log("방 리스트 UI 업데이트 완료");
    }

    void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
}