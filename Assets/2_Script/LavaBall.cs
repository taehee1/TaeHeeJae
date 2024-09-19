using UnityEngine;

public class LavaBall : MonoBehaviour
{
    public float jumpForce = 5.0f; // 튀어오르는 힘
    public float interval = 2.0f; // 튀어오르는 간격

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
        // 플레이어와 충돌 시 LavaBall을 사라지게 함
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // LavaBall 오브젝트 삭제
        }
    }
}
