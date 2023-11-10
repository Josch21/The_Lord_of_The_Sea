using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleController : MonoBehaviour
{
    public static int collectible = 0;
    public Image[] crowns;
    public Sprite fullCrown;
    public Sprite emptyCrown;
    public GameObject crown1;
    public GameObject crown2;
    public GameObject crown3;

    void Start()
    {
        if (collectible >=3)
        {
            collectible = 3;
        }
        if (PlayerPrefs.GetInt("Crown1")==1)
        {
            crown1.SetActive(false);
            crowns[0].sprite = fullCrown;
        }
        if (PlayerPrefs.GetInt("Crown2") == 1)
        {
            crown2.SetActive(false);
            crowns[1].sprite = fullCrown;
        }
        if (PlayerPrefs.GetInt("Crown3") == 1)
        {
            crown3.SetActive(false);
            crowns[2].sprite = fullCrown;
        }
        else
        {
            crown1.SetActive(true);
            crown2.SetActive(true);
            crown3.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Image img in crowns)
        {
            img.sprite = emptyCrown;
        }
        for (int i = 0; i < collectible; i++)
        {
            crowns[i].sprite = fullCrown;
        }
    }

}
