using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2.0f; // �÷����� �̵� �ӵ�
    public float moveDistance = 3.0f; // ���Ʒ��� �̵��� �Ÿ�

    private Vector3 startPosition;
    private bool movingUp = true;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position; // ���� ��ġ ����
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ ��������
    }

    void FixedUpdate()
    {
        Vector3 targetPosition;

        // �÷����� ���� ��ġ ���
        if (movingUp)
        {
            targetPosition = transform.position + Vector3.up * speed * Time.fixedDeltaTime;

            // �������� ������ �Ÿ���ŭ �̵������� ���� ��ȯ
            if (transform.position.y >= startPosition.y + moveDistance)
            {
                movingUp = false;
            }
        }
        else
        {
            targetPosition = transform.position + Vector3.down * speed * Time.fixedDeltaTime;

            // �Ʒ������� ������ �Ÿ���ŭ �̵������� ���� ��ȯ
            if (transform.position.y <= startPosition.y - moveDistance)
            {
                movingUp = true;
            }
        }

        // Rigidbody�� �̿��� �̵�
        rb.MovePosition(targetPosition);
    }
}
