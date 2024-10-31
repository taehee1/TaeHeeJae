using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private int gunTypeIndex = 0; // ���ϴ� ���� �������� �ε����� �����մϴ�.
    [SerializeField] private float reboundForce;
    [SerializeField] private float delay;
    [SerializeField] private float bulletDamage;

    private bool canShoot = true;

    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject hand;

    public PhotonView pv;
    public Movement movement;

    private float lastAngle;
    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Shoot();
        Reload();
    }

    private void Shoot()
    {
        if (player.GetComponent<PhotonView>().IsMine && canShoot)
        {
            if (Input.GetMouseButton(0) && movement.canMove)
            {
                StartCoroutine(ShootCooldown(delay));

                switch (gunTypeIndex) 
                {
                    case 0:
                        {
                            Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized; // ������Ʈ ��ġ�� ����Ͽ� ������ ����մϴ�.
                            GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", spawnPos.position, Quaternion.identity);
                            IShootable orb = orbInstance.GetComponent<IShootable>();
                            if (orb != null)
                            {
                                orb.Shoot(direction);
                                orbInstance.GetComponent<Bullet>().damage = bulletDamage;
                            }
                            else
                            {
                                Debug.Log("The orb does not implement IShootable interface.");
                            }
                            break;
                        }
                    case 1:
                        {
                            int bulletCount = 5; // �� ���� �߻��� �Ѿ� ����
                            float spreadAngle = 5f; // �� �Ѿ� ������ ���� ����

                            Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized;

                            for (int i = 0; i < bulletCount; i++)
                            {
                                float angleOffset = (i - (bulletCount - 1) / 2f) * spreadAngle; // �� �Ѿ��� ���� ������ ���
                                Vector2 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * direction; // ���⿡ ������ �߰�

                                GameObject orbInstance = PhotonNetwork.Instantiate("Bullet", spawnPos.position, Quaternion.identity);
                                IShootable orb = orbInstance.GetComponent<IShootable>();
                                if (orb != null)
                                {
                                    orb.Shoot(spreadDirection);
                                    orbInstance.GetComponent<Bullet>().damage = bulletDamage;
                                }
                            }
                            break;
                        }
                }

                audioSource.Play();
                hand.GetComponent<Rigidbody2D>().AddForce(Vector2.up * reboundForce);
            }
        }
    }

    IEnumerator ShootCooldown(float seconds)
    {
        canShoot = false;
        yield return new WaitForSeconds(seconds);
        canShoot = true;
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
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            
        }
    }
}
