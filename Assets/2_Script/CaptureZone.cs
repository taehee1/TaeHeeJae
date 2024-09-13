using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZone : MonoBehaviour
{
    [Header("UI")]
    public GameObject captureKeyImage;
    public Image captureKeyGaugeImage;

    [Header("수치")]
    public Slider captureGauge; // 점령 게이지 UI
    public float captureTime = 3f; // 점령에 필요한 시간
    public float maxCaptureGauge = 100f; // 최대 점령후 게이지
    public float captureGaugeSpeed = 10f; // 점령후 게이지 증가 속도

    private float currentCaptureTime = 0f; // 현재 점령 시간
    private float currentCaptureGauge = 0;
    private bool isCapturing = false; // 플레이어가 점령 중인지 확인
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        captureGauge.value = 0;
    }

    private void Update()
    {
        // 플레이어가 점령 중일 때 E 키를 누르고 있는지 확인
        if (isCapturing && Input.GetKey(KeyCode.E))
        {
            currentCaptureTime += Time.deltaTime;

            // 점령 완료
            if (currentCaptureTime >= captureTime)
            {
                photonView.RPC("OnCaptureComplete", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
                currentCaptureTime = 0;
            }
        }

        if (isCapturing && Input.GetKeyUp(KeyCode.E))
        {
            currentCaptureTime = 0;
        }
    }

    // 플레이어가 점령지에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isCapturing = true;
            captureKeyImage.SetActive(true);
            Debug.Log("점령 시작 가능: E 키를 누르세요.");
        }
    }

    // 플레이어가 점령지에서 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isCapturing = false;
            captureKeyImage.SetActive(false);
            currentCaptureTime = 0; // 점령 시간 초기화
            Debug.Log("점령 취소됨.");
        }
    }

    // 점령 완료 후 호출되는 함수
    [PunRPC]
    public void OnCaptureComplete(string playerName)
    {
        Debug.Log(playerName + "님이 점령을 완료했습니다!");

        // 점령 후 게이지 증가 시작
        StartCoroutine(FillCaptureGauge());
    }

    // 점령 게이지를 채우는 코루틴
    IEnumerator FillCaptureGauge()
    {
        while (currentCaptureGauge < maxCaptureGauge)
        {
            currentCaptureGauge += captureGaugeSpeed * Time.deltaTime;
            captureGauge.value = currentCaptureGauge / maxCaptureGauge;
            yield return null;
        }

        // 게이지가 다 찼으면 승리
        photonView.RPC("EndGame", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
    }

    // 게임 종료 로직
    [PunRPC]
    public void EndGame(string winner)
    {
        Debug.Log("게임 종료! 승자는: " + winner);
        // 게임 종료 처리 (예: 결과 씬 로드)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameOverScene");  // 예시로 GameOverScene 로드
        }
    }
}
