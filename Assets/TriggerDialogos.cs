using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogos : MonoBehaviour
{
    public GameObject thePlayer;
    public GameObject theDialog;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            theDialog.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            theDialog.SetActive(false);
        }
    }
}
