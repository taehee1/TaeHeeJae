using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZone : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject captureKeyImage; // 점령할 때 띄울 UI
    public Image player1CaptureGauge; // 플레이어 1의 점령 게이지
    public Image player2CaptureGauge; // 플레이어 2의 점령 게이지

    [Header("수치")]
    public float captureTime = 3f; // 점령에 필요한 시간
    public float maxCaptureGauge = 100f; // 최대 점령 게이지
    public float captureGaugeSpeed = 10f; // 점령 후 게이지 증가 속도
    public float gaugeSyncThreshold = 1f; // 동기화 할 때 사용하는 게이지 차이 임계값

    private float currentCaptureTime; // 현재 점령 중 시간
    private float player1CurrentCaptureGauge = 0f; // 플레이어 1의 현재 점령 게이지
    private float player2CurrentCaptureGauge = 0f; // 플레이어 2의 현재 점령 게이지

    private bool isPlayer1Capturing = false; // 플레이어 1이 점령 중인지 여부
    private bool isPlayer2Capturing = false; // 플레이어 2가 점령 중인지 여부

    private bool inCaptureZone = false; // 로컬 플레이어가 점령지 안에 있는지 여부
    private PhotonView pv;
    private PhotonView playerPhotonView; // 점령지에 들어온 플레이어의 PhotonView

    private float lastSyncTime; // 마지막으로 동기화된 시간
    private const float syncInterval = 0.5f; // 동기화 간격 (0.5초)

    private bool isEnd;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        // 로컬 플레이어가 점령지 안에 있을 때
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
                        // 플레이어 1 (마스터 클라이언트)의 점령 진행
                        isPlayer1Capturing = true;
                        isPlayer2Capturing = false;
                    }
                    else
                    {
                        // 플레이어 2의 점령 진행
                        isPlayer2Capturing = true;
                        isPlayer1Capturing = false;
                    }

                    currentCaptureTime = 0f; // 점령 시간이 완료되면 초기화

                    // 상태가 변했을 때만 RPC 호출
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

        // 게이지를 매 프레임 업데이트
        UpdateCaptureGauge();

        // 슬라이더 UI 업데이트
        player1CaptureGauge.fillAmount = player1CurrentCaptureGauge / maxCaptureGauge;
        player2CaptureGauge.fillAmount = player2CurrentCaptureGauge / maxCaptureGauge;

        // 주기적으로 게이지 동기화
        if (Time.time - lastSyncTime > syncInterval)
        {
            SyncGaugesIfNeeded();
            lastSyncTime = Time.time;
        }
    }

    // 게이지 업데이트를 바로 처리
    private void UpdateCaptureGauge()
    {
        if (isPlayer1Capturing && !isPlayer2Capturing)
        {
            player1CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player2CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // 플레이어 2의 게이지 감소
        }
        else if (isPlayer2Capturing && !isPlayer1Capturing)
        {
            player2CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player1CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // 플레이어 1의 게이지 감소
        }

        // 게이지가 0 이하로 내려가지 않도록 제한
        player1CurrentCaptureGauge = Mathf.Clamp(player1CurrentCaptureGauge, 0f, maxCaptureGauge);
        player2CurrentCaptureGauge = Mathf.Clamp(player2CurrentCaptureGauge, 0f, maxCaptureGauge);
    }

    // 게이지가 일정 값 이상 차이가 날 때만 동기화
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
        // 플레이어가 점령지에 들어오면
        PhotonView enteringPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (enteringPlayerPhotonView != null && enteringPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer && collision.gameObject.tag == "Player")
        {
            captureKeyImage.SetActive(true);
            inCaptureZone = true;
            playerPhotonView = enteringPlayerPhotonView; // 들어온 플레이어의 PhotonView 저장
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어가 점령지에서 나가면
        PhotonView exitingPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (exitingPlayerPhotonView != null && exitingPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer && collision.gameObject.tag == "Player")
        {
            captureKeyImage.SetActive(false);
            inCaptureZone = false;
            currentCaptureTime = 0f; // 나갈 때 점령 시간 초기화
            playerPhotonView = null; // 플레이어가 나가면 초기화
        }
    }

    // RPC를 통한 점령 상태 동기화
    [PunRPC]
    private void SyncCaptureState(float p1Gauge, float p2Gauge, bool p1Capturing, bool p2Capturing)
    {
        player1CurrentCaptureGauge = p1Gauge;
        player2CurrentCaptureGauge = p2Gauge;
        isPlayer1Capturing = p1Capturing;
        isPlayer2Capturing = p2Capturing;
    }
}
