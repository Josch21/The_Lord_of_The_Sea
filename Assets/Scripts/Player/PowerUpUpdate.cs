using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUpdate : MonoBehaviour
{
    public PlayerController controller;
    public Sprite speedbuff;
    public Sprite attackDebuff;
    public Sprite attackBuff;
    public GameObject buffSlot;
    private GameObject speedBuffGO;
    private GameObject attackDebuffSlotGO;
    private GameObject attackBuffGO;


    // Update is called once per frame
    void Update()
    {
        if (controller.speedBost > 1) 
        {
            if (speedBuffGO == null) 
            {
                speedBuffGO = Instantiate(buffSlot, gameObject.transform);
                speedBuffGO.GetComponent<Image>().sprite = speedbuff;
            }
            if (attackDebuffSlotGO == null)
            {
                attackDebuffSlotGO = Instantiate(buffSlot, gameObject.transform);
                attackDebuffSlotGO.GetComponent<Image>().sprite = attackDebuff;
            }
        }
        else
        {
            Destroy(speedBuffGO);
            Destroy(attackDebuffSlotGO);
            speedBuffGO = null;
            attackDebuffSlotGO = null;
        }

        if (controller.powerUpTimer > 1)
        {
            if (attackBuffGO == null)
            {
                attackBuffGO = Instantiate(buffSlot, gameObject.transform);
                attackBuffGO.GetComponent<Image>().sprite = attackBuff;
            }
        }
        else 
        {
            Destroy(attackBuffGO);
            attackBuffGO = null;
        }
    }
}
