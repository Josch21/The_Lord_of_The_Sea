using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public bool controlsEnabled;
    public int maxLife = 5;
    public int currentLife;
    public float movingForce;
    public float jumpForce;
    public float changeDirectionOnAirForce;
    public float powerUpShootTime = 10;
    public float powerUpTimer = 0;
    public float invincibleTime = 2;
    public int coinsLoseWhenDie = 5;

    public GameObject swordHitbox;
    public Transform extrasHolder;
    public Transform bulletOrigin;
    public GameObject bulletPrefab;
    public Slider lifeBar;
    public Image lifeBarFill;
    public GameObject coinParticlePrefab;
    public GameObject heartParticlePrefab;
    public GameObject powerUpParticlePrefab;

    public LayerMask platformLayerMask;

    bool isAttacking;
    bool isStunned;
    bool isDead;
    bool isInvincible;
    float shootEndLag = .4f;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    SceneController sceneController;
    CameraController cameraController;

    private void Start()
    {
        controlsEnabled = true;
        currentLife = maxLife;
        lifeBar.maxValue = maxLife;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        //Time.timeScale = .1f;
    }

    void Update()
    {
        if (!isDead)
        {
            MoveByPhysics();
            JumpByPhysics();
            Attack();
        }
        ManageSprite();
        ManagePhysicsValues();
        ChangeDirectionInAir();
        ManageLifeBar();
        ManagePowerUp();
    }

    void MoveByPhysics()
    {
        if (!isAttacking && !isStunned && controlsEnabled)
        {
            float translation = Input.GetAxis("Horizontal") * movingForce;
            translation *= Time.deltaTime;

            rb.AddForce(Vector2.right * translation);
        }
        
    }

    void JumpByPhysics()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isAttacking && !isStunned && controlsEnabled)
        {
            animator.Play("Jump");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        float extraHeightToCheck = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y * .5f), 0f, Vector2.down, extraHeightToCheck, platformLayerMask);
        return raycastHit.collider != null;
    }

    void ManageSprite()
    {
        if (!isDead)
        {
            if (rb.velocity.x > 0.001)
            {
                spriteRenderer.flipX = false;
                animator.SetBool("moving", true);
                extrasHolder.localScale = Vector3.one;
            }
            else if (rb.velocity.x < -0.001)
            {
                spriteRenderer.flipX = true;
                animator.SetBool("moving", true);
                extrasHolder.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                animator.SetBool("moving", false);
            }
            if (rb.velocity.y < -0.001)
            {
                if (!animator.GetBool("falling"))
                {
                    animator.Play("Fall");
                }
                animator.SetBool("falling", true);
            }
            else
            {
                animator.SetBool("falling", false);
            }
        }
    }

    void ManagePhysicsValues()
    {
        if (IsGrounded())
        {
            rb.drag = 15;
            movingForce = 7000;
        }
        else
        {
            rb.drag = 0;
            movingForce = 1000;
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -12, 12), Mathf.Clamp(rb.velocity.y, -30, 19));
        }
    }

    void ChangeDirectionInAir()
    {
        if (!IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) && rb.velocity.x >= 10)
            {
                rb.AddForce(Vector2.left * changeDirectionOnAirForce,ForceMode2D.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) && rb.velocity.x <= -10)
            {
                rb.AddForce(Vector2.right * changeDirectionOnAirForce, ForceMode2D.Impulse);
            }
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isAttacking && !isStunned && controlsEnabled && IsGrounded())
            {
                isAttacking = true;
                animator.Play("GunOut");
                StartCoroutine(WaitToShoot(.17f));
                StartCoroutine(StopAttacking(shootEndLag));
            }
        }
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!isAttacking && !isStunned && controlsEnabled && IsGrounded())
            {
                isAttacking = true;
                animator.Play("Attack1");
                StartCoroutine(ManageSwordHitbox(true, .15f));
                StartCoroutine(ManageSwordHitbox(false, .25f));
                StartCoroutine(StopAttacking(.5f));
            }
        }
    }

    IEnumerator StopAttacking(float endLag)
    {
        yield return new WaitForSeconds(endLag);
        isAttacking = false;
    }
    IEnumerator StopStun(float endLag)
    {
        yield return new WaitForSeconds(endLag);
        isStunned = false;
    }

    IEnumerator ManageSwordHitbox(bool status, float time)
    {
        yield return new WaitForSeconds(time);
        swordHitbox.SetActive(status);
    }

    IEnumerator WaitToShoot(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(bulletPrefab, bulletOrigin);
        cameraController.Shake();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("heart"))
        {
            if (currentLife < maxLife)
            {
                Destroy(collision.gameObject);
                currentLife++;
                Instantiate(heartParticlePrefab, collision.transform.position, collision.transform.rotation);
            }
        }
        if (collision.CompareTag("coin"))
        {
            Destroy(collision.gameObject);
            //sceneController.coins++;
            //sceneController.SpawnCoinPopUp("+1");
            sceneController.GetCoin(1);
            Instantiate(coinParticlePrefab, collision.transform.position, collision.transform.rotation);
        }
        if (collision.CompareTag("enemyBullet") && !isDead && !isInvincible)
        {
            InvincibleSequence();
            Destroy(collision.gameObject);
            animator.Play("Hurt");
            isStunned = true;
            StartCoroutine(StopStun(.4f));
            currentLife--;
            CheckIfDead();
        }
        if (collision.CompareTag("enemySword") && !isDead && !isInvincible)
        {
            InvincibleSequence();
            animator.Play("Hurt");
            isStunned = true;
            StartCoroutine(StopStun(.4f));
            currentLife-=2;
            CheckIfDead();
        }
        if (collision.CompareTag("boss") && !isDead && !isInvincible)
        {
            InvincibleSequence();
            animator.Play("Hurt");
            isStunned = true;
            StartCoroutine(StopStun(.4f));
            currentLife --;
            CheckIfDead();
        }
        if (collision.CompareTag("deathZone") && !isDead)
        {
            isDead = true;
            sceneController.ChangeScene(SceneManager.GetActiveScene().name);
        }
        if (collision.CompareTag("checkpoint") && !isDead)
        {
            PlayerPrefs.SetFloat("checkpointX", collision.transform.position.x);
            PlayerPrefs.SetFloat("checkpointY", collision.transform.position.y);

            collision.GetComponent<CheckpointController>().particles.Play();
            Debug.Log("Saved checkpoint: " + collision.gameObject.name);
        }
        if (collision.CompareTag("powerUp"))
        {
            Destroy(collision.gameObject);
            powerUpTimer = powerUpShootTime;
            Instantiate(powerUpParticlePrefab, collision.transform.position, collision.transform.rotation);
        }
        if (collision.CompareTag("chest"))
        {
            collision.GetComponent<ChestController>().OpenChest();
            //sceneController.coins += 10;
            //sceneController.SpawnCoinPopUp("+10");
            sceneController.GetCoin(10);
            Instantiate(coinParticlePrefab, collision.transform.position, collision.transform.rotation);
        }
        if (collision.CompareTag("bossBattle"))
        {
            collision.GetComponent<BossBattleController>().StartBattleSequence();
            Debug.Log("Boss Battle Collider");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "crown") 
        {
            CollectibleController.collectible++;
            Destroy(collision.gameObject);
        }
    }

    IEnumerator WaitToRestart(float time)
    {
        yield return new WaitForSeconds(time);
        sceneController.ChangeScene(SceneManager.GetActiveScene().name);
    }

    void ManageLifeBar()
    {
        lifeBar.value = currentLife;
        if (currentLife > maxLife / 3 * 2) 
        {
            lifeBarFill.color = Color.green;
        }
        else if (currentLife <= maxLife / 3 * 2 && currentLife > maxLife / 3)
        {
            lifeBarFill.color = Color.yellow;
        }
        else if (currentLife <= maxLife / 3)
        {
            lifeBarFill.color = Color.red;
        }
    }

    void ManagePowerUp()
    {
        if (powerUpTimer > 0)
        {
            powerUpTimer -= 1 * Time.deltaTime;
            shootEndLag = .18f;
        }
        else
        {
            shootEndLag = .5f;
        }
    }

    void InvincibleSequence()
    {
        isInvincible = true;
        InvokeRepeating("EnableDisableSpriterenderer", 0 , .025f);
        StartCoroutine(WaitToFinishInvincibleSequence());
    }
    void EnableDisableSpriterenderer()
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }
    IEnumerator WaitToFinishInvincibleSequence()
    {
        yield return new WaitForSeconds(invincibleTime);
        CancelInvoke("EnableDisableSpriterenderer");
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    void CheckIfDead()
    {
        if (currentLife <= 0)
        {
            isDead = true;
            animator.SetBool("dead", true);
            sceneController.LoseCoins(coinsLoseWhenDie);
            StartCoroutine(WaitToRestart(1));
            CancelInvoke("EnableDisableSpriterenderer");
        }
    }
}
