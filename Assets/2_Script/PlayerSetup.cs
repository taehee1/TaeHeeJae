using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  // ���� ī�޶�

    private void Start()
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
}
