using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public HingeJoint2D upperArmHinge;  // ���(Upper Arm)�� ���� ����Ʈ
    public Camera mainCamera;            // ī�޶� ����
    public float maxMotorSpeed = 100f;   // ���� �ִ� �ӵ�
    public float maxMotorTorque = 1000f; // ���� �ִ� ��(��ũ)
    public float damping = 5f;           // �ε巯�� ȸ���� ���� ���谪
    public float angleThreshold = 1f;    // ȸ�� �ΰ���

    private const float baseRotationZ = -90f; // �⺻ ȸ�� ����

    void Update()
    {
        // ���콺 ��ġ�� �������� ���� ��ǥ�� ��ȯ
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // 2D �����̹Ƿ� z���� 0���� ����

        // Upper Arm�� ���콺 ��ġ ������ ���� ���� ���
        Vector2 direction = mousePosition - upperArmHinge.transform.position;

        // ���콺 ������ ���� ���
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ���� ����� ȸ�� ���� ��������
        float currentAngle = upperArmHinge.transform.rotation.eulerAngles.z;

        // ���� ���� ���
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle + baseRotationZ);

        // ���� ���̰� �Ӱ谪�� ��� ���� ���͸� Ȱ��ȭ
        if (Mathf.Abs(angleDifference) > angleThreshold)
        {
            // ���� �ӵ��� �ε巴�� ����
            float motorSpeed = Mathf.Clamp(angleDifference * damping, -maxMotorSpeed, maxMotorSpeed);

            // HingeJoint2D ���� ����
            JointMotor2D motor = new JointMotor2D
            {
                motorSpeed = motorSpeed,         // ȸ�� ����� �ӵ� ����
                maxMotorTorque = maxMotorTorque  // ������ �ִ� �� ����
            };

            upperArmHinge.motor = motor;
            upperArmHinge.useMotor = true; // ���� Ȱ��ȭ
        }
        else
        {
            upperArmHinge.useMotor = false; // ���� ���̰� ������ ���� ��Ȱ��ȭ
        }
    }
}
