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
            InGameManager.instance.deathUI.GetComponentInChildren<Text>().text = "RESPAWN : " + Mathf.Ceil(respawnTime).ToString();

            if (respawnTime <= 0)
            {
                photonView.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // �� �޼���� ��ΰ� ȣ���� �� �ֵ��� ��
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP�� �ִ밪���� Ŭ����

        UpdateHpUI(); // HP UI ������Ʈ

        pv.RPC("RPC_ShakeCamera", PhotonNetwork.LocalPlayer);
        movement.StartCoroutine("Stun", 0.2f);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("�÷��̾� ���");

        InGameManager.instance.deathUI.SetActive(true);
        respawnTime = 10f;
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // �ʿ��ϴٸ� �ٸ� Ŭ���̾�Ʈ���� �÷��̾��� ������ ���¸� ó��
        currentHp = maxHp;
        transform.position = spawnPosition;

        UpdateHpUI(); // �ٸ� Ŭ���̾�Ʈ���� HP UI�� ������Ʈ
    }

    private void UpdateHpUI()
    {
        hpUI.fillAmount = currentHp / maxHp; // UI ������Ʈ
    }
}
