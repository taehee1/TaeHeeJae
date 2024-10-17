using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  // ���� ī�޶�

    public GameObject[] parts;

    private PhotonView pv;
    public static PlayerSetup instance;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        instance = this;
    }

    private void Start()
    {
        CameraSetup();
        PlayerColorSetup();
    }

    private void CameraSetup()
    {
        PhotonView photonView = GetComponent<PhotonView>();

        // ���� �� ������Ʈ�� ���� Ŭ���̾�Ʈ�� �÷��̾���
        if (photonView.IsMine)
        {
            // ���� �ִ� Virtual Camera�� ã��
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

            // ī�޶� ����
            if (virtualCamera != null)
            {
                virtualCamera.Follow = parts[4].transform;
            }
            else
            {
                Debug.LogError("Cinemachine Virtual Camera�� ã�� �� �����ϴ�.");
            }
        }
    }

    private void PlayerColorSetup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (pv.IsMine)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
        else
        {
            if (!pv.IsMine)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }
        }
    }
}
