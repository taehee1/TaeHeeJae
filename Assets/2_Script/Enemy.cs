using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float hp = 1000f;

    public GameObject dmgTxt;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        DamageDisplay(damage);
    }

    private void DamageDisplay(float damage)
    {
        dmgTxt.SetActive(true);
        dmgTxt.GetComponent<Text>().text = $"-{damage}";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(10f);
        }
    }
}
