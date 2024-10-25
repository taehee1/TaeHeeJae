using System.Collections;
using UnityEngine;

public class MovingPlatform2 : MonoBehaviour
{
    public float speed = 2f;             // �÷��� �̵� �ӵ�
    public float pauseTime = 1f;         // ���ߴ� �ð�

    private Vector3 startPosition;
    private bool movingRight = true;
    private float distance;              // �̵��� ���� �Ÿ�

    void Start()
    {
        startPosition = transform.position;
        distance = Random.Range(5f, 15f); // 5���� 15 ������ ���� �Ÿ� ����
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            // �¿�� �̵�
            if (movingRight)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, startPosition) >= distance)
                {
                    movingRight = false;
                    yield return new WaitForSeconds(pauseTime); // ����
                }
            }
            else
            {
                transform.position -= Vector3.right * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, startPosition) <= 0.1f)
                {
                    movingRight = true;
                    distance = Random.Range(5f, 15f); // ���� ��ȯ �� ���ο� ���� �Ÿ� ����
                    yield return new WaitForSeconds(pauseTime); // ����
                }
            }
            yield return null; // ���� �����ӱ��� ���
        }
    }
}
