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

    [Header("��ġ")]
    public Slider player1CaptureGauge; // �÷��̾� 1�� ���� ������
    public Slider player2CaptureGauge; // �÷��̾� 2�� ���� ������
    public float captureTime = 3f; // ���ɿ� �ʿ��� �ð�
    public float maxCaptureGauge = 100f; // �ִ� ���� �� ������
    public float captureGaugeSpeed = 10f; // ���� �� ������ ���� �ӵ�

    private float currentCaptureTime = 0f; // ���� ���� �ð�
    private float player1CurrentCaptureGauge = 0;
    private float player2CurrentCaptureGauge = 0;
    private bool isCapturing = false; // �÷��̾ ���� ������ Ȯ��
    private PhotonView photonView;
    private string capturingPlayerName = ""; // ���� ���� ���� �÷��̾� �̸�

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        player1CaptureGauge.value = 0;
        player2CaptureGauge.value = 0;
    }

    private void Update()
    {
        // �÷��̾ ���� ���� �� E Ű�� ������ �ִ��� Ȯ��
        if (isCapturing && Input.GetKey(KeyCode.E))
        {
            currentCaptureTime += Time.deltaTime;

            // ���� �Ϸ�
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

    // �÷��̾ �������� ������ ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isCapturing = true;
            captureKeyImage.SetActive(true);
            Debug.Log("���� ���� ����: E Ű�� ��������.");
        }
    }

    // �÷��̾ ���������� ������ ��
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isCapturing = false;
            captureKeyImage.SetActive(false);
            currentCaptureTime = 0; // ���� �ð� �ʱ�ȭ

            if (capturingPlayerName == PhotonNetwork.LocalPlayer.NickName)
            {
                capturingPlayerName = ""; // ���� ���� �÷��̾� �ʱ�ȭ
                photonView.RPC("StopCapture", RpcTarget.AllBuffered);
            }

            Debug.Log("���� ��ҵ�.");
        }
    }

    // ���� ���� �Լ�
    [PunRPC]
    public void StartCapture(string playerName)
    {
        if (capturingPlayerName == "" || capturingPlayerName == playerName)
        {
            capturingPlayerName = playerName; // ���� ������ �÷��̾� ����
            StartCoroutine(FillCaptureGauge(playerName));
            Debug.Log(playerName + "���� ������ �����߽��ϴ�!");
        }
    }

    // ���� �������� ä��� �ڷ�ƾ
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

    // ���� ������ ������Ʈ �Լ�
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

    // ���� �ߴ� �Լ�
    [PunRPC]
    public void StopCapture()
    {
        capturingPlayerName = ""; // ���� ���� �÷��̾� �ʱ�ȭ
        StopAllCoroutines(); // ������ ä��� �ڷ�ƾ �ߴ�
        Debug.Log("������ �ߴܵǾ����ϴ�.");
    }

    // ���� ���� �Լ�
    [PunRPC]
    public void EndGame(string winner)
    {
        Debug.Log("���� ����! ���ڴ�: " + winner);
        // ���� ���� ó�� (��: ��� �� �ε�)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameOverScene");  // ���÷� GameOverScene �ε�
        }
    }
}
