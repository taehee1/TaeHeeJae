using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour, IShootable
{
    private Rigidbody2D rb;
    private PhotonView pv;

    public float speed = 10f;
    public float damage = 10f;

    private bool isDestroyed = false; // 파괴 여부 플래그

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
            Debug.LogError("Rigidbody2D가 할당되지 않았습니다!");
            return;
        }
        Debug.Log("총알 발사 방향: " + direction);

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 파괴된 경우 처리하지 않음
        if (isDestroyed) return;

        Debug.Log("충돌 감지: " + collision.gameObject.name);

        // 땅과 충돌 처리
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (pv.IsMine && !isDestroyed)
            {
                isDestroyed = true; // 파괴 플래그 설정
                PhotonNetwork.Destroy(gameObject); // Photon에서 객체 파괴
            }
            return; // Ground와의 충돌 처리가 끝났으므로 더 이상 진행하지 않음
        }

        // 충돌한 객체의 PhotonView를 가져옴
        PhotonView photonView = collision.gameObject.GetComponentInParent<PhotonView>();
        if (photonView == null) return; // PhotonView가 없으면 중단

        // 공격 판정
        if (collision.gameObject.CompareTag("Player") && pv.IsMine != photonView.IsMine)
        {
            Debug.Log("상대방 플레이어에게 명중: " + collision.gameObject.name);

            // Hp 스크립트 찾기
            Hp hp = collision.gameObject.GetComponentInParent<Hp>();
            if (hp != null)
            {
                hp.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);

                if (pv.IsMine && !isDestroyed)
                {
                    isDestroyed = true; // 파괴 플래그 설정
                    PhotonNetwork.Destroy(gameObject); // Photon에서 객체 파괴
                }
            }
        }
    }

    private void AutoRemove()
    {
        if (isDestroyed) return; // 이미 파괴된 경우 중단

        if (pv.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
