using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2.0f; // 플랫폼의 이동 속도
    public float moveDistance = 3.0f; // 위아래로 이동할 거리

    private Vector3 startPosition;
    private bool movingUp = true;

    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장
    }

    void Update()
    {
        // 플랫폼의 현재 위치 계산
        if (movingUp)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;

            // 위쪽으로 지정한 거리만큼 이동했으면 방향 전환
            if (transform.position.y >= startPosition.y + moveDistance)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position += Vector3.down * speed * Time.deltaTime;

            // 아래쪽으로 지정한 거리만큼 이동했으면 방향 전환
            if (transform.position.y <= startPosition.y - moveDistance)
            {
                movingUp = true;
            }
        }
    }
}
