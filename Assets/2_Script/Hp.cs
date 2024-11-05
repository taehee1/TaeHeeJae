using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public PlayerSetup playerSetup;

    public Image hpUI;
    public Vector3 spawnPosition;  // ���� ��ġ

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private float respawnTime;
    private bool isDead;

    private PhotonView pv; // PhotonView ���� �߰�
    private Movement movement;
    private Animator animator;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        movement = GetComponent<Movement>();
        animator = GetComponent<Animator>();

        currentHp = maxHp;
        spawnPosition = transform.position;
        UpdateHpUI(); // �ʱ� UI ������Ʈ
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

        // �� �޼���� ��ΰ� ȣ���� �� �ֵ��� ��
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP�� �ִ밪���� Ŭ����

        UpdateHpUI(); // HP UI ������Ʈ

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
        Debug.Log("�÷��̾� ���");

        if (pv.IsMine)
        {
            InGameManager.instance.deathUI.SetActive(true);
        }
        respawnTime = 10f;

        movement.canMove = false;

        animator.SetBool("Walk_R", false);
        animator.SetBool("Walk_L", false);
        animator.SetBool("Die", true);
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // �ʿ��ϴٸ� �ٸ� Ŭ���̾�Ʈ���� �÷��̾��� ������ ���¸� ó��
        currentHp = maxHp;

        animator.SetBool("Die", false);

        transform.position = spawnPosition;
        UpdateHpUI(); // �ٸ� Ŭ���̾�Ʈ���� HP UI�� ������Ʈ

        movement.canMove = true;
        isDead = false;
    }

    private void UpdateHpUI()
    {
        hpUI.fillAmount = currentHp / maxHp; // UI ������Ʈ
    }
}
