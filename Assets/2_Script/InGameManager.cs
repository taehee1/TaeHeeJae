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
    public CinemachineVirtualCamera virtualCamera;  // ���� ī�޶�
    // Transform���� ���� ��ġ�� ������Ʈ���� �޾ƿ���
    [SerializeField] private Transform player1SpawnTransform;  // 1�� �÷��̾� ���� ������Ʈ
    [SerializeField] private Transform player2SpawnTransform;  // 2�� �÷��̾� ���� ������Ʈ

    private PhotonView pv;

    private Vector3 spawnPosition;
    private bool isPlayerSpawned = false;  // �÷��̾ �����Ǿ����� ����

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
        if (isPlayerSpawned) return;  // �̹� ������ ��� �ߺ� ����

        if (PhotonNetwork.IsMasterClient) // ���� ���� �÷��̾�
        {
            spawnPosition = player1SpawnTransform.position; // 1�� �÷��̾� ���� ��ġ
        }
        else // �濡 ������ �÷��̾�
        {
            spawnPosition = player2SpawnTransform.position; // 2�� �÷��̾� ���� ��ġ
        }

        PhotonNetwork.Instantiate("Head", spawnPosition, Quaternion.identity);
        isPlayerSpawned = true;  // �����Ǿ����� ���
    }

    [PunRPC]
    public void randomMap(int random)
    {
        // ��� �� ��Ȱ��ȭ
        foreach (var m in map)
        {
            m.gameObject.SetActive(false);
        }

        // �������� ���õ� �� Ȱ��ȭ
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
