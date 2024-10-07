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

    Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
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
        PhotonView photonView = collision.gameObject.GetComponent<PhotonView>();

        //공격
        if (collision.gameObject.tag == "Player" && pv.IsMine != photonView.IsMine)
        {
            photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);

            PhotonNetwork.Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Ground")
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}