using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2.0f; // �÷����� �̵� �ӵ�
    public float moveDistance = 3.0f; // ���Ʒ��� �̵��� �Ÿ�

    private Vector3 startPosition;
    private bool movingUp = true;

    void Start()
    {
        startPosition = transform.position; // ���� ��ġ ����
    }

    void Update()
    {
        // �÷����� ���� ��ġ ���
        if (movingUp)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;

            // �������� ������ �Ÿ���ŭ �̵������� ���� ��ȯ
            if (transform.position.y >= startPosition.y + moveDistance)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position += Vector3.down * speed * Time.deltaTime;

            // �Ʒ������� ������ �Ÿ���ŭ �̵������� ���� ��ȯ
            if (transform.position.y <= startPosition.y - moveDistance)
            {
                movingUp = true;
            }
        }
    }
}
