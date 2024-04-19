using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public TMP_Text noTxt;
    public int levelNo;
    public void SetLevelNo(int no)
    {
        noTxt.text=no.ToString();
        levelNo=no;
    }
    public void OnClick()
    {
        
    }
}
