using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossBattleController : MonoBehaviour
{
    public bool isMoving;
    public bool battleSequenceStarted;
    public int maxBossLife;
    public int currentBossLife;
    public int moveToChoose;
    public bool isLeft;
    public bool isDead;
    public Transform cameraTarget;
    public GameObject walls;
    public SpriteRenderer bossSprite;
    public Transform bossTransform;
    public Animator bossAnimator;
    public Transform[] bossPositions;
    public Slider bossLifeBar;
    public Transform extrasHolder;
    public Transform bulletOrigin;
    public GameObject swordHitboxA;
    public GameObject swordHitboxB;
    public GameObject enemyBulletPrefab;
    public Transform[] boxSpawners;
    public GameObject boxPrefab;
    CameraController cameraController;
    SceneController sceneController;
    PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        currentBossLife = maxBossLife;
        bossLifeBar.maxValue = maxBossLife;
        bossLifeBar.GetComponent<CanvasGroup>().alpha = 0;
        bossTransform.gameObject.SetActive(false);
        isDead = false;
    }

    private void Update()
    {
        ManageLifeBar();
    }

    void ManageLifeBar()
    {
        bossLifeBar.value = currentBossLife;
    }

    public void StartBattleSequence()
    {
        if (!battleSequenceStarted)
        {
            sceneController.audioSource.DOFade(0, 1).OnComplete(sceneController.audioSource.Stop);
            bossTransform.gameObject.SetActive(true);
            battleSequenceStarted = true;
            playerController.controlsEnabled = false;
            cameraController.target = cameraTarget;
            cameraController.controlCamera = false;
            walls.SetActive(true);
            StartCoroutine(WaitToEnterRoom());
        }
        
    }
    IEnumerator WaitToEnterRoom()
    {
        yield return new WaitForSeconds(1f);
        EnterRoom();
        StartCoroutine(WaitToBossLifeBarAppear());
        StartCoroutine(WaitToStartBattle());
    }
    IEnumerator WaitToBossLifeBarAppear()
    {
        yield return new WaitForSeconds(1f);
        bossLifeBar.GetComponent<CanvasGroup>().DOFade(1, .5f);
        Camera.main.DOOrthoSize(6.5f, .5f);
    }

    IEnumerator WaitToStartBattle()
    {
        yield return new WaitForSeconds(2);
        StartBattle();
    }

    void StartBattle()
    {
        sceneController.PlayBGM(sceneController.bossFightBgm);
        playerController.controlsEnabled = true;
        InvokeRepeating("PerformMovement", 1, 3);
        InvokeRepeating("SpawnBox", 10, 10);
    }

    void PerformMovement()
    {
        if (!isMoving && !isDead)
        {
            isMoving = true;
            
            if (currentBossLife > maxBossLife/2)
            {
                moveToChoose = Random.Range(0, 2);
            }
            else
            {
                moveToChoose = Random.Range(1, 4);
            }
            
            if (isLeft)
            {
                switch (moveToChoose)
                {
                    case 0: RunRight();
                        break;
                    case 1: SwordRight();
                        break;
                    case 2: SwordJumpRight();
                        break;
                    case 3:
                        Shoot();
                        break;
                }
            }
            else
            {
                switch (moveToChoose)
                {
                    case 0:
                        RunLeft();
                        break;
                    case 1:
                        SwordLeft();
                        break;
                    case 2:
                        SwordJumpLeft();
                        break;
                    case 3:
                        Shoot();
                        break;
                }
            }
            Debug.Log("Boss Moved: Left:" + isLeft + "Movement choosed" + moveToChoose);
        }
    }

    #region EnterRoomSequence
    void EnterRoom()
    {
        bossAnimator.Play("Fall");
        bossTransform.DOMove(bossPositions[3].position, .4f).SetEase(Ease.Flash).OnComplete(EndEnterRoom);
    }
    void EndEnterRoom()
    {
        bossAnimator.Play("Land");
        isLeft = false;
        isMoving = false;
    }
    #endregion

    #region ShootSequence
    void Shoot()
    {
        bossAnimator.Play("Shoot");
        StartCoroutine(WaitToShoot());
    }
    IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(.8f);
        //SpawnBullet
        Instantiate(enemyBulletPrefab, bulletOrigin);
        Debug.Log("Disparo Boss");
        isMoving = false;
    }
    #endregion

    #region RunLeft
    void RunLeft()
    {
        bossAnimator.Play("Run");
        bossTransform.DOMove(bossPositions[0].position, 1.5f).SetEase(Ease.Flash).OnComplete(EndRunLeft);
    }
    void EndRunLeft()
    {
        bossSprite.flipX = false;
        extrasHolder.localScale = Vector3.one;
        bossAnimator.Play("Idle");
        isLeft = true;
        isMoving = false;
    }
    #endregion

    #region RunRight
    void RunRight()
    {
        bossAnimator.Play("Run");
        bossTransform.DOMove(bossPositions[3].position, 1.5f).SetEase(Ease.Flash).OnComplete(EndRunRight);
    }
    void EndRunRight()
    {
        bossSprite.flipX = true;
        extrasHolder.localScale = new Vector3(-1, 1, 1);
        bossAnimator.Play("Idle");
        isLeft = false;
        isMoving = false;
    }
    #endregion

    #region SwordAttackLeft
    void SwordLeft()
    {
        bossAnimator.Play("Sword1");
        StartCoroutine(WaitToSwordLeftA());
    }
    IEnumerator WaitToSwordLeftA()
    {
        yield return new WaitForSeconds(.15f);
        bossTransform.DOMove(bossPositions[2].position, .25f).OnComplete(SwordLeftA);
        EnableSwordHitbox(swordHitboxA);
    }
    void SwordLeftA()
    {
        bossAnimator.Play("Sword2");
        StartCoroutine(WaitToSwordLeftB());
    }
    IEnumerator WaitToSwordLeftB()
    {
        yield return new WaitForSeconds(.15f);
        bossTransform.DOMove(bossPositions[1].position, .25f).OnComplete(SwordLeftB);
        EnableSwordHitbox(swordHitboxA);
    }
    void SwordLeftB()
    {
        bossAnimator.Play("Sword3");
        StartCoroutine(WaitToSwordLeftC());
    }
    IEnumerator WaitToSwordLeftC()
    {
        yield return new WaitForSeconds(.15f);
        bossTransform.DOMove(bossPositions[0].position, .25f);
        StartCoroutine(WaitToEndSwordLeft());
        EnableSwordHitbox(swordHitboxB);
    }
    IEnumerator WaitToEndSwordLeft()
    {
        yield return new WaitForSeconds(.5f);
        EndSwordLeft();
    }
    void EndSwordLeft()
    {
        bossSprite.flipX = false;
        extrasHolder.localScale = Vector3.one;
        isLeft = true;
        isMoving = false;
    }
    #endregion

    #region SwordAttackRight
    void SwordRight()
    {
        bossAnimator.Play("Sword1");
        StartCoroutine(WaitToSwordRightA());
    }
    IEnumerator WaitToSwordRightA()
    {
        yield return new WaitForSeconds(.15f);
        bossTransform.DOMove(bossPositions[1].position, .25f).OnComplete(SwordRightA);
        EnableSwordHitbox(swordHitboxA);
    }
    void SwordRightA()
    {
        bossAnimator.Play("Sword2");
        StartCoroutine(WaitToSwordRightB());
    }
    IEnumerator WaitToSwordRightB()
    {
        yield return new WaitForSeconds(.15f);
        bossTransform.DOMove(bossPositions[2].position, .25f).OnComplete(SwordRightB);
        EnableSwordHitbox(swordHitboxA);
    }
    void SwordRightB()
    {
        bossAnimator.Play("Sword3");
        StartCoroutine(WaitToSwordRightC());
    }
    IEnumerator WaitToSwordRightC()
    {
        yield return new WaitForSeconds(.15f);
        bossTransform.DOMove(bossPositions[3].position, .25f);
        StartCoroutine(WaitToEndSwordRight());
        EnableSwordHitbox(swordHitboxB);
    }
    IEnumerator WaitToEndSwordRight()
    {
        yield return new WaitForSeconds(.5f);
        EndSwordRight();
    }
    void EndSwordRight()
    {
        bossSprite.flipX = true;
        extrasHolder.localScale = new Vector3(-1, 1, 1);
        isLeft = false;
        isMoving = false;
    }
    #endregion

    #region SwordJumpLeftAttack
    void SwordJumpLeft()
    {
        bossAnimator.Play("Run");
        bossTransform.DOMove(bossPositions[2].position, .5f).SetEase(Ease.Flash).OnComplete(SwordJumpLeftA);
    }
    void SwordJumpLeftA()
    {
        bossAnimator.Play("Jump");
        bossTransform.DOMoveX(bossPositions[1].position.x, .35f).SetEase(Ease.Flash);
        bossTransform.DOMoveY(bossPositions[1].position.y + 4, .35f).SetEase(Ease.OutSine).OnComplete(SwordJumpLeftB);
    }
    void SwordJumpLeftB()
    {
        bossAnimator.Play("SwordJump");
        bossTransform.DOMoveX(bossPositions[0].position.x, .3f).SetEase(Ease.Flash);
        bossTransform.DOMoveY(bossPositions[0].position.y, .3f).SetEase(Ease.InSine);
        StartCoroutine(WaitToEndSwordJumpLeft());
        EnableSwordHitbox(swordHitboxB);
    }
    IEnumerator WaitToEndSwordJumpLeft()
    {
        yield return new WaitForSeconds(.6f);
        EndSwordJumpLeft();
    }
    void EndSwordJumpLeft()
    {
        bossAnimator.Play("Idle");
        bossSprite.flipX = false;
        extrasHolder.localScale = Vector3.one;
        isLeft = true;
        isMoving = false;
    }
    #endregion

    #region SwordJumpRightAttack
    void SwordJumpRight()
    {
        bossAnimator.Play("Run");
        bossTransform.DOMove(bossPositions[1].position, .5f).SetEase(Ease.Flash).OnComplete(SwordJumpRightA);
    }
    void SwordJumpRightA()
    {
        bossAnimator.Play("Jump");
        bossTransform.DOMoveX(bossPositions[2].position.x, .35f).SetEase(Ease.Flash);
        bossTransform.DOMoveY(bossPositions[2].position.y + 4, .35f).SetEase(Ease.OutSine).OnComplete(SwordJumpRightB);
    }
    void SwordJumpRightB()
    {
        bossAnimator.Play("SwordJump");
        bossTransform.DOMoveX(bossPositions[3].position.x, .3f).SetEase(Ease.Flash);
        bossTransform.DOMoveY(bossPositions[3].position.y, .3f).SetEase(Ease.InSine);
        StartCoroutine(WaitToEndSwordJumpRight());
        EnableSwordHitbox(swordHitboxB);
    }
    IEnumerator WaitToEndSwordJumpRight()
    {
        yield return new WaitForSeconds(.6f);
        EndSwordJumpRight();
    }
    void EndSwordJumpRight()
    {
        bossAnimator.Play("Idle");
        bossSprite.flipX = true;
        extrasHolder.localScale = new Vector3(-1, 1, 1);
        isLeft = false;
        isMoving = false;
    }
    #endregion

    void EnableSwordHitbox(GameObject hitbox)
    {
        hitbox.SetActive(true);
        StartCoroutine(WaitToDisableSwordHitbox(hitbox));
    }
    IEnumerator WaitToDisableSwordHitbox(GameObject hitbox)
    {
        yield return new WaitForSeconds(.15f);
        hitbox.SetActive(false);
    }

    public void TakeDamageSequence(int amount)
    {
        if (currentBossLife > 0 && !isDead)
        {
            currentBossLife -= amount;
            InvokeRepeating("ChangeSpriteColor", 0, .05f);
            StartCoroutine(WaitToFinishTakeDamageSequence());
            CheckIfDead();
        }
    }
    void ChangeSpriteColor()
    {
        if (bossSprite.color == Color.white)
        {
            bossSprite.color = Color.red;
        }
        else
        {
            bossSprite.color = Color.white;
        }
    }
    IEnumerator WaitToFinishTakeDamageSequence()
    {
        yield return new WaitForSeconds(.15f);
        CancelInvoke("ChangeSpriteColor");
        bossSprite.color = Color.white;
    }

    void CheckIfDead()
    {
        if (currentBossLife <= 0)
        {
            isDead = true;
            playerController.controlsEnabled = false;
            bossTransform.GetComponent<BoxCollider2D>().enabled = false;
            extrasHolder.gameObject.SetActive(false);
            DOTween.KillAll();
            bossTransform.DOMoveY(bossPositions[0].position.y, .25f).SetEase(Ease.Flash);
            bossAnimator.Play("Death");
            StartCoroutine(WaitToChangeScene());
        }
    }

    IEnumerator WaitToChangeScene()
    {
        yield return new WaitForSeconds(2);
        sceneController.ChangeScene("VictoryScreen");
    }

    void SpawnBox()
    {
        Transform spawnPoint = boxSpawners[Random.Range(0, 3)];
        Instantiate(boxPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
