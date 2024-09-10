using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour, IShootable
{
    [SerializeField] private float bombTime = 3f;
    [SerializeField] private float speed = 5f;

    public GameObject particle;
    public Rigidbody2D rb;

    private void Start()
    {
        StartCoroutine("Bomb");
    }

    IEnumerator Bomb()
    {
        yield return new WaitForSeconds(bombTime);
        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
}
