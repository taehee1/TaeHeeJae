using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;

    Enemy enemy;

    public float speed = 10f;
    public float damage = 10f;

    Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        rb.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //공격
        if (collision.gameObject.GetComponent<PhotonView>().IsMine != true && gameObject.tag == "Player")
        {
            gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
        }
        else
        {

        }
    }
}