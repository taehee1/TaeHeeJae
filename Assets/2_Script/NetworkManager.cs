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

    #region ��������
    //��������
    public void Connect()
    {
        if (nicknameInput.text != "")
        {
            PhotonNetwork.ConnectUsingSettings();
            connectLoadingPanel.SetActive(true);
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
        connectLoadingPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        mainPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        Debug.Log("�κ� ���� �Ϸ�");
        myList.Clear();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("�������");
        lobbyPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    #endregion


    #region ��
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
        Debug.Log("������ �Ϸ�");
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
            // ���� ��� �÷��̾ Ư�� ������ ��ȯ
            PhotonNetwork.LoadLevel("InGame"); // "GameScene"�� ���ϴ� �� �̸����� ��ü
        }
    }
    #endregion


    #region �渮��Ʈ
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (myList.ContainsKey(room.Name))
                {
                    myList.Remove(room.Name);  // ���� �����Ǹ� ����Ʈ���� ����
                }
            }
            else
            {
                if (myList.ContainsKey(room.Name))
                {
                    myList[room.Name] = room;  // �̹� �ִ� �� ���� ������Ʈ
                }
                else
                {
                    myList.Add(room.Name, room);  // ���ο� �� �߰�
                }
            }
        }
        UpdateRoomList();  // �� ����Ʈ UI ������Ʈ
    }

    void UpdateRoomList()
    {
        Debug.Log("�� ����Ʈ UI ������Ʈ ����");
        Debug.Log($"���� myList�� �� ����: {myList.Count}");
        foreach (Transform child in roomListContent.transform)
        {
            Destroy(child.gameObject);  // ���� UI �׸�� ����
        }

        foreach (RoomInfo roomInfo in myList.Values)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent.transform);
            roomItem.GetComponentInChildren<Text>().text = roomInfo.Name;

            Debug.Log($"���ο� �� �׸� �߰�: {roomInfo.Name}");

            Button roomButton = roomItem.GetComponent<Button>();
            roomButton.onClick.AddListener(() => JoinRoom(roomInfo.Name));
        }

        Debug.Log("�� ����Ʈ UI ������Ʈ �Ϸ�");
    }

    void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
}