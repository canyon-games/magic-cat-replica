using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreference
{
    static readonly string CoinsPrefs = "Coins";
    static readonly string OpenLevelPrefs = "OpenLevels";
    static readonly string SoundPrefs = "Sound";
    static readonly string MusicPrefs = "Muisc";
    static readonly string VibrationPrefs = "Vibration";

    public static readonly string removeAdsID = "com.removeads.magicCat";
    public static int Coins
    {
        get => PlayerPrefs.GetInt(CoinsPrefs, 0);
        set => PlayerPrefs.SetInt(CoinsPrefs, value);
    }
    public static int loadedLevel=0;
    public static int selectedLevel = 0;
    public static int openLevels
    {
        get => PlayerPrefs.GetInt(OpenLevelPrefs, 0);
        set
        {
            //if(value>9)return;
            PlayerPrefs.SetInt(OpenLevelPrefs, value);
        } 
    }
    public static int GetRandomLevel()
    {
        int totalLevels = 9;  // Total number of levels
        int loadLevel = Random.Range(0, totalLevels+1);
        if (loadLevel == loadedLevel)
        {
            if (loadLevel < totalLevels)
            {
                loadLevel++;
            }
            else
            {
                loadLevel--;
            }
        }
        loadedLevel = loadLevel; 

        return loadLevel;
    }
    public static bool tutorial
    {
        get
        {
            if (PlayerPrefs.GetInt("Tutorial", 1) == 1)
            {
                return true;
            }

            return false;
        }
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("Tutorial",1);
            }
            else
            {
                PlayerPrefs.SetInt("Tutorial",0);
            }
        }
    }
    public static bool gameFinished
    {
        get
        {
            if (PlayerPrefs.GetInt("GameFinished", 0) == 1)
            {
                return true;
            }

            return false;
        }
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("GameFinished",1);
            }
            else
            {
                PlayerPrefs.SetInt("GameFinished",0);
            }
        }
    }
    public static bool sound
    {
        get
        {
            if(PlayerPrefs.GetInt(SoundPrefs,1)==1)
            {
                return true;
            }
            return false;
        } 
        set
        {
            if(value)
            {
                PlayerPrefs.SetInt(SoundPrefs,1);
                return;
            }
            PlayerPrefs.SetInt(SoundPrefs,0);
        }
    }
    public static bool music
    {
        get
        {
            if(PlayerPrefs.GetInt(MusicPrefs,1)==1)
            {
                return true;
            }
            return false;
        } 
        set
        {
            if(value)
            {
                PlayerPrefs.SetInt(MusicPrefs,1);
                return;
            }
            PlayerPrefs.SetInt(MusicPrefs,0);
        }
    }
    public static bool vibration
    {
        get
        {
            if(PlayerPrefs.GetInt(VibrationPrefs,1)==1)
            {
                return true;
            }
            return false;
        } 
        set
        {
            if(value)
            {
                PlayerPrefs.SetInt(VibrationPrefs,1);
                return;
            }
            PlayerPrefs.SetInt(VibrationPrefs,0);
        }
    }
}
