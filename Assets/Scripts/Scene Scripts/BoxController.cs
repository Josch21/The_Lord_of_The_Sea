using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoxController : MonoBehaviour
{
    public enum drop {Random ,Heart, PowerUp };
    public drop boxDrop;

    bool isBroken;
    public GameObject[] boxDrops;
    BoxCollider2D boxCollider;
    SpriteRenderer spriterenderer;
    public GameObject brokenPieces;
    public SpriteRenderer[] partsSprites;
    BoxCollider2D playerCollider;
    public CircleCollider2D[] partsColliders;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        playerCollider = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(boxCollider, playerCollider);
        foreach (CircleCollider2D collider in partsColliders)
        {
            Physics2D.IgnoreCollision(collider, playerCollider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("sword") && !isBroken)
        {
            Broke();
            switch (boxDrop)
            {
                case drop.Random:
                    Instantiate(boxDrops[Random.Range(0, 2)], transform.position, transform.rotation);
                    break;
                case drop.Heart:
                    Instantiate(boxDrops[0], transform.position, transform.rotation);
                    break;
                case drop.PowerUp:
                    Instantiate(boxDrops[1], transform.position, transform.rotation);
                    break;
                default:
                    Instantiate(boxDrops[Random.Range(0, 2)], transform.position, transform.rotation);

                    break;
            }
        }
    }

    void Broke()
    {
        spriterenderer.enabled = false;
        boxCollider.enabled = false;
        brokenPieces.SetActive(true);
        StartCoroutine(WaitToDestroy());
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(.5f);
        foreach (SpriteRenderer partSprite in partsSprites)
        {
            partSprite.DOFade(0, .25f).SetEase(Ease.InExpo).OnComplete(()=>Destroy(gameObject));
        }
    }
}
