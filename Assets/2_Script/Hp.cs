using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviourPunCallbacks
{
    public Image hpUI;
    public Vector3 spawnPosition;  // ���� ��ġ

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private PhotonView photonView; // PhotonView ���� �߰�

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentHp = maxHp;
        spawnPosition = transform.position;
        UpdateHpUI(); // �ʱ� UI ������Ʈ
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // �� �޼���� ��ΰ� ȣ���� �� �ֵ��� ��
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP�� �ִ밪���� Ŭ����

        UpdateHpUI(); // HP UI ������Ʈ

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // HP �ʱ�ȭ�ϰ� ������ ��ġ�� �̵�
        currentHp = maxHp;
        transform.position = spawnPosition;
        UpdateHpUI(); // HP UI ������Ʈ

        // �߰����� ������ ���� (�����ð�, �ִϸ��̼� ��)
        photonView.RPC("OnRespawn", RpcTarget.All, photonView.Owner.NickName);
    }

    [PunRPC]
    public void OnRespawn(string playerName)
    {
        Debug.Log($"{playerName} has respawned.");
        // �ʿ��ϴٸ� �ٸ� Ŭ���̾�Ʈ���� �÷��̾��� ������ ���¸� ó��
        UpdateHpUI(); // �ٸ� Ŭ���̾�Ʈ���� HP UI�� ������Ʈ
    }

    private void UpdateHpUI()
    {
        hpUI.fillAmount = currentHp / maxHp; // UI ������Ʈ
    }
}
