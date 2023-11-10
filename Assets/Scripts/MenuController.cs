using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Image fadeScreen;
    public Button continueButton;
    void Start()
    {
        FadeInScene();
        if (continueButton && PlayerPrefs.HasKey("checkpointX"))
        {
            continueButton.interactable = true;
        }
    }

    void FadeInScene()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.DOFade(0, 1f).OnComplete(() => fadeScreen.gameObject.SetActive(false));
    }

    public void NewGame(string sceneName)
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene(sceneName));
    }

    public void ContinueGame(string sceneName)
    {

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene(sceneName));
    }

    public void ResetPrefs()
    {
        PlayerPrefs.SetInt("Crown1", 0);
        PlayerPrefs.SetInt("Crown2", 0);
        PlayerPrefs.SetInt("Crown3", 0);
        CollectibleController.collectible = 0;
        PlayerPrefs.DeleteAll();
    }

    public void ExitGame()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.DOFade(1, 1).OnComplete(Application.Quit);
    }
}
