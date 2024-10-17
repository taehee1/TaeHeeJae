using Photon.Pun;
using UnityEngine;

public class LavaBall : MonoBehaviour
{
    public float jumpForce = 5.0f; // Ƣ������� ��
    public float interval = 2.0f; // Ƣ������� ����
    public float damage = 10f;

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
            if (collision.gameObject.GetComponentInParent<PhotonView>() != null && collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }
        }
    }
}
