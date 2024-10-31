using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private int gunTypeIndex = 0; // 원하는 오브 프리팹의 인덱스를 선택합니다.
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
            // z = 0으로 설정하여 마우스의 2D 위치를 가져옵니다.
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
                            Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized; // 오브젝트 위치를 사용하여 방향을 계산합니다.
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
                            int bulletCount = 5; // 한 번에 발사할 총알 개수
                            float spreadAngle = 5f; // 각 총알 사이의 각도 차이

                            Vector2 direction = ((Vector2)spawnPos.position - (Vector2)gun.transform.position).normalized;

                            for (int i = 0; i < bulletCount; i++)
                            {
                                float angleOffset = (i - (bulletCount - 1) / 2f) * spreadAngle; // 각 총알의 각도 오프셋 계산
                                Vector2 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * direction; // 방향에 오프셋 추가

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
