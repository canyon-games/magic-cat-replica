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
        //EventManager.onGamePlay.Invoke();    
    }
    public void PlayGame()
    {
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail); 
        Time.timeScale=1;
    }
    public void LevelComplete()
    {
        //Time.timeScale=0;
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
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail); 
    }
    public void LevelFail()
    {
        //Time.timeScale=0;
        EventManager.OnLevelFail.RemoveAllListeners();
        EventManager.OnLevelComplete.RemoveAllListeners();
        AudioManager.Instance.PlaySFX(SFX.Fail);
    }
}
