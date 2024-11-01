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

    private bool isDestroyed = false; // �ı� ���� �÷���

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
            Debug.LogError("Rigidbody2D�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        Debug.Log(direction);

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return; // �̹� �ı��� ��� ó������ ����

        Debug.Log("�浹 ����: " + collision.gameObject.name); // �浹�� ��ü�� �̸� ���

        PhotonView photonView = collision.gameObject.GetComponentInParent<PhotonView>();

        // ����
        if (collision.gameObject.CompareTag("Player") && pv.IsMine != photonView.IsMine)
        {
            Debug.Log("��뿡�� ����: " + collision.gameObject.name); // ��뿡�� �¾��� �� ���

            // �θ� ��ü���� Hp ��ũ��Ʈ�� ã��
            Hp hp = collision.gameObject.GetComponentInParent<Hp>();
            if (hp != null)
            {
                hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                Debug.Log("������ ����: " + damage); // ������ ���� �� ���
            }

            if (pv.IsMine && !isDestroyed)
            {
                isDestroyed = true; // �ı� �÷��� ����
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            if (pv.IsMine && !isDestroyed)
            {
                isDestroyed = true; // �ı� �÷��� ����
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