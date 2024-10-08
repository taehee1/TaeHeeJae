using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  // ���� ī�޶�

    public GameObject hand;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
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
                virtualCamera.Follow = transform;
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
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                hand.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                hand.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        else
        {
            if (!pv.IsMine)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                hand.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                hand.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
    }
}
