using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  // 가상 카메라

    private void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();

        // 만약 이 오브젝트가 현재 클라이언트의 플레이어라면
        if (photonView.IsMine)
        {
            // 씬에 있는 Virtual Camera를 찾음
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

            // 카메라 설정
            if (virtualCamera != null)
            {
                virtualCamera.Follow = transform;
            }
            else
            {
                Debug.LogError("Cinemachine Virtual Camera를 찾을 수 없습니다.");
            }
        }
    }
}
