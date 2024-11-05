using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;
    private PhotonView pv;

    public float speed = 10f;
    public float damage = 10f;

    private bool isDestroyed = false; // �ı� ���� �÷���

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Invoke("AutoRemove", 6f);

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1f, Color.red, 5f);
    }

    public void Shoot(Vector2 direction)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        Debug.Log("�Ѿ� �߻� ����: " + direction);

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �̹� �ı��� ��� ó������ ����
        if (isDestroyed) return;

        Debug.Log("�浹 ����: " + collision.gameObject.name);

        // ���� �浹 ó��
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (pv.IsMine && !isDestroyed)
            {
                isDestroyed = true; // �ı� �÷��� ����
                PhotonNetwork.Destroy(gameObject); // Photon���� ��ü �ı�
            }
            return; // Ground���� �浹 ó���� �������Ƿ� �� �̻� �������� ����
        }

        // �浹�� ��ü�� PhotonView�� ������
        PhotonView photonView = collision.gameObject.GetComponentInParent<PhotonView>();
        if (photonView == null) return; // PhotonView�� ������ �ߴ�

        // ���� ����
        if (collision.gameObject.CompareTag("Player") && pv.IsMine != photonView.IsMine)
        {
            Debug.Log("���� �÷��̾�� ����: " + collision.gameObject.name);

            // Hp ��ũ��Ʈ ã��
            Hp hp = collision.gameObject.GetComponentInParent<Hp>();
            if (hp != null)
            {
                hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);

                if (pv.IsMine && !isDestroyed)
                {
                    isDestroyed = true; // �ı� �÷��� ����
                    PhotonNetwork.Destroy(gameObject); // Photon���� ��ü �ı�
                }
            }
        }
    }

    private void AutoRemove()
    {
        if (isDestroyed) return; // �̹� �ı��� ��� �ߴ�

        if (pv.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
