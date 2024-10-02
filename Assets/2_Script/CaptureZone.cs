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
    public Slider player1CaptureGauge; // 플레이어 1의 점령 게이지
    public Slider player2CaptureGauge; // 플레이어 2의 점령 게이지
    public float captureTime = 3f; // 점령에 필요한 시간
    public float maxCaptureGauge = 100f; // 최대 점령 후 게이지
    public float captureGaugeSpeed = 10f; // 점령 후 게이지 증가 속도

    private float currentCaptureTime = 0f; // 현재 점령 시간
    private float player1CurrentCaptureGauge = 0;
    private float player2CurrentCaptureGauge = 0;
    private bool isCapturing = false; // 플레이어가 점령 중인지 확인
    private PhotonView photonView;
    private string capturingPlayerName = ""; // 현재 점령 중인 플레이어 이름

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        player1CaptureGauge.value = 0;
        player2CaptureGauge.value = 0;
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
                photonView.RPC("StartCapture", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
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

            if (capturingPlayerName == PhotonNetwork.LocalPlayer.NickName)
            {
                capturingPlayerName = ""; // 점령 중인 플레이어 초기화
                photonView.RPC("StopCapture", RpcTarget.AllBuffered);
            }

            Debug.Log("점령 취소됨.");
        }
    }

    // 점령 시작 함수
    [PunRPC]
    public void StartCapture(string playerName)
    {
        if (capturingPlayerName == "" || capturingPlayerName == playerName)
        {
            capturingPlayerName = playerName; // 점령 시작한 플레이어 설정
            StartCoroutine(FillCaptureGauge(playerName));
            Debug.Log(playerName + "님이 점령을 시작했습니다!");
        }
    }

    // 점령 게이지를 채우는 코루틴
    IEnumerator FillCaptureGauge(string playerName)
    {
        while (capturingPlayerName == playerName)
        {
            if (playerName == PhotonNetwork.LocalPlayer.NickName)
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    player1CurrentCaptureGauge += captureGaugeSpeed * Time.deltaTime;
                    photonView.RPC("UpdateCaptureGauge", RpcTarget.AllBuffered, playerName, player1CurrentCaptureGauge);

                    if (player1CurrentCaptureGauge >= maxCaptureGauge)
                    {
                        photonView.RPC("EndGame", RpcTarget.AllBuffered, playerName);
                        yield break;
                    }
                }
                else
                {
                    player2CurrentCaptureGauge += captureGaugeSpeed * Time.deltaTime;
                    photonView.RPC("UpdateCaptureGauge", RpcTarget.AllBuffered, playerName, player2CurrentCaptureGauge);

                    if (player2CurrentCaptureGauge >= maxCaptureGauge)
                    {
                        photonView.RPC("EndGame", RpcTarget.AllBuffered, playerName);
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    // 점령 게이지 업데이트 함수
    [PunRPC]
    public void UpdateCaptureGauge(string playerName, float captureGaugeValue)
    {
        if (playerName == PhotonNetwork.LocalPlayer.NickName)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1CaptureGauge.value = captureGaugeValue / maxCaptureGauge;
            }
            else
            {
                player2CaptureGauge.value = captureGaugeValue / maxCaptureGauge;
            }
        }
    }

    // 점령 중단 함수
    [PunRPC]
    public void StopCapture()
    {
        capturingPlayerName = ""; // 점령 중인 플레이어 초기화
        StopAllCoroutines(); // 게이지 채우는 코루틴 중단
        Debug.Log("점령이 중단되었습니다.");
    }

    // 게임 종료 함수
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
