using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;

    private Rigidbody2D rb;
    public Image hpUI;

    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    public float currentHp = 100;
    [SerializeField] private float maxHp = 100;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }


    private void Update()
    {
        Move();
        Jump();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        hpUI.fillAmount = currentHp / maxHp;
    }

    private void Move()
    {
        if (pv.IsMine)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
        }
    }
    
    //변수동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hpUI.fillAmount);
        }
        else
        {
            hpUI.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
