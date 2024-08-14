using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;

    Enemy enemy;

    [Header("능력")]
    public float speed = 10f;
    public int bounceCount = 1;
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
        if (collision.gameObject.tag == "Enemy")
        {

        }

        bounceCount--;
        if (bounceCount == 0)
        {
            Destroy(gameObject);
            return;
        }

        // 반사 로직을 주석 해제할 수 있습니다.
        // var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
        // rb.velocity = direction * speed;
    }
}