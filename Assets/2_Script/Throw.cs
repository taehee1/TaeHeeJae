using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPos;

    public GameObject grenade;

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
        throwGrenade();
    }

    private void throwGrenade()
    {
        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Vector2 direction = (MousePos - (Vector2)player.transform.position).normalized; // ������Ʈ ��ġ�� ����Ͽ� ������ ����մϴ�.
                GameObject orbInstance = PhotonNetwork.Instantiate("Grenade", spawnPos.position, Quaternion.identity);
                IShootable orb = orbInstance.GetComponent<IShootable>();
                if (orb != null)
                {
                    orb.Shoot(direction);
                }
                else
                {
                    Debug.Log("The orb does not implement IShootable interface.");
                }
            }
        }
    }
}
