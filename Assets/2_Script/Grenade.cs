using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour, IShootable
{
    [SerializeField] private float bombTime = 3f;
    [SerializeField] private float speed = 5f;

    private CameraShake cameraShake;
    public GameObject particle;
    public Rigidbody2D rb;
    public PhotonView pv;

    private void Start()
    {
        cameraShake = GetComponent<CameraShake>();
        StartCoroutine("Bomb");
    }

    IEnumerator Bomb()
    {
        yield return new WaitForSeconds(bombTime);
        PhotonNetwork.Instantiate("Explosion", transform.position, Quaternion.identity);

        cameraShake.ShakeCamera();

        pv.RPC("RPC_ShakeCamera", RpcTarget.Others);

        Destroy(gameObject);
    }

    public void Shoot(Vector2 direction)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D가 할당되지 않았습니다!");
            return;
        }

        rb.velocity = direction * speed;
    }
}
