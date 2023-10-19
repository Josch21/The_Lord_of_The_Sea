using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChestController : MonoBehaviour
{
    Animator animator;
    public ParticleSystem coinParticles;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenChest()
    {
        animator.Play("Chest");
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(WaitToParticles());
        StartCoroutine(WaitToDestroy());
    }

    IEnumerator WaitToParticles()
    {
        yield return new WaitForSeconds(.15f);
        coinParticles.Play();
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        GetComponent<SpriteRenderer>().DOFade(0, .5f).OnComplete(()=>Destroy(gameObject));

    }
}
