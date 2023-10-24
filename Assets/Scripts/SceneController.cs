using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Image fadeScreen;
    public Transform playertTr;
    public Transform cameraTr;
    public Text coinCounter;
    public RectTransform coinsPopUpSpawn;
    public GameObject coinsPopUpPrefab;
    public GameObject pausePanel;
    public int coins = 0;
    bool isPaused;
    public AudioSource audioSource;
    public AudioClip levelBgm;
    public AudioClip bossFightBgm;

    void Start()
    {
        FadeInScene();
        TeleportPlayerToCheckpoint();
        coins = PlayerPrefs.GetInt("Coins");
        isPaused = false;
        audioSource = GetComponent<AudioSource>();
        PlayBGM(levelBgm);
    }

    private void Update()
    {
        coinCounter.text = coins.ToString();
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    void TeleportPlayerToCheckpoint()
    {
        if (PlayerPrefs.HasKey("checkpointX") && PlayerPrefs.HasKey("checkpointY"))
        {
            playertTr.position = new Vector3(PlayerPrefs.GetFloat("checkpointX"), PlayerPrefs.GetFloat("checkpointY"), playertTr.position.z);
            cameraTr.position = new Vector3(PlayerPrefs.GetFloat("checkpointX"), PlayerPrefs.GetFloat("checkpointY"), cameraTr.position.z);
        }
    }

    void FadeInScene()
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.DOFade(0, 1f).OnComplete(() => fadeScreen.gameObject.SetActive(false));
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1;
        fadeScreen.gameObject.SetActive(true);
        audioSource.DOFade(0, 1);
        fadeScreen.DOFade(1, 1).OnComplete(()=>SceneManager.LoadScene(sceneName));
    }

    public void SpawnCoinPopUp(string amountText, Color color)
    {
        GameObject popUpCoin = Instantiate(coinsPopUpPrefab, coinsPopUpSpawn);
        popUpCoin.GetComponent<Text>().text = amountText;
        popUpCoin.GetComponent<Text>().color = color;
    }

    public void GetCoin(int amount)
    {
        if (amount + coins > 99)
        {
            coins = 99;
        }
        else
        {
            coins += amount;

        }
        SpawnCoinPopUp("+" + amount, Color.green);
        PlayerPrefs.SetInt("Coins", coins);
    }

    public void LoseCoins(int amount)
    {
        if (coins > amount)
        {
            coins-= amount;
        }
        else
        {
            coins = 0;

        }
        SpawnCoinPopUp("-" + amount, Color.red);
        PlayerPrefs.SetInt("Coins", coins);
    }

    public void Pause()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
            pausePanel.SetActive(false);
        }
    }

    public void PlayBGM(AudioClip audioClip)
    {
        audioSource.volume = .5f;
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
