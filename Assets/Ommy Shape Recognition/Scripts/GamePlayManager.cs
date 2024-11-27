using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OmmyShapeML
{

public class GamePlayManager : MonoBehaviour
{
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
    public int currentLevel;
    public void LevelComplete()
    {

    }
    public void LevelStart()
    {

    }
    public void LevelFail()
    {

    }
}
}