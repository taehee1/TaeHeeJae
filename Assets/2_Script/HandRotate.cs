using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRotate : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SpriteRenderer gunSpr;

    public PhotonView pv;

    private float lastAngle;

    private Camera cam;

    Vector2 MousePos
    {
        get
        {
            // z = 0���� �����Ͽ� ���콺�� 2D ��ġ�� �����ɴϴ�.
            Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
            return mousePos;
        }
    }

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        GunPos();
    }

    private void GunPos()
    {
        if (player.GetComponent<PlayerScript>().pv.IsMine)
        {
            // ���콺 ��ġ�� ���� ��ġ ���� ������ ����մϴ�.
            Vector2 direction = MousePos - (Vector2)player.transform.position;

            // ���� ���Ϳ��� ������ ����մϴ�.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // ���� ������Ʈ ȸ���� �����մϴ�. Z���� �������� ȸ���ϹǷ� Quaternion.Euler�� ����մϴ�.
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (Mathf.Abs(lastAngle - angle) > 1f) // �Ӱ谪 1�� ���̷� ����
            {
                pv.RPC("GunFlip", RpcTarget.AllBuffered);
                lastAngle = angle;
            }
        }
    }

    [PunRPC]
    void GunFlip()
    {
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (angle > 90f || angle < -90f)
        {
            gunSpr.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gunSpr.GetComponent<SpriteRenderer>().flipY = false;
        }
    }
}
