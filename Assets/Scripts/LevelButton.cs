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
    public GameObject bossSign;
    public GameObject currentObject;
    public void SetLevelNo(int no,bool hasBossLevel)
    {
        button.interactable = no <= GamePreference.openLevels;
        noTxt.text = (no + 1).ToString();
        levelNo = no;
        if (no == GamePreference.openLevels)
        {
            print("Set Current Level UI");
            //UIManager.instance.SnapTo(transform.GetComponent<RectTransform>());
            button.image.color = Color.green;
            //currentObject?.SetActive(true);
        }
        else
        {
            button.image.color = Color.white;
        }
        bossSign.SetActive(hasBossLevel);
    }
    public void OnClick()
    {
        UIManager.instance.playButton.interactable = true;
        GamePreference.selectedLevel = levelNo;
        UIManager.instance.SelectLevel(this);
    }
}
