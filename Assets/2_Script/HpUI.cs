using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviourPunCallbacks
{
    public Image hpImage;

    public Vector3 offset = new Vector3(0, 0.4f, 0);  // HP ���� ������
    private Transform target;                         // Ÿ�� �÷��̾��� Transform

    // Ÿ���� �����ϴ� �޼���
    public void SetTarget(GameObject player)
    {
        target = player.transform;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Ÿ���� �Ӹ� �� ��ġ�� ������� HP �� ��ġ�� ����
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;
    }
}
