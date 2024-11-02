using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public PhotonView pv;

    public GameObject leftLeg;
    public GameObject rightLeg;
    Rigidbody2D leftLegRb;
    Rigidbody2D rightLegRb;

    public GameObject body;

    private Animator animator;
    private AudioSource audioSource;

    [SerializeField] float speed = 1.5f;
    [SerializeField] float stepWait = 0.5f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] float gravityForce = 5f;

    public bool isGround = true;
    public bool canMove = true;

    //public static Movement instance;

    private void Awake()
    {
        //instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        leftLegRb = leftLeg.GetComponent<Rigidbody2D>();
        rightLegRb = rightLeg.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }

        if (pv.IsMine)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    animator.SetBool("Walk_R", true);
                    StartCoroutine(MoveRight(stepWait));
                    animator.SetBool("Walk_L", false);
                }
                else
                {
                    animator.SetBool("Walk_L", true);
                    StartCoroutine(MoveLeft(stepWait));
                    animator.SetBool("Walk_R", false);
                }
            }
            else
            {
                animator.SetBool("Walk_R", false);
                animator.SetBool("Walk_L", false);
                body.GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(body.GetComponent<Rigidbody2D>().velocity, Vector2.zero, 50 * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                body.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower);
                isGround = false;
            }
        }
    }

    IEnumerator MoveRight(float seconds)
    {
        leftLegRb.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        rightLegRb.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
    }

    IEnumerator MoveLeft(float seconds)
    {
        rightLegRb.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        leftLegRb.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
    }
}
