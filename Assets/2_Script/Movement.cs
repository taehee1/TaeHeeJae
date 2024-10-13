using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;
    Rigidbody2D leftLegRb;
    Rigidbody2D rightLegRb;

    public GameObject body;

    private Animator animator;

    [SerializeField] float speed = 1.5f;
    [SerializeField] float stepWait = 0.5f;

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
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                animator.Play("Walk_Right");
                StartCoroutine(MoveRight(stepWait));
            }
            else
            {
                animator.Play("Walk_Left");
                StartCoroutine(MoveLeft(stepWait));
            }
        }
        else
        {
            animator.Play("Idle");
            body.GetComponent<Rigidbody2D>().velocity = Vector2.Lerp(body.GetComponent<Rigidbody2D>().velocity, Vector2.zero, 50 * Time.deltaTime);
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
