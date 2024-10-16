using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class InGameManager : MonoBehaviourPunCallbacks
{
    // Transform으로 스폰 위치를 오브젝트에서 받아오기
    [SerializeField] private Transform player1SpawnTransform;  // 1번 플레이어 스폰 오브젝트
    [SerializeField] private Transform player2SpawnTransform;  // 2번 플레이어 스폰 오브젝트

    private PhotonView pv;

    private Vector3 spawnPosition;

    public GameObject winPanel;
    public GameObject deathUI;
    public Text winnerText;

    public Map[] map;

    public static InGameManager instance;

    private void Awake()
    {
        instance = this;

        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int random = Random.Range(0, map.Length);
            pv.RPC("randomMap", RpcTarget.All, random);
        }
    }

    private void SpawnPlayer()
    {

        // 플레이어 리스트에서 자신의 순서에 맞는 스폰 위치를 지정
        if (PhotonNetwork.IsMasterClient) // 방을 만든 플레이어
        {
            spawnPosition = player1SpawnTransform.position; // 1번 플레이어 스폰 위치
        }
        else // 방에 참여한 플레이어
        {
            spawnPosition = player2SpawnTransform.position; // 2번 플레이어 스폰 위치
        }

        // 플레이어 생성 (PhotonNetwork.Instantiate 사용)
        PhotonNetwork.Instantiate("Head", spawnPosition, Quaternion.identity);
    }

    [PunRPC]
    public void randomMap(int random)
    {
        map[random].gameObject.SetActive(true);

        switch (map[random].type)
        {
            case MapType.Move :
                //무브
                player1SpawnTransform = map[random].spawnPoint1.transform;
                player2SpawnTransform = map[random].spawnPoint2.transform;
                break;
            case MapType.Lava :
                //라바
                player1SpawnTransform = map[random].spawnPoint1.transform;
                player2SpawnTransform = map[random].spawnPoint2.transform;
                break;
            case MapType.Ice :
                //희발련
                player1SpawnTransform = map[random].spawnPoint1.transform;
                player2SpawnTransform = map[random].spawnPoint2.transform;
                break;
        }

        SpawnPlayer();
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        winPanel.SetActive(true);
        winnerText.text = winner;
    }
}
