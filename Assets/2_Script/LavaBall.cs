using Photon.Pun;
using UnityEngine;

public class LavaBall : MonoBehaviour
{
    public float jumpForce = 5.0f;      // Ƣ������� ��
    public float interval = 2.0f;       // Ƣ������� ����
    public float damage = 10f;          // ������ ��
    public float damageInterval = 1.0f; // �������� ���� ����

    private float lastDamageTime = 0f;  // ���������� �������� �� �ð�

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
        if (collision.gameObject.tag == "Player")
        {
            Hp hp = collision.gameObject.GetComponentInParent<Hp>();

            // ������ ������ �������� damageInterval �̻� ����� ��쿡�� ������ ����
            if (hp != null && Time.time >= lastDamageTime + damageInterval)
            {
                hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                Debug.Log("������ ����: " + damage); // ������ ���� �� ���

                // ������ ������ ������ ���� �ð����� ������Ʈ
                lastDamageTime = Time.time;
            }
        }
    }
}
