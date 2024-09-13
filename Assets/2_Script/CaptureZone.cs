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
    public Slider captureGauge; // ���� ������ UI
    public float captureTime = 3f; // ���ɿ� �ʿ��� �ð�
    public float maxCaptureGauge = 100f; // �ִ� ������ ������
    public float captureGaugeSpeed = 10f; // ������ ������ ���� �ӵ�

    private float currentCaptureTime = 0f; // ���� ���� �ð�
    private float currentCaptureGauge = 0;
    private bool isCapturing = false; // �÷��̾ ���� ������ Ȯ��
    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        captureGauge.value = 0;
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
                photonView.RPC("OnCaptureComplete", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
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
            Debug.Log("���� ��ҵ�.");
        }
    }

    // ���� �Ϸ� �� ȣ��Ǵ� �Լ�
    [PunRPC]
    public void OnCaptureComplete(string playerName)
    {
        Debug.Log(playerName + "���� ������ �Ϸ��߽��ϴ�!");

        // ���� �� ������ ���� ����
        StartCoroutine(FillCaptureGauge());
    }

    // ���� �������� ä��� �ڷ�ƾ
    IEnumerator FillCaptureGauge()
    {
        while (currentCaptureGauge < maxCaptureGauge)
        {
            currentCaptureGauge += captureGaugeSpeed * Time.deltaTime;
            captureGauge.value = currentCaptureGauge / maxCaptureGauge;
            yield return null;
        }

        // �������� �� á���� �¸�
        photonView.RPC("EndGame", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
    }

    // ���� ���� ����
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
