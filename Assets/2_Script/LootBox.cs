using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBox : MonoBehaviour
{
    public GameObject Image;

    PhotonView enteringPlayerPhotonView;
    GunRandom gunRandom;

    private bool canLoot = true;

    private float lootTime = 2f;
    private float currentLootTime = 2f;

    private float lootCoolTime = 10f;
    private float currentLootCoolTime;

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && canLoot && enteringPlayerPhotonView.IsMine)
        {
            currentLootTime -= Time.deltaTime;

            if (currentLootTime < 0 && canLoot)
            {
                canLoot = false;

                RandomGun();
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && canLoot && enteringPlayerPhotonView.IsMine)
        {
            currentLootTime = lootTime;
        }

        if (!canLoot)
        {
            currentLootCoolTime += Time.deltaTime;

            if (currentLootCoolTime > lootCoolTime)
            {
                canLoot = true;

                currentLootCoolTime = 0;
            }
        }

        if (canLoot)
        {
            Image.GetComponent<Image>().fillAmount = currentLootTime / lootTime;
        }
        else
        {
            Image.GetComponent<Image>().fillAmount = currentLootCoolTime / lootCoolTime;
        }
    }

    private void RandomGun()
    {
        int random = Random.Range(1, 4);
        gunRandom.photonView.RPC("RandomGunSetup", RpcTarget.All, random, enteringPlayerPhotonView.Owner.NickName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enteringPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();
        gunRandom = collision.GetComponentInParent<GunRandom>();

        if (collision.tag == "Player" && enteringPlayerPhotonView != null && enteringPlayerPhotonView.IsMine)
        {
            Image.SetActive(true);

            canLoot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enteringPlayerPhotonView = collision.gameObject.GetComponent<PhotonView>();

        if (collision.tag == "Player" && enteringPlayerPhotonView != null && enteringPlayerPhotonView.IsMine)
        {
            canLoot = false;

            currentLootTime = lootTime;

            Image.GetComponent<Image>().fillAmount = currentLootTime / lootTime;

            Image.SetActive(false);
        }
    }
}
