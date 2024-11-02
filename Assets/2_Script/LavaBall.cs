using Photon.Pun;
using UnityEngine;

public class LavaBall : MonoBehaviour
{
    public float jumpForce = 5.0f;      // 튀어오르는 힘
    public float interval = 2.0f;       // 튀어오르는 간격
    public float damage = 10f;          // 데미지 값
    public float damageInterval = 1.0f; // 데미지가 들어가는 간격

    private float lastDamageTime = 0f;  // 마지막으로 데미지가 들어간 시간

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
            Hp hp = collision.gameObject.GetComponentInParent<Hp>();

            // 마지막 데미지 시점에서 damageInterval 이상 경과한 경우에만 데미지 적용
            if (hp != null && Time.time >= lastDamageTime + damageInterval)
            {
                hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                Debug.Log("데미지 전송: " + damage); // 데미지 전송 시 출력

                // 마지막 데미지 시점을 현재 시간으로 업데이트
                lastDamageTime = Time.time;
            }
        }
    }
}
