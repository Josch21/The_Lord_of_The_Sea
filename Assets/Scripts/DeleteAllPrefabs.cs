using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAllPrefabs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void YourButtonClickMethod()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted!");
    }
}
