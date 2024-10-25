using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZone : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject captureKeyImage; // ������ �� ��� UI
    public Image player1CaptureGauge; // �÷��̾� 1�� ���� ������
    public Image player2CaptureGauge; // �÷��̾� 2�� ���� ������

    [Header("��ġ")]
    public float captureTime = 3f; // ���ɿ� �ʿ��� �ð�
    public float maxCaptureGauge = 100f; // �ִ� ���� ������
    public float captureGaugeSpeed = 10f; // ���� �� ������ ���� �ӵ�
    public float gaugeSyncThreshold = 1f; // ����ȭ �� �� ����ϴ� ������ ���� �Ӱ谪

    private float currentCaptureTime; // ���� ���� �� �ð�
    private float player1CurrentCaptureGauge = 0f; // �÷��̾� 1�� ���� ���� ������
    private float player2CurrentCaptureGauge = 0f; // �÷��̾� 2�� ���� ���� ������

    private bool isPlayer1Capturing = false; // �÷��̾� 1�� ���� ������ ����
    private bool isPlayer2Capturing = false; // �÷��̾� 2�� ���� ������ ����

    private bool inCaptureZone = false; // ���� �÷��̾ ������ �ȿ� �ִ��� ����
    private PhotonView pv;
    private PhotonView playerPhotonView; // �������� ���� �÷��̾��� PhotonView

    private float lastSyncTime; // ���������� ����ȭ�� �ð�
    private const float syncInterval = 0.5f; // ����ȭ ���� (0.5��)

    private bool isEnd;

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
                captureKeyImage.GetComponent<Image>().fillAmount = 1 - (currentCaptureTime / captureTime);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                currentCaptureTime = 0;
                captureKeyImage.GetComponent<Image>().fillAmount = 1;
            }

            if (currentCaptureTime >= captureTime)
            {
                if (playerPhotonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    bool previousIsPlayer1Capturing = isPlayer1Capturing;
                    bool previousIsPlayer2Capturing = isPlayer2Capturing;

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

                    // ���°� ������ ���� RPC ȣ��
                    if (previousIsPlayer1Capturing != isPlayer1Capturing || previousIsPlayer2Capturing != isPlayer2Capturing)
                    {
                        pv.RPC("SyncCaptureState", RpcTarget.All, player1CurrentCaptureGauge, player2CurrentCaptureGauge, isPlayer1Capturing, isPlayer2Capturing);
                    }
                }
            }

        }
            if (player1CurrentCaptureGauge >= maxCaptureGauge && !isEnd)
            {
                isEnd = true;
                InGameManager.instance.photonView.RPC("EndGame", RpcTarget.All, PhotonNetwork.MasterClient.NickName);
            }
            else if (player2CurrentCaptureGauge >= maxCaptureGauge && !isEnd)
            {
                isEnd = true;
                Photon.Realtime.Player player2 = PhotonNetwork.PlayerList.First(p => !p.IsMasterClient);
                InGameManager.instance.photonView.RPC("EndGame", RpcTarget.All, player2.NickName);
            }

        // �������� �� ������ ������Ʈ
        UpdateCaptureGauge();

        // �����̴� UI ������Ʈ
        player1CaptureGauge.fillAmount = player1CurrentCaptureGauge / maxCaptureGauge;
        player2CaptureGauge.fillAmount = player2CurrentCaptureGauge / maxCaptureGauge;

        // �ֱ������� ������ ����ȭ
        if (Time.time - lastSyncTime > syncInterval)
        {
            SyncGaugesIfNeeded();
            lastSyncTime = Time.time;
        }
    }

    // ������ ������Ʈ�� �ٷ� ó��
    private void UpdateCaptureGauge()
    {
        if (isPlayer1Capturing && !isPlayer2Capturing)
        {
            player1CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player2CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // �÷��̾� 2�� ������ ����
        }
        else if (isPlayer2Capturing && !isPlayer1Capturing)
        {
            player2CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player1CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // �÷��̾� 1�� ������ ����
        }

        // �������� 0 ���Ϸ� �������� �ʵ��� ����
        player1CurrentCaptureGauge = Mathf.Clamp(player1CurrentCaptureGauge, 0f, maxCaptureGauge);
        player2CurrentCaptureGauge = Mathf.Clamp(player2CurrentCaptureGauge, 0f, maxCaptureGauge);
    }

    // �������� ���� �� �̻� ���̰� �� ���� ����ȭ
    private void SyncGaugesIfNeeded()
    {
        float gaugeDifference1 = Mathf.Abs(player1CurrentCaptureGauge - player1CaptureGauge.fillAmount * maxCaptureGauge);
        float gaugeDifference2 = Mathf.Abs(player2CurrentCaptureGauge - player2CaptureGauge.fillAmount * maxCaptureGauge);

        if (gaugeDifference1 > gaugeSyncThreshold || gaugeDifference2 > gaugeSyncThreshold)
        {
            pv.RPC("SyncCaptureState", RpcTarget.All, player1CurrentCaptureGauge, player2CurrentCaptureGauge, isPlayer1Capturing, isPlayer2Capturing);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �������� ������
        PhotonView enteringPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (enteringPlayerPhotonView != null && enteringPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer && collision.gameObject.tag == "Player")
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

        if (exitingPlayerPhotonView != null && exitingPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer && collision.gameObject.tag == "Player")
        {
            captureKeyImage.SetActive(false);
            inCaptureZone = false;
            currentCaptureTime = 0f; // ���� �� ���� �ð� �ʱ�ȭ
            playerPhotonView = null; // �÷��̾ ������ �ʱ�ȭ
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
