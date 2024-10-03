using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZone : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("UI")]
    public GameObject captureKeyImage; // ������ �� ��� UI
    public Slider player1CaptureGauge; // �÷��̾� 1�� ���� ������
    public Slider player2CaptureGauge; // �÷��̾� 2�� ���� ������

    [Header("��ġ")]
    public float captureTime = 3f; // ���ɿ� �ʿ��� �ð�
    public float maxCaptureGauge = 100f; // �ִ� ���� ������
    public float captureGaugeSpeed = 10f; // ���� �� ������ ���� �ӵ�

    private float currentCaptureTime; // ���� ���� �� �ð�
    private float player1CurrentCaptureGauge = 0f; // �÷��̾� 1�� ���� ���� ������
    private float player2CurrentCaptureGauge = 0f; // �÷��̾� 2�� ���� ���� ������

    private bool isPlayer1Capturing = false; // �÷��̾� 1�� ���� ������ ����
    private bool isPlayer2Capturing = false; // �÷��̾� 2�� ���� ������ ����

    private bool inCaptureZone = false; // ���� �÷��̾ ������ �ȿ� �ִ��� ����
    private PhotonView pv;
    private PhotonView playerPhotonView; // �������� ���� �÷��̾��� PhotonView

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        // ���� �÷��̾ ������ �ȿ� ���� ��
        if (inCaptureZone && playerPhotonView != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                currentCaptureTime += Time.deltaTime;
            }

            if (currentCaptureTime >= captureTime)
            {
                if (playerPhotonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        // �÷��̾� 1 (������ Ŭ���̾�Ʈ)�� ���� ����
                        isPlayer1Capturing = true;
                        isPlayer2Capturing = false;
                    }
                    else
                    {
                        // �÷��̾� 2�� ���� ����
                        isPlayer2Capturing = true;
                        isPlayer1Capturing = false;
                    }

                    currentCaptureTime = 0f; // ���� �ð��� �Ϸ�Ǹ� �ʱ�ȭ

                    // ������ �Ϸ�� �� RPC ȣ���� ���� ���� ����
                    pv.RPC("SyncCaptureState", RpcTarget.All, player1CurrentCaptureGauge, player2CurrentCaptureGauge, isPlayer1Capturing, isPlayer2Capturing);
                }
            }
        }

        // �����̴� �������� �׻� ������Ʈ
        player1CaptureGauge.value = player1CurrentCaptureGauge / maxCaptureGauge;
        player2CaptureGauge.value = player2CurrentCaptureGauge / maxCaptureGauge;

        StartCoroutine(FillCaptureGauge());
    }

    IEnumerator FillCaptureGauge()
    {
        // �÷��̾� 1�� ���� ���� ��
        if (isPlayer1Capturing && !isPlayer2Capturing)
        {
            player1CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player2CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // �÷��̾� 2�� ������ ����
        }
        // �÷��̾� 2�� ���� ���� ��
        else if (isPlayer2Capturing && !isPlayer1Capturing)
        {
            player2CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player1CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // �÷��̾� 1�� ������ ����
        }

        // �������� 0 ���Ϸ� �������� �ʵ��� ����
        player1CurrentCaptureGauge = Mathf.Clamp(player1CurrentCaptureGauge, 0f, maxCaptureGauge);
        player2CurrentCaptureGauge = Mathf.Clamp(player2CurrentCaptureGauge, 0f, maxCaptureGauge);

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �������� ������
        PhotonView enteringPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (enteringPlayerPhotonView != null && enteringPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer)
        {
            captureKeyImage.SetActive(true);
            inCaptureZone = true;
            playerPhotonView = enteringPlayerPhotonView; // ���� �÷��̾��� PhotonView ����
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾ ���������� ������
        PhotonView exitingPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (exitingPlayerPhotonView != null && exitingPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer)
        {
            captureKeyImage.SetActive(false);
            inCaptureZone = false;
            currentCaptureTime = 0f; // ���� �� ���� �ð� �ʱ�ȭ
            playerPhotonView = null; // �÷��̾ ������ �ʱ�ȭ
        }
    }

    // Photon SerializeView: ����ȭ ���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� ���� �������� ���¸� ���� (������ Ŭ���̾�Ʈ�� ����)
            stream.SendNext(player1CurrentCaptureGauge);
            stream.SendNext(player2CurrentCaptureGauge);
            stream.SendNext(isPlayer1Capturing);
            stream.SendNext(isPlayer2Capturing);
        }
        else
        {
            // Ŭ���̾�Ʈ�� �����κ��� �����͸� �����Ͽ� ���� ���¸� ������Ʈ
            player1CurrentCaptureGauge = (float)stream.ReceiveNext();
            player2CurrentCaptureGauge = (float)stream.ReceiveNext();
            isPlayer1Capturing = (bool)stream.ReceiveNext();
            isPlayer2Capturing = (bool)stream.ReceiveNext();
        }
    }

    // RPC�� ���� ���� ���� ����ȭ
    [PunRPC]
    private void SyncCaptureState(float p1Gauge, float p2Gauge, bool p1Capturing, bool p2Capturing)
    {
        player1CurrentCaptureGauge = p1Gauge;
        player2CurrentCaptureGauge = p2Gauge;
        isPlayer1Capturing = p1Capturing;
        isPlayer2Capturing = p2Capturing;
    }
}
