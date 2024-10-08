using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class InGameManager : MonoBehaviourPunCallbacks
{
    // Transform���� ���� ��ġ�� ������Ʈ���� �޾ƿ���
    [SerializeField] private Transform player1SpawnTransform;  // 1�� �÷��̾� ���� ������Ʈ
    [SerializeField] private Transform player2SpawnTransform;  // 2�� �÷��̾� ���� ������Ʈ

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

        // �÷��̾� ����Ʈ���� �ڽ��� ������ �´� ���� ��ġ�� ����
        if (PhotonNetwork.IsMasterClient) // ���� ���� �÷��̾�
        {
            spawnPosition = player1SpawnTransform.position; // 1�� �÷��̾� ���� ��ġ
        }
        else // �濡 ������ �÷��̾�
        {
            spawnPosition = player2SpawnTransform.position; // 2�� �÷��̾� ���� ��ġ
        }

        // �÷��̾� ���� (PhotonNetwork.Instantiate ���)
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    }

    [PunRPC]
    public void EndGame(string winner)
    {
        winPanel.SetActive(true);
        winnerText.text = winner;
    }
}
