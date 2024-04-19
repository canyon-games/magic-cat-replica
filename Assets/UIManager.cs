using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public LevelEnemyConfig levelEnemyConfig;
    [Header("Panel")]
    public GameObject levelCompletePanel;
    public GameObject levelFailPanel,exitPanel;
    public LevelButton levelButton;
    public GameObject levelParent;
    private void OnEnable() 
    {    
        EventManager.OnLevelComplete.AddListener(LevelComplete);
        EventManager.OnLevelFail.AddListener(LevelFail);
    }
    private void OnDisable() 
    {
        EventManager.OnLevelComplete.RemoveListener(LevelComplete);
        EventManager.OnLevelFail.RemoveListener(LevelFail);
    }
    private void Start() {
        SpawnLevels();
    }
    public void LevelComplete()
    {
        levelCompletePanel.SetActive(true);
    }
    public void LevelStart()
    {

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
            levelButton.SetLevelNo(i+1);
        }
    }
    public void ExitButton()
    {
        exitPanel.SetActive(true);
    }
}
