using System.Collections;
using UnityEngine;

public class MovingPlatform2 : MonoBehaviour
{
    public float speed = 2f;             // 플랫폼 이동 속도
    public float pauseTime = 1f;         // 멈추는 시간

    private Vector3 startPosition;
    private bool movingRight = true;
    private float distance;              // 이동할 랜덤 거리

    void Start()
    {
        startPosition = transform.position;
        distance = Random.Range(5f, 15f); // 5에서 15 사이의 랜덤 거리 설정
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            // 좌우로 이동
            if (movingRight)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, startPosition) >= distance)
                {
                    movingRight = false;
                    yield return new WaitForSeconds(pauseTime); // 멈춤
                }
            }
            else
            {
                transform.position -= Vector3.right * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, startPosition) <= 0.1f)
                {
                    movingRight = true;
                    distance = Random.Range(5f, 15f); // 방향 전환 시 새로운 랜덤 거리 설정
                    yield return new WaitForSeconds(pauseTime); // 멈춤
                }
            }
            yield return null; // 다음 프레임까지 대기
        }
    }
}
