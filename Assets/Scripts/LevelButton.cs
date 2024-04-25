using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TMP_Text noTxt;
    public int levelNo;
    public Button button;
    public void SetLevelNo(int no)
    {
        button.interactable = no <=  GamePreference.openLevels;
        noTxt.text = (no + 1).ToString();
        levelNo = no;
    }
    public void OnClick()
    {
        UIManager.instance.playButton.interactable=true;
        GamePreference.selectedLevel = levelNo;
    }
}
