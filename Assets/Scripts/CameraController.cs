using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    public bool followTarget;
    public bool controlCamera;
    public float followSpeedX;
    public float followSpeedY;
    public float yOffset;
    public Transform target;

    private void Start()
    {
        followTarget = true;
        controlCamera = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Shake();
        }
    }

    void FixedUpdate()
    {
        if (followTarget)
        {
            Vector3 newPosX;
            if (controlCamera)
            {
                newPosX = new Vector3(target.position.x + Input.GetAxis("Horizontal") * 2, transform.position.y, -10);
            }
            else
            {
                newPosX = new Vector3(target.position.x, transform.position.y, -10);
            }
            transform.position = Vector3.Slerp(transform.position, newPosX, followSpeedX * Time.deltaTime);

            Vector3 newPosY = new Vector3(transform.position.x, target.position.y + yOffset, -10);
            transform.position = Vector3.Slerp(transform.position, newPosY, followSpeedY * Time.deltaTime);
        }
    }

    public void Shake()
    {
        followTarget = false;
        transform.DOShakePosition(.23f, .4f, 50).OnComplete(EnableFollowTarget);
    }

    void EnableFollowTarget()
    {
        followTarget = true;
    }
}
