using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake() {
        if (instance == null) {instance=this;}
    }
    public LevelEnemyConfig levelEnemyConfig;
    public Image progressBar;
    [Header("Panel")]
    public GameObject levelCompletePanel;
    public GameObject levelFailPanel,exitPanel,mainPanel,gamePanel,settingPanel;
    [Header("Texts")]
    public TMP_Text levelNoTxt;
    public TMP_Text nextLevelTxt;
    public Button playButton;
    public GameObject musicOff;
    public GameObject soundOff;
    public LevelButton levelButton;
    public GameObject levelParent;
    private void OnEnable() 
    {    
        EventManager.onGamePlay.AddListener(StartGame);
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail);
    }
    private void OnDisable() 
    {
        EventManager.onGamePlay.RemoveListener(StartGame);
        EventManager.OnLevelComplete.RemoveListener(LevelComplete);
        EventManager.OnLevelFail.RemoveListener(LevelFail);
    }
    private void StartGame() 
    {
        GamePreference.selectedLevel=GamePreference.openLevels;
        OnEnable();
        //OnDisable();
        SpawnLevels();
        InitUIStates();
    }
    public void Start()
    {
        StartGame();
    }
    public void InitUIStates()
    {
        MusicButton(GamePreference.music);
        SoundButton(GamePreference.sound);
    }
    public void MusicButton(bool on)
    {
        musicOff.SetActive(!on);
        GamePreference.music=on;
        AudioManager.Instance.SetSFXSetting(on);
    }
    public void SoundButton(bool on)
    {
        soundOff.SetActive(!on);
        GamePreference.sound=on;
        AudioManager.Instance.SetBGSetting(on);
    }
    public void SettingButton(bool on)
    {
        if(on)Time.timeScale=0;
        else Time.timeScale=1;
        settingPanel.SetActive(on);
    }
    public void PlayButton()
    {
        LevelStart();
    }
    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
    }
    public void LevelStart()
    {
        mainPanel.SetActive(false);
        gamePanel.SetActive(true);
        levelCompletePanel.SetActive(false);
        levelFailPanel.SetActive(false);
        levelNoTxt.text=$""+(GamePreference.selectedLevel+1);
        nextLevelTxt.text=$""+(GamePreference.selectedLevel+2);
        EventManager.onGamePlay.Invoke();
    }
    public void NextLevel()
    {
        GamePreference.selectedLevel=GamePreference.openLevels;
        LevelStart();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LevelFail()
    {
        levelFailPanel.SetActive(true);
    }
    public void SpawnLevels()
    {
        for (int i = 0; i < levelEnemyConfig.levels.Count; i++)
        {
            levelButton=Instantiate(levelButton,levelParent.transform);
            levelButton.SetLevelNo(i);
        }
    }
    public void UpdateProgress(float progress)
    {
        Debug.Log("current progress"+progress);
        progressBar.fillAmount= progress;
    }
    public void ExitButton()
    {
        exitPanel.SetActive(true);
    }
    public void ExitGame(bool exit)
    {
        if (exit) Application.Quit();
        else exitPanel.SetActive(false);
    }
}
