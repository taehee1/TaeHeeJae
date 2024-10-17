using UnityEngine;

public class PushPlayerOnCollision : MonoBehaviour
{
    public float pushForce = 10f; // 밀리는 힘의 크기

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어가 충돌했는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 Rigidbody2D를 가져옴
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // 플랫폼과 플레이어의 충돌 지점을 계산하여 힘의 방향을 설정
                Vector2 pushDirection = collision.contacts[0].normal * -1; // 충돌한 법선 방향의 반대방향으로 힘을 가함

                // 플레이어에게 밀리는 힘을 가함
                playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}
