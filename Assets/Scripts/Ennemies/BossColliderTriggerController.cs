using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColliderTriggerController : MonoBehaviour
{
    public BossBattleController bossBattleController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("sword"))
        {
            bossBattleController.TakeDamageSequence(2);
        }
        if (collision.CompareTag("bullet"))
        {
            Destroy(collision.gameObject);
            bossBattleController.TakeDamageSequence(1);
        }
    }
}
