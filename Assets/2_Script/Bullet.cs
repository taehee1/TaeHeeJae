using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;

    Enemy enemy;

    [Header("�ɷ�")]
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
            Debug.LogError("Rigidbody2D�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        rb.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //����
        if (collision.gameObject.tag == "Enemy")
        {

        }

        bounceCount--;
        if (bounceCount == 0)
        {
            Destroy(gameObject);
            return;
        }

        // �ݻ� ������ �ּ� ������ �� �ֽ��ϴ�.
        // var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
        // rb.velocity = direction * speed;
    }
}