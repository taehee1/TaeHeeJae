using UnityEngine;

public class PushPlayerOnCollision : MonoBehaviour
{
    public float pushForce = 10f; // �и��� ���� ũ��

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �÷��̾ �浹�ߴ��� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾��� Rigidbody2D�� ������
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // �÷����� �÷��̾��� �浹 ������ ����Ͽ� ���� ������ ����
                Vector2 pushDirection = collision.contacts[0].normal * -1; // �浹�� ���� ������ �ݴ�������� ���� ����

                // �÷��̾�� �и��� ���� ����
                playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }
}
