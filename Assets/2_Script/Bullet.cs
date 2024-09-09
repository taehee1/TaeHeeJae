using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;

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
            Debug.LogError("Rigidbody2D�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        rb.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PhotonView photonView = collision.gameObject.GetComponent<PhotonView>();

        //����
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PhotonView>().IsMine == false)
        {
            PlayerScript playerScript = gameObject.GetComponent<PlayerScript>();

            // PlayerScript�� �ִ��� Ȯ��
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else
        {
            
        }
    }
}