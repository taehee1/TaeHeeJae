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

    public GameObject winPanel;
    public Text winnerText;

    public static InGameManager instance;

    private void Awake()
    {
        instance = this;

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition;

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
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        winPanel.SetActive(true);
        winnerText.text = winner;
    }
}
