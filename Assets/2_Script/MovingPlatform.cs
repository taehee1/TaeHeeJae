using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2.0f; // 플랫폼의 이동 속도
    public float moveDistance = 3.0f; // 위아래로 이동할 거리

    private Vector3 startPosition;
    private bool movingUp = true;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
    }

    void FixedUpdate()
    {
        Vector3 targetPosition;

        // 플랫폼의 현재 위치 계산
        if (movingUp)
        {
            targetPosition = transform.position + Vector3.up * speed * Time.fixedDeltaTime;

            // 위쪽으로 지정한 거리만큼 이동했으면 방향 전환
            if (transform.position.y >= startPosition.y + moveDistance)
            {
                movingUp = false;
            }
        }
        else
        {
            targetPosition = transform.position + Vector3.down * speed * Time.fixedDeltaTime;

            // 아래쪽으로 지정한 거리만큼 이동했으면 방향 전환
            if (transform.position.y <= startPosition.y - moveDistance)
            {
                movingUp = true;
            }
        }

        // Rigidbody를 이용해 이동
        rb.MovePosition(targetPosition);
    }
}
