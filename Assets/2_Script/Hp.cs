using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public PlayerSetup playerSetup;

    public Image hpUI;
    public Vector3 spawnPosition;  // 시작 위치

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private float respawnTime;
    private bool isDead;

    private PhotonView pv; // PhotonView 변수 추가
    private Movement movement;
    private Animator animator;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();

        currentHp = maxHp;
        spawnPosition = transform.position;
        UpdateHpUI(); // 초기 UI 업데이트
    }

    private void Update()
    {
        if (respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            InGameManager.instance.respawnText.text = "RESPAWN " + Mathf.Ceil(respawnTime).ToString();

            if (respawnTime <= 0)
            {
                InGameManager.instance.deathUI.SetActive(false);
                pv.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
            }
        }

        if (transform.position.y < InGameManager.instance.blackZone.transform.position.y)
        {
            photonView.RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            movement.body.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            transform.position = new Vector2(transform.position.x, transform.position.y + 20);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // 이 메서드는 모두가 호출할 수 있도록 함
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP를 최대값으로 클램프

        UpdateHpUI(); // HP UI 업데이트

        if (pv.IsMine)
        {
            gameObject.GetComponent<CameraShake>().RPC_ShakeCamera();
        }

        if (currentHp <= 0 && !isDead)
        {
            Die();
            isDead = true;
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");

        if (pv.IsMine)
        {
            InGameManager.instance.deathUI.SetActive(true);
        }
        respawnTime = 8f;

        movement.canMove = false;

        animator.SetBool("Walk_R", false);
        animator.SetBool("Walk_L", false);
        animator.SetBool("Die", true);
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // 필요하다면 다른 클라이언트에서 플레이어의 리스폰 상태를 처리
        currentHp = maxHp;

        animator.SetBool("Die", false);

        transform.position = spawnPosition;
        UpdateHpUI(); // 다른 클라이언트에서 HP UI를 업데이트

        movement.canMove = true;
        isDead = false;
    }

    private void UpdateHpUI()
    {
        hpUI.fillAmount = currentHp / maxHp; // UI 업데이트
    }
}
