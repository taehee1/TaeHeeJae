using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public Image hpUI;
    public Vector3 spawnPosition;  // ���� ��ġ

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private float respawnTime;
    private bool isDead;

    private PhotonView pv; // PhotonView ���� �߰�
    private Movement movement;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        movement = GetComponent<Movement>();

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
                for (int i = 0; i < 4; i++)
                {
                    PlayerSetup.instance.parts[i].GetComponent<Balance>().force = 10000;
                }

                PlayerSetup.instance.parts[4].GetComponent<Balance>().force = 300;
                PlayerSetup.instance.parts[7].GetComponent<Balance>().force = 300;

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

        for (int i = 0; i < 5; i++)
        {
            PlayerSetup.instance.parts[i].GetComponent<Balance>().force = 0;
        }

        PlayerSetup.instance.parts[7].GetComponent<Balance>().force = 0;
        movement.canMove = false;
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // �ʿ��ϴٸ� �ٸ� Ŭ���̾�Ʈ���� �÷��̾��� ������ ���¸� ó��
        currentHp = maxHp;
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
