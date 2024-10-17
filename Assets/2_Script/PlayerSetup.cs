using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;  // 가상 카메라

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

        // 만약 이 오브젝트가 현재 클라이언트의 플레이어라면
        if (photonView.IsMine)
        {
            // 씬에 있는 Virtual Camera를 찾음
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

            // 카메라 설정
            if (virtualCamera != null)
            {
                virtualCamera.Follow = parts[4].transform;
            }
            else
            {
                Debug.LogError("Cinemachine Virtual Camera를 찾을 수 없습니다.");
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
