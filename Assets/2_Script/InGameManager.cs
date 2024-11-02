using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Cinemachine;
using System.Linq;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public CinemachineVirtualCamera virtualCamera;  // 가상 카메라
    // Transform으로 스폰 위치를 오브젝트에서 받아오기
    [SerializeField] private Transform player1SpawnTransform;  // 1번 플레이어 스폰 오브젝트
    [SerializeField] private Transform player2SpawnTransform;  // 2번 플레이어 스폰 오브젝트

    private PhotonView pv;

    private Vector3 spawnPosition;
    private bool isPlayerSpawned = false;  // 플레이어가 스폰되었는지 여부

    public GameObject blackZone;
    [Header("UI")]
    public GameObject startUI;
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;

    public GameObject winPanel;
    public GameObject deathUI;
    public TextMeshProUGUI respawnText;
    public Text winnerText;

    public Map[] map;

    public static InGameManager instance;

    private void Awake()
    {
        instance = this;

        pv = GetComponent<PhotonView>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        foreach (var m in map)
        {
            m.gameObject.SetActive(false);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            int random = Random.Range(0, map.Length);
            pv.RPC("randomMap", RpcTarget.All, random);
        }


        string player1Name = PhotonNetwork.MasterClient.NickName; // 마스터 클라이언트의 닉네임
        string player2Name = "Waiting for Player..."; // 기본값 설정

        // 방에 있는 플레이어의 수 확인
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 마스터 클라이언트가 아닌 플레이어의 닉네임 가져오기
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.MasterClient) // 마스터 클라이언트를 제외하고
                {
                    player2Name = player.NickName; // 상대 플레이어의 닉네임
                    break; // 첫 번째 상대 플레이어 닉네임만 필요
                }
            }
        }

        // UI 업데이트
        player1NameText.text = player1Name; // 마스터 클라이언트의 닉네임
        player2NameText.text = player2Name; // 상대 플레이어의 닉네임
        startUI.SetActive(true);
    }

    public void SpawnPlayer()
    {
        if (isPlayerSpawned) return;  // 이미 스폰된 경우 중복 방지

        if (PhotonNetwork.IsMasterClient) // 방을 만든 플레이어
        {
            spawnPosition = player1SpawnTransform.position; // 1번 플레이어 스폰 위치
        }
        else // 방에 참여한 플레이어
        {
            spawnPosition = player2SpawnTransform.position; // 2번 플레이어 스폰 위치
        }

        // 플레이어 오브젝트 생성
        GameObject player = PhotonNetwork.Instantiate("Head", spawnPosition, Quaternion.identity);

        // HP 바 생성
        GameObject hpBar = PhotonNetwork.Instantiate("HpCanvas", spawnPosition, Quaternion.identity);

        // HP 바가 플레이어를 따라다니도록 설정
        hpBar.GetComponent<HpUI>().SetTarget(player);
        player.GetComponent<Hp>().hpUI = hpBar.GetComponent<HpUI>().hpImage;

        isPlayerSpawned = true;  // 스폰되었음을 기록
    }

    [PunRPC]
    public void randomMap(int random)
    {
        // 랜덤으로 선택된 맵 활성화
        map[random].gameObject.SetActive(true);

        switch (map[random].type)
        {
            case MapType.Move:
            case MapType.Lava:
            case MapType.Ice:
            case MapType.Push:
            case MapType.Forest:
                player1SpawnTransform = map[random].spawnPoint1.transform;
                player2SpawnTransform = map[random].spawnPoint2.transform;
                virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = map[random].camerazone.GetComponent<PolygonCollider2D>();
                break;
        }
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        winPanel.SetActive(true);
        winnerText.text = winner;
        Invoke("Restart", 3f);
    }

    private void Restart()
    {
        pv.RPC("Restart_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void Restart_RPC()
    {
        PhotonNetwork.LoadLevel("InGame");
    }
}
