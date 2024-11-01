using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;
    private PhotonView pv;

    public float speed = 10f;
    public float damage = 10f;

    private bool isDestroyed = false; // 파괴 여부 플래그

    Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Invoke("AutoRemove", 6f);
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    public void Shoot(Vector2 direction)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D가 할당되지 않았습니다!");
            return;
        }
        Debug.Log(direction);

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return; // 이미 파괴된 경우 처리하지 않음

        Debug.Log("충돌 감지: " + collision.gameObject.name); // 충돌한 객체의 이름 출력

        PhotonView photonView = collision.gameObject.GetComponentInParent<PhotonView>();

        // 공격
        if (collision.gameObject.CompareTag("Player") && pv.IsMine != photonView.IsMine)
        {
            Debug.Log("상대에게 맞음: " + collision.gameObject.name); // 상대에게 맞았을 때 출력

            // 부모 객체에서 Hp 스크립트를 찾기
            Hp hp = collision.gameObject.GetComponentInParent<Hp>();
            if (hp != null)
            {
                hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                Debug.Log("데미지 전송: " + damage); // 데미지 전송 시 출력
            }

            if (pv.IsMine && !isDestroyed)
            {
                isDestroyed = true; // 파괴 플래그 설정
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            if (pv.IsMine && !isDestroyed)
            {
                isDestroyed = true; // 파괴 플래그 설정
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void AutoRemove()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}