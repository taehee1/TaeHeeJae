using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private List<GameObject> orbPrefabs;
    [SerializeField] private Transform orbSpawnPos;
    [SerializeField] private int orbIndex = 0; // ���ϴ� ���� �������� �ε����� �����մϴ�.
    [SerializeField] private GameObject player;

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
        Shoot();
        GunPos();
    }

    private void Shoot()
    {
        if (player.GetComponent<PlayerScript>().pv.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 direction = (MousePos - (Vector2)player.transform.position).normalized; // ������Ʈ ��ġ�� ����Ͽ� ������ ����մϴ�.
                GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", orbSpawnPos.position, Quaternion.identity);
                IShootable orb = orbInstance.GetComponent<IShootable>();
                if (orb != null)
                {
                    orb.Shoot(direction);
                }
                else
                {
                    Debug.LogError("The orb does not implement IShootable interface.");
                }
            }
        }
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

            GunFlip();
        }
    }

    private void GunFlip()
    {
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (angle > 90f || angle < -90f)
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        }
    }
}
