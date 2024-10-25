using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

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
        if (PhotonNetwork.IsMasterClient)
        {
            int random = Random.Range(0, map.Length);
            pv.RPC("randomMap", RpcTarget.All, random);
        }

        Invoke("SpawnPlayer", 2);
    }

    private void SpawnPlayer()
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

        PhotonNetwork.Instantiate("Head", spawnPosition, Quaternion.identity);
        isPlayerSpawned = true;  // 스폰되었음을 기록
    }

    [PunRPC]
    public void randomMap(int random)
    {
        // 모든 맵 비활성화
        foreach (var m in map)
        {
            m.gameObject.SetActive(false);
        }

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
