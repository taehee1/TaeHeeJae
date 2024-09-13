using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;

    private Rigidbody2D rb;
    public Image hpUI;

    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    public Vector3 spawnPosition;  // 시작 위치

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        currentHp = maxHp;
        spawnPosition = transform.position;
    }


    private void Update()
    {
        Move();
        Jump();
    }


    private void Move()
    {
        if (pv.IsMine)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
        }
    }
    
    //변수동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hpUI.fillAmount);
        }
        else
        {
            hpUI.fillAmount = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        currentHp -= damage;
        hpUI.fillAmount = currentHp / maxHp;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 체력을 초기화하고 리스폰 위치로 이동
        currentHp = maxHp;
        transform.position = spawnPosition;

        // 추가적인 리스폰 로직 (무적시간, 애니메이션 등)
        photonView.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // 필요하다면 다른 클라이언트에서 플레이어의 리스폰 상태를 처리
    }
}
