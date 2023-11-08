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
