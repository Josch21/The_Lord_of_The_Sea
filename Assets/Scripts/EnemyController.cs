using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public enum state { LookOut, PlayerSpotted, Dead };
    public state currentState;
    public enum type { Melee, Distance };
    public type enemyType;
    public int maxLife = 5;
    public int currentLife;
    public float viewRange = 6;
    public float directionLookOutTime = 3;
    public float shootCooldown = 3;

    float directionTimeCounter = 0;
    float shootCooldownCounter = 0;

    public Transform viewPoint;
    public Transform bulletOrigin;
    public GameObject enemyBulletPrefab;
    public GameObject swordHitbox;
    public GameObject spotedAlert;
    public Slider lifeBar;
    public Canvas localCanvas;

    public LayerMask viewRaycastLayermask;

    bool isAttacking;

    Animator animator;
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    Transform playerTr;
    Vector2 currentDirection = Vector2.right;

    void Start()
    {
        currentLife = maxLife;
        lifeBar.maxValue = maxLife;
        lifeBar.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerTr = GameObject.Find("Player").transform;
        boxCollider = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(boxCollider, playerTr.gameObject.GetComponent<BoxCollider2D>());
    }

    void Update()
    {
        switch (currentState)
        {
            case state.LookOut:
                LookOut();
                break;
            case state.PlayerSpotted:
                if (spotedAlert.activeInHierarchy) { spotedAlert.transform.Translate(Vector3.up * 2 * Time.deltaTime); }
                spotedAlert.GetComponent<SpriteRenderer>().DOFade(0, .65f).SetEase(Ease.Flash).OnComplete(()=>spotedAlert.SetActive(false));

                if (playerTr.position.x < transform.position.x) { currentDirection = Vector2.left; }
                else { currentDirection = Vector2.right; }
                transform.localScale = new Vector3(currentDirection.x, 1, 1);

                if (Mathf.Abs(transform.position.x - playerTr.position.x) > 15)
                {
                    currentState = state.LookOut;
                }

                switch (enemyType)
                {
                    case type.Distance:
                        AttackDistance();
                        break;
                    case type.Melee:
                        AttackMelee();
                        break;
                }
                break;
            case state.Dead:
                animator.SetBool("isDead", true);
                break;
            default:
                animator.SetBool("isDead", false);
                break;
        }
        ManageLifeBar();
    }

    void LookOut()
    {
        if (enemyType == type.Melee)
        {
            animator.SetBool("walking", false);
        }
        directionTimeCounter += Time.deltaTime;
        if (directionTimeCounter >= directionLookOutTime)
        {
            directionTimeCounter = 0;
            currentDirection = currentDirection * -1;
        }
        
        RaycastHit2D raycastHit = Physics2D.Raycast(viewPoint.position, currentDirection, viewRange, viewRaycastLayermask);
        Debug.DrawRay(viewPoint.position, currentDirection * viewRange, Color.red);
        transform.localScale = new Vector3(currentDirection.x, 1, 1);
        if (raycastHit && raycastHit.collider.CompareTag("Player"))
        {
            spotedAlert.SetActive(true);
            spotedAlert.GetComponent<SpriteRenderer>().DOFade(1, 0);
            spotedAlert.transform.localPosition = new Vector3(.035f, 2.15f, 0);
            currentState = state.PlayerSpotted;
            Debug.Log("Player spotted");
        }
        shootCooldownCounter = 0;
    }

    void AttackDistance()
    {
        shootCooldownCounter += Time.deltaTime;
        if (shootCooldownCounter >= shootCooldown)
        {
            shootCooldownCounter = 0;
            animator.Play("Shoot");
            StartCoroutine(WaitToShoot(.17f));
        }

        
    }
    IEnumerator WaitToShoot(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(enemyBulletPrefab, bulletOrigin);
    }

    void AttackMelee()
    {
        if (!isAttacking)
        {
            if (Mathf.Abs(transform.position.x - playerTr.position.x) > 2)
            {
                animator.SetBool("walking", true);
                transform.Translate(currentDirection * Time.deltaTime * 5);
            }
            else
            {
                animator.SetBool("walking", false);
                animator.Play("Attack");
                StartCoroutine(ManageSwordHitbox(true, .4f));
                StartCoroutine(ManageSwordHitbox(false, .5f));
                StartCoroutine(WaitToStopAttack(1));
                isAttacking = true;
            }
        }
    }
    IEnumerator ManageSwordHitbox(bool status, float time)
    {
        yield return new WaitForSeconds(time);
        swordHitbox.SetActive(status);
    }
    IEnumerator WaitToStopAttack(float time)
    {
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentLife > 0)
        {
            if (collision.CompareTag("bullet"))
            {
                lifeBar.gameObject.SetActive(true);
                spotedAlert.SetActive(true);
                spotedAlert.GetComponent<SpriteRenderer>().DOFade(1, 0);
                spotedAlert.transform.localPosition = new Vector3(.035f, 2.15f, 0);
                currentState = state.PlayerSpotted;
                rb.AddForce(Vector2.right * 5 * CheckLeftRight(transform.position.x, playerTr.transform.position.x), ForceMode2D.Impulse);
                Destroy(collision.gameObject);
                animator.Play("Hurt");
                currentLife--;
                if (currentLife <= 0)
                {
                    currentState = state.Dead;
                    StartCoroutine(WaitToDestroy());
                }
            }
            if (collision.CompareTag("sword"))
            {
                lifeBar.gameObject.SetActive(true);
                spotedAlert.SetActive(true);
                spotedAlert.GetComponent<SpriteRenderer>().DOFade(1, 0);
                spotedAlert.transform.localPosition = new Vector3(.035f, 2.15f, 0);
                currentState = state.PlayerSpotted;
                rb.AddForce(Vector2.right * 5 * CheckLeftRight(transform.position.x, playerTr.transform.position.x), ForceMode2D.Impulse);
                animator.Play("Hurt");
                currentLife -= 2;
                if (currentLife <= 0)
                {
                    currentState = state.Dead;
                    StartCoroutine(WaitToDestroy());
                }
            }
            if (collision.CompareTag("deathZone")) 
            {
                Destroy(gameObject);
            }
        }
        
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    float CheckLeftRight(float A, float B)
    {
        if (A > B) { return 1; }
        else { return -1; }
    }

    void ManageLifeBar()
    {
        lifeBar.value = currentLife;
        if(currentDirection.x < 0)
        {
            localCanvas.transform.localScale = new Vector3(-.01f, .01f, .01f);
        }
        else
        {
            localCanvas.transform.localScale = new Vector3(.01f, .01f, .01f);
        }
    }
}
