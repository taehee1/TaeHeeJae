using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public float rotationSpeed = 5f; // ȸ�� �ӵ� ����
    private Balance balance; // Balance ��ũ��Ʈ ����

    void Start()
    {
        // Balance ��ũ��Ʈ ��������
        balance = GetComponent<Balance>();
    }

    void Update()
    {
        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // 2D ȯ���̹Ƿ� Z���� 0���� ����

        // Upper Arm�� ���� ��ġ���� ���콺 ���������� ���� ���
        Vector2 direction = (mousePosition - transform.position).normalized;

        // ��ǥ ȸ�� ���� ��� (���⿡ +90���� �߰�)
        float targetRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Balance ��ũ��Ʈ�� targetRotation �� ������Ʈ
        balance.targetRotation = targetRotation;
    }
}
