using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZone : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("UI")]
    public GameObject captureKeyImage; // 점령할 때 띄울 UI
    public Slider player1CaptureGauge; // 플레이어 1의 점령 게이지
    public Slider player2CaptureGauge; // 플레이어 2의 점령 게이지

    [Header("수치")]
    public float captureTime = 3f; // 점령에 필요한 시간
    public float maxCaptureGauge = 100f; // 최대 점령 게이지
    public float captureGaugeSpeed = 10f; // 점령 후 게이지 증가 속도

    private float currentCaptureTime; // 현재 점령 중 시간
    private float player1CurrentCaptureGauge = 0f; // 플레이어 1의 현재 점령 게이지
    private float player2CurrentCaptureGauge = 0f; // 플레이어 2의 현재 점령 게이지

    private bool isPlayer1Capturing = false; // 플레이어 1이 점령 중인지 여부
    private bool isPlayer2Capturing = false; // 플레이어 2가 점령 중인지 여부

    private bool inCaptureZone = false; // 로컬 플레이어가 점령지 안에 있는지 여부
    private PhotonView pv;
    private PhotonView playerPhotonView; // 점령지에 들어온 플레이어의 PhotonView

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
            }

            if (currentCaptureTime >= captureTime)
            {
                if (playerPhotonView.Owner == PhotonNetwork.LocalPlayer)
                {
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

                    // 점령이 완료된 후 RPC 호출을 통해 상태 전파
                    pv.RPC("SyncCaptureState", RpcTarget.All, player1CurrentCaptureGauge, player2CurrentCaptureGauge, isPlayer1Capturing, isPlayer2Capturing);
                }
            }
        }

        // 슬라이더 게이지는 항상 업데이트
        player1CaptureGauge.value = player1CurrentCaptureGauge / maxCaptureGauge;
        player2CaptureGauge.value = player2CurrentCaptureGauge / maxCaptureGauge;

        StartCoroutine(FillCaptureGauge());
    }

    IEnumerator FillCaptureGauge()
    {
        // 플레이어 1이 점령 중일 때
        if (isPlayer1Capturing && !isPlayer2Capturing)
        {
            player1CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player2CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // 플레이어 2의 게이지 감소
        }
        // 플레이어 2가 점령 중일 때
        else if (isPlayer2Capturing && !isPlayer1Capturing)
        {
            player2CurrentCaptureGauge += Time.deltaTime * captureGaugeSpeed;
            player1CurrentCaptureGauge -= Time.deltaTime * captureGaugeSpeed; // 플레이어 1의 게이지 감소
        }

        // 게이지가 0 이하로 내려가지 않도록 제한
        player1CurrentCaptureGauge = Mathf.Clamp(player1CurrentCaptureGauge, 0f, maxCaptureGauge);
        player2CurrentCaptureGauge = Mathf.Clamp(player2CurrentCaptureGauge, 0f, maxCaptureGauge);

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 점령지에 들어오면
        PhotonView enteringPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (enteringPlayerPhotonView != null && enteringPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer)
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

        if (exitingPlayerPhotonView != null && exitingPlayerPhotonView.Owner == PhotonNetwork.LocalPlayer)
        {
            captureKeyImage.SetActive(false);
            inCaptureZone = false;
            currentCaptureTime = 0f; // 나갈 때 점령 시간 초기화
            playerPhotonView = null; // 플레이어가 나가면 초기화
        }
    }

    // Photon SerializeView: 동기화 기능
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 현재 점령 게이지와 상태를 전송 (서버가 클라이언트로 전송)
            stream.SendNext(player1CurrentCaptureGauge);
            stream.SendNext(player2CurrentCaptureGauge);
            stream.SendNext(isPlayer1Capturing);
            stream.SendNext(isPlayer2Capturing);
        }
        else
        {
            // 클라이언트가 서버로부터 데이터를 수신하여 로컬 상태를 업데이트
            player1CurrentCaptureGauge = (float)stream.ReceiveNext();
            player2CurrentCaptureGauge = (float)stream.ReceiveNext();
            isPlayer1Capturing = (bool)stream.ReceiveNext();
            isPlayer2Capturing = (bool)stream.ReceiveNext();
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
