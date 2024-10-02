using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InGameManager : MonoBehaviourPunCallbacks
{
    // Transform���� ���� ��ġ�� ������Ʈ���� �޾ƿ���
    [SerializeField] private Transform player1SpawnTransform;  // 1�� �÷��̾� ���� ������Ʈ
    [SerializeField] private Transform player2SpawnTransform;  // 2�� �÷��̾� ���� ������Ʈ


    private void Awake()
    {
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
}
