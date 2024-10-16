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
        PhotonNetwork.Instantiate("Head", spawnPosition, Quaternion.identity);
    }

    [PunRPC]
    public void randomMap(int random)
    {
        map[random].gameObject.SetActive(true);

        switch (map[random].type)
        {
            case MapType.Move :
                //����
                player1SpawnTransform = map[random].spawnPoint1.transform;
                player2SpawnTransform = map[random].spawnPoint2.transform;
                break;
            case MapType.Lava :
                //���
                player1SpawnTransform = map[random].spawnPoint1.transform;
                player2SpawnTransform = map[random].spawnPoint2.transform;
                break;
            case MapType.Ice :
                //��߷�
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
