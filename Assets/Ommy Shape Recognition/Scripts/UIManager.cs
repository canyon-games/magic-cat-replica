using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace OmmyShapeML
{
public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject levelCompletePanel;
    public GameObject levelFailPanel;
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
}
}