using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;
    private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public EnemiesManager enemyManager;
    private void OnEnable() 
    {    
        EventManager.onGamePlay.AddListener(PlayGame);
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail);
    }
    private void OnDisable() 
    {
        EventManager.OnLevelComplete.RemoveListener(LevelComplete);
        EventManager.OnLevelFail.RemoveListener(LevelFail);
    }
    private void Start() 
    {
        Time.timeScale=1;
        AudioManager.Instance.StartGame();
        //EventManager.onGamePlay.Invoke();    
    }
    public void PlayGame()
    {
        Time.timeScale=1;
        AudioManager.Instance.StartGame();
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail); 
    }
    public void LevelComplete()
    {
        Time.timeScale=0;
        AudioManager.Instance.GameEnd();
        print("selected leve "+ GamePreference.selectedLevel+ " open level "+GamePreference.openLevels);
        if(GamePreference.selectedLevel==GamePreference.openLevels&&GamePreference.openLevels<9)
        {
            GamePreference.openLevels++;
        }
        AudioManager.Instance.PlaySFX(SFX.Complete);
        EventManager.OnLevelFail.RemoveAllListeners();
        EventManager.OnLevelComplete.RemoveAllListeners();
    }
    public void LevelStart()
    {
        print("selected leve "+ GamePreference.selectedLevel+ " open level "+GamePreference.openLevels);
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail); 
    }
    public void LevelFail()
    {
        Time.timeScale=0;
        AudioManager.Instance.GameEnd();
        EventManager.OnLevelFail.RemoveAllListeners();
        EventManager.OnLevelComplete.RemoveAllListeners();
        AudioManager.Instance.PlaySFX(SFX.Fail);
    }
}
