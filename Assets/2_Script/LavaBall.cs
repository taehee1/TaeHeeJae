using UnityEngine;

public class LavaBall : MonoBehaviour
{
    public float jumpForce = 5.0f; // Ƣ������� ��
    public float interval = 2.0f; // Ƣ������� ����

    void Start()
    {
        // Ƣ������� ����
        InvokeRepeating("Jump", 0, interval);
    }

    void Jump()
    {
        // ���� ��ġ���� ���� Ƣ�������
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �÷��̾�� �浹 �� LavaBall�� ������� ��
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // LavaBall ������Ʈ ����
        }
    }
}
