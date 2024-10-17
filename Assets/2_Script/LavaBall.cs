using Photon.Pun;
using UnityEngine;

public class LavaBall : MonoBehaviour
{
    public float jumpForce = 5.0f; // 튀어오르는 힘
    public float interval = 2.0f; // 튀어오르는 간격
    public float damage = 10f;

    void Start()
    {
        // 튀어오르기 시작
        InvokeRepeating("Jump", 0, interval);
    }

    void Jump()
    {
        // 현재 위치에서 위로 튀어오르기
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponentInParent<PhotonView>() != null && collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }
        }
    }
}
